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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Reflection;
using System.Net.Http;
using FuseCP.Providers.OS;

namespace FuseCP.Tests
{
    public class Kestrel: IDisposable
    {
		Process process = null;

		public (Scheme Protocol, string Url)[] Urls;
		public string HttpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Http).Url;
		public string HttpsUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.Https).Url;
		public string NetTcpUrl => Urls.FirstOrDefault(u => u.Protocol == Scheme.NetTcp).Url;
		readonly Component component;
		readonly WSLShell.WSLDistro WslDistro;
		readonly Scheme readinessProtocol;
		static readonly List<Kestrel> instances = new List<Kestrel>();
		public Kestrel(Component component, (Scheme, string)[] urls = null, WSLShell.WSLDistro wslDistro = null, Scheme readinessProtocol = Scheme.Http)
		{
			Urls = urls;
			this.component = component;
			this.WslDistro = wslDistro;
			this.readinessProtocol = readinessProtocol;
			instances.Add(this);
			var apppath = Paths.Path(component);
			var testdllpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var testprojpath = Path.GetFullPath(Path.Join(testdllpath, "..", "..", ".."));
			var workingDir = Path.Join(apppath, "bin_dotnet");
			var log = Path.GetFullPath(Path.Join(Paths.Test, "TestResults", $"Kestrel.log"));
			if (!Directory.Exists(Path.GetDirectoryName(log))) Directory.CreateDirectory(Path.GetDirectoryName(log));
			var dll = Path.Join(workingDir, $"{Paths.App}.{component}.dll");
			var pfx = Certificate.CertFilePath;

			Shell shell;
			if (wslDistro == null) shell = Shell.Standard.Clone;
			else
			{
				shell = new WSLShell(wslDistro);

				Paths.Wsl(testdllpath);
				Paths.Wsl(testprojpath);
				dll = Paths.Wsl(dll);
				pfx = Paths.Wsl(pfx);
			}
			var distro = (wslDistro?.ToString() ?? "Windows");
			WriteTestOverlay(workingDir, pfx, $"{HttpUrl};{HttpsUrl}");

			var exe = wslDistro != null ? "/usr/lib/dotnet/dotnet" : shell.Find("dotnet");
			if (wslDistro != null)
			{
				// Use ExecScriptAsync so the shell metacharacters (&&) in the script are
				// handled by bash inside WSL rather than being validated as argument tokens.
				var dotnetExists = shell.ExecScriptAsync($"test -x \"{exe}\" && echo ok").Output().Result.Trim() == "ok";
				if (!dotnetExists)
				{
					var discoveredExe = shell.Find("dotnet");
					if (!string.IsNullOrWhiteSpace(discoveredExe)) exe = discoveredExe;
				}
			}
			if (string.IsNullOrWhiteSpace(exe))
			{
				throw new FileNotFoundException(wslDistro != null
					? "Could not find dotnet in WSL. Expected /usr/lib/dotnet/dotnet or a PATH entry for dotnet."
					: "Could not find dotnet in PATH.");
			}
			shell.LogFile = log;
			shell.LogCommand += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"{distro}>{msg}");
				Console.Write($"{distro}>{msg}");
			};
			shell.Log += msg =>
				{
					if (Debugger.IsAttached) Debug.Write($"{distro}/Kestrel>{msg}");
					Console.Write($"{distro}/Kestrel>{msg}");
				};
			shell.LogError += msg =>
			{
				if (Debugger.IsAttached) Debug.Write($"{distro}/Kestrel>{msg}");
				var mainColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write($"{distro}/Kestrel>{msg}");
				Console.ForegroundColor = mainColor;
			};
			shell.Environment = new Dictionary<string, string>()
			{
				{ "ASPNETCORE_ENVIRONMENT", "Development" },
				//{ "ASPNETCORE_Kestrel__Certificates__Default__Path", pfx },
				//{ "ASPNETCORE_Kestrel__Certificates__Default__Password", Certificate.Password },
				//{ "ServerCertificate__File", pfx },
				//{ "ServerCertificate__Password", Certificate.Password },
			};
			shell.WorkingDirectory = workingDir;

			shell.Exec("dotnet dev-certs https");
			/*var cert = shell.ExecAsync("dotnet dev-certs https --trust");
			cert.Input.WriteLine("yes");
			cert.Wait();*/

			// URLs are written to appsettings.hardened.json by WriteTestOverlay above;
			// the server reads applicationUrls from config. Do not pass --urls on the
			// command line because the semicolon separator is rejected as a shell
			// metacharacter by Shell's argument validator.
			var launchCommand = $"\"{exe}\" \"{dll}\"";

			process = shell.ExecAsync(launchCommand).Process;
			if (process == null) throw new InvalidOperationException($"Could not start Kestrel: executable not found at '{exe}'");

			try
			{
				if (WslDistro == null && process.HasExited) throw new Exception($"Kestrel exited with code {process.ExitCode}");

				// Wait for the protocol endpoint requested by the test row.
				// This avoids racing into HTTPS requests while only HTTP is ready.
				var readinessUrls = GetReadinessUrls();
				const int maxRetries = 30;
				var isReady = false;
				for (var n = 0; n < maxRetries; n++)
				{
					if (process.HasExited)
					{
						throw new Exception($"Kestrel process terminated before readiness check completed (exit code {process.ExitCode}).");
					}

					if (readinessUrls.Any(TryProbe))
					{
						isReady = true;
						break;
					}

					Thread.Sleep(2000);
				}

				if (!isReady)
				{
					throw new TimeoutException($"Kestrel endpoint readiness timed out for component '{component}' and protocol '{readinessProtocol}'. Probed URLs: {string.Join(", ", readinessUrls)}");
				}
			}
			catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
			{
				if (!process.HasExited)
				{
					process.Kill();
				}
				throw;
			}
        }

		string[] GetReadinessUrls()
		{
			var protocolUrl = Urls.FirstOrDefault(u => u.Protocol == readinessProtocol).Url;
			if (!string.IsNullOrWhiteSpace(protocolUrl))
			{
				return new[] { protocolUrl };
			}

			if (!string.IsNullOrWhiteSpace(HttpUrl) || !string.IsNullOrWhiteSpace(HttpsUrl))
			{
				return new[] { HttpUrl, HttpsUrl }.Where(u => !string.IsNullOrWhiteSpace(u)).ToArray();
			}

			return Array.Empty<string>();
		}

		static void WriteTestOverlay(string workingDir, string certificateFile, string applicationUrls)
		{
			if (!Directory.Exists(workingDir))
			{
				Directory.CreateDirectory(workingDir);
			}

			var overlayPath = Path.Join(workingDir, "appsettings.hardened.json");
			var escapedCertificateFile = certificateFile?.Replace("\\", "\\\\") ?? string.Empty;
			var escapedApplicationUrls = applicationUrls?.Replace("\\", "\\\\") ?? string.Empty;
			const string testServerPassword = "cRDtpNCeBiql5KOQsKVyrA0sAiA=";
			var overlay = "{\n" +
				$"  \"applicationUrls\": \"{escapedApplicationUrls}\",\n" +
				"  \"Server\": {\n" +
				$"    \"Password\": \"{testServerPassword}\",\n" +
				"    \"AllowLegacyPasswordAuthentication\": true\n" +
				"  },\n" +
				"  \"ServerCertificate\": {\n" +
				$"    \"File\": \"{escapedCertificateFile}\",\n" +
				$"    \"Password\": \"{Certificate.Password}\"\n" +
				"  }\n" +
				"}";

			File.WriteAllText(overlayPath, overlay);
		}

		static bool TryProbe(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
			{
				return false;
			}

			try
			{
				_ = Servers.HttpClient.GetAsync(url).Result;
				return true;
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				return false;
			}
		}

		public void Dispose()
		{
			instances.Remove(this);

			if (process != null && !process.HasExited) process.Kill();
			process = null;

			if (WslDistro != null)
			{
				if (!instances.Any(k => k.WslDistro == WslDistro))
				{
					WSLShell.Default.Terminate(WslDistro);
				} 
				if (!instances.Any(k => k.WslDistro != null)) {
					WSLShell.Default.ShutdownAll();
				}
			}
        }
	}
}
