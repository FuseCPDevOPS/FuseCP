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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
#if !NETSTANDARD2_0
using System.Runtime.Versioning;
#endif
using System.IO;

namespace FuseCP.Providers.OS
{

	public enum OSPlatform { Unknown = 0, Windows, Mac, Linux, Unix, Other };
	public enum OSFlavor { Unknown = 0, Min = 0, Windows, Mac, Debian, Mint, Kali, Ubuntu, Fedora, RedHat, Oracle, CentOS, Alma, Rocky, SUSE, Alpine, Arch, FreeBSD, NetBSD, Other, Max = Other }

	public class OSInfo
	{
		public static bool IsMono => Type.GetType("Mono.Runtime") != null;
		public static bool IsCore => !(IsNetFX || IsNetNative);
		public static bool IsNetFX => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
		public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
		public static bool IsWindows => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
		public static bool IsWindowsServer => WindowsVersion.ToString().Contains("WindowsServer");
		public static bool IsLinux => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
		public static bool IsMac => RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
		public static bool IsArm => Architecture == Architecture.Arm64 || Architecture == Architecture.Arm;
		public static bool IsIntel => Architecture == Architecture.X64 || Architecture == Architecture.X86;
		public static bool IsNet48 => !IsMono && IsWindows && Regex.IsMatch(RuntimeInformation.FrameworkDescription, @"^\.NET Framework 4\.[8-9]");
		public static bool Is64 => Environment.Is64BitOperatingSystem;
		public static bool Is32 => !Is64;
		public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;

		public static readonly System.Runtime.InteropServices.OSPlatform FreeBSD = System.Runtime.InteropServices.OSPlatform.Create("FREEBSD");
		public static readonly System.Runtime.InteropServices.OSPlatform NetBSD = System.Runtime.InteropServices.OSPlatform.Create("NETBSD");

		public static bool IsUnix => IsLinux || IsMac || IsFreeBSD || IsNetBSD;
		public static bool IsFreeBSD => RuntimeInformation.IsOSPlatform(FreeBSD);
		public static bool IsNetBSD => RuntimeInformation.IsOSPlatform(NetBSD);
		public static bool IsSystemd => IsLinux && Directory.Exists("/run/systemd/system");
		public static bool IsWSL => IsLinux && File.Exists("/proc/version") && File.ReadAllText("/proc/version").ToLower().Contains("microsoft");
		public static bool IsOpenRC => IsLinux && File.Exists("etc/rc") && Shell.Standard.Find("rc-status") != null;

		static OSFlavor flavor = OSFlavor.Unknown;
		static Version version = new Version("0.0.0.0");

#if !NETSTANDARD2_0
		[SupportedOSPlatformGuard("windows")]
#endif
		private static bool IsWindowsPlatform() => IsWindows;

		public static string NetVersion
		{
			get
			{
				if (IsMono || IsCore) return Regex.Match(FrameworkDescription, @"[0-9]+(\.[0-9]+)?").Value;

				return IsWindowsPlatform() ? WindowsOSInfo.NetFXVersion : String.Empty;
			}
		}
		public static string NetFXVersion => IsWindowsPlatform() ? WindowsOSInfo.NetFXVersion : String.Empty;

		public static string NetDescription
		{
			get
			{
				if (IsMono || IsCore) return FrameworkDescription;
				return $".NET Framework {NetVersion}";
			}
		}
		public static OSPlatform OSPlatform => IsWindows ? OSPlatform.Windows :
			 (IsMac ? OSPlatform.Mac :
			 (IsLinux ? OSPlatform.Linux :
			 (IsNetBSD || IsFreeBSD ? OSPlatform.Unix : OSPlatform.Other)));

		public static Architecture Architecture => RuntimeInformation.ProcessArchitecture;
		public static OSFlavor OSFlavor
		{
			get
			{
				if (flavor != OSFlavor.Unknown) return flavor;
				version = Environment.OSVersion.Version;
				if (IsWindows) return OSFlavor.Windows;
				if (IsMac) return OSFlavor.Mac;
				if (IsFreeBSD) return OSFlavor.FreeBSD;
				if (IsNetBSD) return OSFlavor.NetBSD;
				if (IsLinux)
				{
					string name = null;
					const string OsReleaseFile = "/etc/os-release";
					if (File.Exists(OsReleaseFile))
					{
						var osRelease = File.ReadAllText(OsReleaseFile);
						var match = Regex.Match(osRelease, "(?<=^NAME\\s*=\\s*\")[^\"]+(?=\")", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						if (match.Success) name = match.Value;
						match = Regex.Match(osRelease, @"(?<=^VERSION_ID\s*=\s*""[^""0-9]*?)[0-9]+(\.[0-9]+)?(\.[0-9]+)?(\.[0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						Version osReleaseVersion;
						int intVersion;
						if (match.Success)
						{
							if (Version.TryParse(match.Value, out osReleaseVersion)) version = osReleaseVersion;
							else if (int.TryParse(match.Value, out intVersion))
							{
								version = new Version(intVersion, 0);
							}
						}
					}
					if (name == null)
					{
						var osRelease = Shell.Standard.Exec("lsb_release -a").Output().Result;
						var match = Regex.Match(osRelease, "(?<=^Distributor ID\\s*:\\s*)[^\\s$]+(?=\\s|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						if (match.Success) name = match.Value;
						match = Regex.Match(osRelease, @"(?<=^Release\s*:[^0-9]*?)[0-9]+\.[0-9]+(\.[0-9]+)?(\.[0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						if (match.Success) Version.TryParse(match.Value, out version);
					}
					// TODO use hostnamectl
					OSFlavor f;
					if (name == null) flavor = OSFlavor.Other;
					else
					{
						if (name != "Linux" && name != "linux" && name.EndsWith("linux", StringComparison.OrdinalIgnoreCase))
						{
							name = name.Substring(0, name.Length - "linux".Length);
						}
						if (Enum.TryParse<OSFlavor>(name, out f)) flavor = f;
						else
						{
							for (var local_os = OSFlavor.Min; local_os <= OSFlavor.Max; local_os++)
							{
								if (Regex.IsMatch(name, $"(?<=^|\\s){Regex.Escape(Enum.GetName(typeof(OSFlavor), local_os))}(?=\\s|$)", RegexOptions.IgnoreCase))
								{
									flavor = local_os;
									break;
								}
							}
						}
					}
				}
				return flavor == OSFlavor.Unknown ? OSFlavor.Other : flavor;
			}
		}
		public static Version OSVersion
		{
			get
			{
				return version;
			}
		}

		public static string FuseCPVersion
		{
			get
			{
				var entryAssemblies = new string[] { "FuseCP.Server", "FuseCP.EnterpriseServer", "FuseCP.WebPortal", "FuseCP.WebDavPortal" };
				var entryAssembly = AppDomain.CurrentDomain.GetAssemblies()
					.FirstOrDefault(a => entryAssemblies.Any(name => a.GetName().Name == name)) ?? Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
				var fileVersion = entryAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
				return fileVersion == null ? "" : fileVersion.Version;

			}
		}
		public static WindowsVersion WindowsVersion => IsWindowsPlatform() ? WindowsOSInfo.GetVersion() : WindowsVersion.NonWindows;
		public static string Description
		{
			get
			{
				if (IsLinux)
				{
					string name = null;
					const string OsReleaseFile = "/etc/os-release";
					if (File.Exists(OsReleaseFile))
					{
						var osRelease = File.ReadAllText(OsReleaseFile);
						var match = Regex.Match(osRelease, "(?<=^PRETTY_NAME\\s*=\\s*\")[^\"]+(?=\")", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						if (match.Success) name = match.Value;
					}
					if (name == null)
					{
						var osRelease = Shell.Standard.Exec("lsb_release -a").Output().Result;
						var match = Regex.Match(osRelease, "(?<=^Description\\s*:\\s*)[^\\s$]+(?=\\s|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
						if (match.Success) name = match.Value;
					}
					if (name == null)
					{
						name = Shell.Standard.Exec("uname").Output().Result;
					}
					return name;

				}
				else if (IsWindows)
				{
					switch (WindowsVersion)
					{
						case WindowsVersion.WindowsServer2025:
							return "Windows Server 2025";
						case WindowsVersion.WindowsServer2022:
							return "Windows Server 2022";
						case WindowsVersion.Windows11:
							return "Windows 11";
						case WindowsVersion.Windows10:
							return "Windows 10";
						case WindowsVersion.WindowsServer2019:
							return "Windows Server 2019";
						case WindowsVersion.WindowsServer2016:
							return "Windows Server 2016";
						case WindowsVersion.WindowsServer2012R2:
							return "Windows Server 2012 R2";
						case WindowsVersion.WindowsServer2012:
							return "Windows Server 2012";
						case WindowsVersion.Windows8:
							return "Windows 8";
						case WindowsVersion.Windows81:
							return "Windows 8.1";
						case WindowsVersion.Vista:
							return "Windows Vista";
						case WindowsVersion.Windows7:
							return "Windows 7";
						case WindowsVersion.WindowsServer2008R2:
							return "Windows Server 2008 R2";
						case WindowsVersion.WindowsServer2008:
							return "Windows Server 2008";
						case WindowsVersion.WindowsServer2003:
							return "Windows Server 2003";
						case WindowsVersion.WindowsXP:
							return "Windows XP";
						default:
							return RuntimeInformation.OSDescription;
					}
				}
				else
				{
					return RuntimeInformation.OSDescription;
				}
			}
		}

		public static OS.IUnixOperatingSystem Unix => (IUnixOperatingSystem)Current;
		public static IWindowsOperatingSystem Windows => (IWindowsOperatingSystem)Current;

		private static Type ResolveOperatingSystemType(string typeName)
		{
			if (String.IsNullOrWhiteSpace(typeName))
			{
				return null;
			}

			Type type = Type.GetType(typeName, throwOnError: false);
			if (type != null)
			{
				return type;
			}

			var typeParts = typeName.Split(',');
			if (typeParts.Length < 2)
			{
				return null;
			}

			var className = typeParts[0].Trim();
			var assemblySimpleName = typeParts[1].Trim();
			var probeRoots = GetOperatingSystemProbeRoots();

			foreach (var root in probeRoots.Where(Directory.Exists))
			{
				var assemblyPath = Path.Combine(root, assemblySimpleName + ".dll");
				if (!File.Exists(assemblyPath))
				{
					assemblyPath = Directory.EnumerateFiles(root, assemblySimpleName + ".dll", SearchOption.AllDirectories)
						.FirstOrDefault();
				}

				if (String.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
				{
					continue;
				}

				try
				{
					var assembly = Assembly.LoadFrom(assemblyPath);
					type = assembly.GetType(className, throwOnError: false, ignoreCase: false);
					if (type != null)
					{
						return type;
					}
				}
				catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
				{
					System.Diagnostics.Trace.TraceWarning("Could not load OS provider assembly '{0}'. Reason: {1}", assemblyPath, ex.Message);
				}
			}

			return null;
		}

		private static string[] GetOperatingSystemProbeRoots()
		{
			string executingAssemblyDirectory = null;
			try
			{
				executingAssemblyDirectory = Path.GetDirectoryName(typeof(OSInfo).Assembly.Location);
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
			}

			var baseDirectories = new[]
			{
				executingAssemblyDirectory,
				AppContext.BaseDirectory,
				AppDomain.CurrentDomain.BaseDirectory,
				Environment.CurrentDirectory
			}
			.Where(d => !String.IsNullOrWhiteSpace(d))
			.Distinct(StringComparer.OrdinalIgnoreCase);

			return baseDirectories
				.SelectMany(baseDir => new[]
				{
					Path.Combine(baseDir, "bin", "OS"),
					Path.Combine(baseDir, "bin", "Providers", "OS"),
					Path.Combine(baseDir, "bin", "Providers"),
					Path.Combine(baseDir, "OS"),
					Path.Combine(baseDir, "Providers"),
					Path.Combine(baseDir, "DNS"),
					Path.Combine(baseDir, "ProvidersLegacy")
				})
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();
		}

		static Providers.OS.IOperatingSystem CreateOperatingSystem(params string[] typeNames)
		{
			foreach (string typeName in typeNames)
			{
				Type type = ResolveOperatingSystemType(typeName);
				if (type == null)
				{
					continue;
				}

				try
				{
					Providers.OS.IOperatingSystem instance = Activator.CreateInstance(type) as Providers.OS.IOperatingSystem;
					if (instance != null)
					{
						return instance;
					}
				}
				catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
				{
					System.Diagnostics.Trace.TraceWarning("Could not create OS provider '{0}'. Reason: {1}", type.AssemblyQualifiedName, ex.Message);
				}
			}

			return null;
		}

		static Providers.OS.IOperatingSystem os = null;
		public static Providers.OS.IOperatingSystem Current
		{
			get
			{
				if (os == null)
				{
					if (IsWindowsPlatform())
					{
						var local_version = WindowsOSInfo.GetVersion();
						switch (local_version)
						{
							case WindowsVersion.WindowsServer2025:
								os = CreateOperatingSystem(
									"FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025",
									"FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022");
								break;
							case WindowsVersion.WindowsServer2022:
							case WindowsVersion.Windows11:
							case WindowsVersion.Windows10:
							case WindowsVersion.WindowsServer2019:
							case WindowsVersion.WindowsServer2016:
							case WindowsVersion.WindowsServer2012:
							case WindowsVersion.Windows8:
							case WindowsVersion.WindowsServer2012R2:
							case WindowsVersion.Windows81:
							case WindowsVersion.WindowsServer2008:
							case WindowsVersion.WindowsServer2008R2:
							case WindowsVersion.Vista:
							case WindowsVersion.Windows7:
							case WindowsVersion.WindowsServer2003:
							case WindowsVersion.WindowsXP:
							case WindowsVersion.WindowsNT4:
								os = CreateOperatingSystem(
									"FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022",
									"FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025");
								break;
						}

						if (os == null)
						{
							os = CreateOperatingSystem(
								"FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025",
								"FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022");
						}
					}
					else if (IsUnix)
					{
						os = CreateOperatingSystem("FuseCP.Providers.OS.Unix, FuseCP.Providers.OS.Unix");
					}
				}
				return os;
			}
		}
	}
}
