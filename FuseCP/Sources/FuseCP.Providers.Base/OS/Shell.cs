// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

namespace FuseCP.Providers.OS
{

	public abstract class Shell : INotifyCompletion
	{
		static int N = 0;

		public int ShellId = N++;
		public Shell() : base()
		{
			output = new StringBuilder();
			error = new StringBuilder();
			outputAndError = new StringBuilder();
			Log += OnLog;
			LogCommand += OnLogCommandHandler;
			LogCommandEnd += OnLogCommandEnd;
			LogError += OnLogError;
			LogOutput += OnLogOutput;
		}

		void OnLogCommandHandler(string text) => OnLogCommand(text);

		protected SemaphoreSlim Lock = new SemaphoreSlim(1, 1);
		protected SemaphoreSlim OutputLock = new SemaphoreSlim(1, 1);
		protected SemaphoreSlim ErrorLock = new SemaphoreSlim(1, 1);
		protected SemaphoreSlim OutputAndErrorLock = new SemaphoreSlim(1, 1);

		//methods to support await on Shell type
		public Shell GetAwaiter() => this;

		Action Continuation = null;
		public void OnCompleted(Action continuation)
		{
			Lock.Wait();
			Continuation += continuation;
			Lock.Release();

			CheckCompleted();
		}

		bool errorEOF = true, outputEOF = true, hasProcessExited = true;
		int exitCode = 0;
		bool checkHasExited = true;

		// Process.HasExited can cause deadlock because it raises Exit event, therefore disable it in CheckCompleted by checkHasExited = false
		public bool IsCompleted
		{
			get
			{
				if (Process == null)
				{
					return true;
				}

				var isProcessExitSatisfied = hasProcessExited;
				if (!isProcessExitSatisfied && checkHasExited)
				{
					isProcessExitSatisfied = Process.HasExited;
				}

				return isProcessExitSatisfied && errorEOF && outputEOF;
			}
		}
		public Shell GetResult() => this;
		public Shell Parent { get; set; } = null;
		public virtual char PathSeparator => Path.PathSeparator;
		public bool CreateNoWindow = true;
		public virtual string WorkingDirectory { get; set; } = null;
		public Encoding Encoding = null;
		public Dictionary<string, string> Environment = new Dictionary<string, string>();

		public ProcessWindowStyle WindowStyle = ProcessWindowStyle.Minimized;
		public abstract string ShellExe { get; }

		Process process;
		public virtual Process Process
		{
			get { return process; }
			protected set
			{
				if (process != value)
				{
					Lock.Wait();
					hasProcessExited = outputEOF = errorEOF = value == null;
					process = value;
					Lock.Release();
				}
			}
		}

		public bool NotFound { get; set; }
		public static IEnumerable<string> Paths
		{
			get
			{
				string proc, machine = "", user = "";
				string[] sources;
				proc = System.Environment.GetEnvironmentVariable("PATH");
				if (IsWindows)
				{
					machine = System.Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
					user = System.Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
					var localProcess = System.Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
					sources = new string[] {
						System.Environment.GetFolderPath(System.Environment.SpecialFolder.System),
						System.Environment.GetFolderPath(System.Environment.SpecialFolder.SystemX86),
						localProcess, machine, user };
				}
				else sources = new string[] { proc };

				return sources
					.SelectMany(paths => paths.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries))
					.Select(path => path.Trim())
					.Distinct();
			}
		}

		public virtual string Find(string cmd)
		{
			string file = null;
			cmd = cmd.Trim('"');
			if (cmd.IndexOf(Path.DirectorySeparatorChar) >= 0)
			{
				if (File.Exists(cmd)) file = cmd;
			}
			else
			{
				file = Paths
					  .SelectMany(p =>
					  {
						  var p1 = Path.Join(p, cmd.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
						  return new string[] { p1, Path.ChangeExtension(p1, "exe") };
					  })
					  .FirstOrDefault(p => File.Exists(p));
			}
			NotFound = file == null;
			return file;
		}

		static IEnumerable<string> TokenizeArguments(string arguments)
		{
			if (string.IsNullOrWhiteSpace(arguments))
				yield break;

		foreach (var token in Regex.Matches(arguments, @"(?:[^\s\""']+|\""(?:\\.|[^\""])*\""|'(?:\\.|[^'])*')+").Cast<Match>().Select(match => match.Value))
		{
			var processedToken = token;
			var startsAndEndsWithDoubleQuotes = token[0] == '"' && token[token.Length - 1] == '"';
			var startsAndEndsWithSingleQuotes = token[0] == '\'' && token[token.Length - 1] == '\'';
			if (token.Length >= 2 && (startsAndEndsWithDoubleQuotes || startsAndEndsWithSingleQuotes))
				processedToken = token.Substring(1, token.Length - 2);
			yield return processedToken;
			}
		}

		static string QuoteArgument(string argument)
		{
			if (string.IsNullOrEmpty(argument))
				return "\"\"";
			if (!argument.Any(char.IsWhiteSpace) && argument.IndexOf('"') < 0)
				return argument;
			return "\"" + argument.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
		}

		static void ApplyArguments(ProcessStartInfo startInfo, string arguments)
		{
			foreach (var token in TokenizeArguments(arguments))
				startInfo.ArgumentList.Add(token);
		}

		static bool ContainsShellMetaCharacters(string value)
		{
			if (string.IsNullOrEmpty(value))
				return false;

			return value.IndexOfAny(new[] { '&', '|', ';', '>', '<', '`' }) >= 0;
		}

		static bool IsSafeCommandToken(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return false;

			if (value.IndexOf(Path.DirectorySeparatorChar) >= 0 || value.IndexOf(Path.AltDirectorySeparatorChar) >= 0)
				return false;

			return Regex.IsMatch(value, @"^[A-Za-z0-9._\-]+$");
		}

		protected virtual string ToTempFile(string script)
		{
			var file = Path.GetTempFileName();
			File.WriteAllText(file, script);
			return file;
		}

		void CheckCompleted()
		{
			Action cnt = null;
			var exited = Process.HasExited;
			Lock.Wait();
			try
			{
				hasProcessExited = hasProcessExited || exited;
				checkHasExited = false;
				if (IsCompleted && Continuation != null)
				{
					cnt = Continuation;
					Continuation = null;
				}
				checkHasExited = true;
			}
			finally
			{
				Lock.Release();
			}
			cnt?.Invoke();
		}

		public virtual StreamWriter StandardInput => process.StandardInput;
		public virtual Shell ExecAsync(string cmd, Encoding encoding = null, Dictionary<string, string> environment = null)
		{
			if (string.IsNullOrWhiteSpace(cmd))
				throw new ArgumentException("Command cannot be null or empty.", nameof(cmd));
			if (cmd.IndexOf('\0') >= 0 || cmd.IndexOf('\r') >= 0 || cmd.IndexOf('\n') >= 0)
				throw new ArgumentException("Command contains invalid control characters.", nameof(cmd));

			LogCommand?.Invoke(cmd);

			// separate command from arguments
			string arguments;
			if (cmd.Length > 0 && cmd[0] == '"') // command is a " delimited string
			{
				var pos = cmd.IndexOf('"', 1);
				if (pos >= 1)
				{
					if (pos < cmd.Length - 1)
					{
						arguments = cmd.Substring(pos + 1).Trim();
						cmd = cmd.Substring(1, pos - 1);
					}
					else
					{
						cmd = cmd.Substring(1, pos - 1);
						arguments = "";
					}
				}
				else
				{
					cmd = cmd.Substring(1);
					arguments = "";
				}
			}
			else // command is the first token of space separated tokens
			{
				var pos = cmd.IndexOf(' ');
				if (pos >= 0 && pos < cmd.Length - 1)
				{
					arguments = cmd.Substring(pos + 1);
					cmd = cmd.Substring(0, pos);
				}
				else arguments = "";
			}

			if (arguments.IndexOf('\0') >= 0 || arguments.IndexOf('\r') >= 0 || arguments.IndexOf('\n') >= 0)
				throw new ArgumentException("Arguments contain invalid control characters.", nameof(cmd));

			if (ContainsShellMetaCharacters(cmd))
				throw new ArgumentException("Command contains invalid shell meta characters.", nameof(cmd));

			if (!IsSafeCommandToken(cmd))
				throw new ArgumentException("Command contains invalid characters.", nameof(cmd));

			var cmdWithPath = Find(cmd);
			if (cmdWithPath != null)
			{
				var child = Clone;
				var local_process = new Process();
				child.Process = local_process;
				local_process.StartInfo.FileName = cmdWithPath;
				ApplyArguments(local_process.StartInfo, arguments);
				local_process.StartInfo.UseShellExecute = false;
				local_process.StartInfo.CreateNoWindow = CreateNoWindow;
				local_process.StartInfo.WindowStyle = WindowStyle;
				local_process.StartInfo.WorkingDirectory = WorkingDirectory ??
					local_process.StartInfo.WorkingDirectory;
				local_process.StartInfo.RedirectStandardOutput = true;
				local_process.StartInfo.RedirectStandardError = true;
				local_process.StartInfo.RedirectStandardInput = true;
				local_process.StartInfo.StandardOutputEncoding = encoding ?? Encoding ?? Encoding.Default;
				local_process.StartInfo.StandardErrorEncoding = encoding ?? Encoding ?? Encoding.Default;
				var env = environment ?? Environment;
				if (env != null)
				{
					foreach (var variable in env)
					{
						if (!local_process.StartInfo.EnvironmentVariables.ContainsKey(variable.Key))
							local_process.StartInfo.EnvironmentVariables.Add(variable.Key, variable.Value);
						else
							local_process.StartInfo.EnvironmentVariables[variable.Key] = variable.Value;
					}
				}
				local_process.Exited += (obj, args) =>
				{
					child.exitCode = child.Process.ExitCode;
					child.Lock.Wait();
					child.hasProcessExited = true;
					child.Lock.Release();

					child.CheckCompleted();
				};
				local_process.EnableRaisingEvents = true;
				local_process.ErrorDataReceived += (p, data) =>
				{
					if (data.Data == null)
					{
						child.Lock.Wait();
						child.errorEOF = true;
						child.Lock.Release();

						child.CheckCompleted();
					}
					else
					{
						var line = $"{data.Data}{System.Environment.NewLine}";
						var shell = child;
						while (shell != null)
						{
							shell.Log?.Invoke(line);
							shell.LogError?.Invoke(line);
							shell = shell.Parent;
						}
					}
				};
				local_process.OutputDataReceived += (p, data) =>
				{
					if (data.Data == null)
					{
						child.Lock.Wait();
						child.outputEOF = true;
						child.Lock.Release();

						child.CheckCompleted();
						LogCommandEnd?.Invoke();
					}
					else
					{
						var line = $"{data.Data}{System.Environment.NewLine}";
						var shell = child;
						while (shell != null)
						{
							shell.Log?.Invoke(line);
							shell.LogOutput?.Invoke(line);
							shell = shell.Parent;
						}
					}
				};
				local_process.Start();
				local_process.BeginOutputReadLine();
				local_process.BeginErrorReadLine();
				local_process.StandardInput.AutoFlush = true;
				return child;
			}
			else
			{
				LogError?.Invoke($"Error {cmd} not found.{System.Environment.NewLine}");
				var child = Clone;
				child.Process = null;
				child.NotFound = true;
				return child;
			}
		}
		public virtual Shell Exec(string command, Encoding encoding = null, Dictionary<string, string> environment = null) => ExecAsync(command, encoding, environment).Task().Result;
		public virtual Shell Clone
		{
			get
			{
				Shell clone = Activator.CreateInstance(GetType()) as Shell
					?? throw new InvalidOperationException($"Unable to clone shell type {GetType().FullName}.");
				clone.Parent = this;
				clone.CreateNoWindow = this.CreateNoWindow;
				clone.WindowStyle = this.WindowStyle;
				clone.WorkingDirectory = this.WorkingDirectory;
				clone.Encoding = this.Encoding;
				clone.Environment = new Dictionary<string, string>();
				foreach (var item in this.Environment) clone.Environment.Add(item.Key, item.Value);

				return clone;
			}
		}

		public virtual Shell SilentClone
		{
			get
			{
				var clone = Clone;
				clone.Log = clone.LogCommand = clone.LogOutput = clone.LogError = null;
				clone.Parent = null;
				return clone;
			}
		}

		public virtual Shell ExecScriptAsync(string script, string args = null, Encoding encoding = null, Dictionary<string, string> environment = null)
		{
			script = script.Trim();
			// adjust new lines to OS type
			script = Regex.Replace(script, @"\r?\n", System.Environment.NewLine);
			var file = ToTempFile(script.Trim());
			var cmd = new StringBuilder();
			cmd.Append(ShellExe);
			cmd.Append(" \"");
			cmd.Append(file);
			cmd.Append("\"");
			if (args != null)
			{
				cmd.Append(" ");
				cmd.Append(args);
			}
			var shell = ExecAsync(cmd.ToString(), encoding, environment);
			if (shell.Process != null)
			{
				shell.Process.Exited += (sender, args) =>
				{
					File.Delete(file);
				};
			}
			return shell;
		}

		public virtual Shell ExecScript(string script, string args = null, Encoding encoding = null, Dictionary<string, string> environment = null)
			=> ExecScriptAsync(script, args, encoding, environment).Task().Result;


		/* public virtual async Task<Shell> Wait(int milliseconds = Timeout.Infinite)
		{
			if (milliseconds == Timeout.Infinite) Process.WaitForExit();
			else Process.WaitForExit(milliseconds);
			return await this;
		} */

		public Action<string> Log { get; set; }
		public Action<string> LogCommand { get; set; }
		public Action LogCommandEnd { get; set; }
		public Action<string> LogOutput { get; set; }
		public Action<string> LogError { get; set; }

		readonly StringBuilder output, error, outputAndError;

		public async Task<Shell> Task()
		{
			return await this;
		}

		public void Wait() => Task().Wait();

		public async Task<string> Output()
		{
			if (Process == null && NotFound) return null;
			await this;
			await OutputLock.WaitAsync();
			try
			{
				return output.ToString();
			}
			finally
			{
				OutputLock.Release();
			}
		}

		public async Task<string> Error()
		{
			if (Process == null && NotFound) return null;
			await this;
			await ErrorLock.WaitAsync();
			try
			{
				return error.ToString();
			}
			finally
			{
				ErrorLock.Release();
			}
		}
		public async Task<string> OutputAndError()
		{
			if (Process == null && NotFound) return null;
			await this;
			await OutputAndErrorLock.WaitAsync();
			try
			{
				return outputAndError.ToString();
			}
			finally
			{
				OutputAndErrorLock.Release();
			}
		}

		public async Task<int> ExitCode()
		{
			if (Process == null && NotFound) return -500;
			await this;
			return exitCode;
		}
		public bool Redirect = false;
		public string LogFile = null;
		protected void AppendAllText(string filename, string text)
		{
			try
			{
				using (var file = new FileStream(filename, FileMode.Append, FileAccess.Write))
				using (var writer = new StreamWriter(file, Encoding.UTF8))
				{
					writer.Write(text);
				}
			}
			catch (IOException ex) { System.Diagnostics.Trace.TraceWarning("AppendAllText IO exception: " + ex.Message); }
			catch (UnauthorizedAccessException ex) { System.Diagnostics.Trace.TraceWarning("AppendAllText unauthorized: " + ex.Message); }
		}
		protected void OnLog(string text)
		{
			OutputAndErrorLock.Wait();
			try
			{
				outputAndError.Append(text);
				if (LogFile != null) AppendAllText(LogFile, text);
			}
			finally
			{
				OutputAndErrorLock.Release();
			}
		}

		protected virtual void OnLogCommand(string text)
		{
			OutputAndErrorLock.Wait();
			try
			{
				text = $"> {text}";
				if (Redirect) Console.WriteLine(text);
				if (LogFile != null) AppendAllText(LogFile, text);
			}
			finally
			{
				OutputAndErrorLock.Release();
			}
		}
		protected void OnLogCommandEnd()
		{
			OutputAndErrorLock.Wait();
			try
			{
				if (Redirect) Console.WriteLine();
				if (LogFile != null) AppendAllText(LogFile, System.Environment.NewLine);
			} finally
			{
				OutputAndErrorLock.Release();
			}
		}
		protected void OnLogOutput(string text)
		{
			OutputLock.Wait();
			try
			{
				output.Append(text);
				if (Redirect) Console.Write(text);
			}
			finally
			{
				OutputLock.Release();
			}
		}
		protected void OnLogError(string text)
		{
			ErrorLock.Wait();
			try
			{
				error.Append(text);
				if (Redirect) Console.Error.Write(text);
			}
			finally
			{
				ErrorLock.Release();
			}
		}
		public StreamWriter Input => Process?.StandardInput;

		static Shell standard = null;
		public static Shell Standard => standard ??= new StandardShell();

#if wpkg
		public readonly static Shell Default = new StandardShell(); // OSInfo.Current.DefaultShell;
		public static bool IsWindows => System.Environment.OSVersion.Platform == PlatformID.Win32NT;
#else
		public static Shell Default => OSInfo.Current.DefaultShell;
		public static bool IsWindows => OSInfo.IsWindows;
#endif
	}

	public class StandardShell : Shell
	{
		public override string ShellExe => Shell.IsWindows ? "cmd" : "sh";
	}
}
