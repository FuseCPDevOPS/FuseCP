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

#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace FuseCP.Server
{
	public static class Program
	{
		private static bool providerResolverConfigured;
		private static Dictionary<string, string> providerAssemblyMap;

		public static void Main(string[] args)
		{
			//if (!Debugger.IsAttached) Debugger.Launch();
			ConfigureProviderAssemblyResolver();
			PasswordValidator.Init();
			FuseCP.Web.Services.StartupCore.Init(args);
		}

		private static void ConfigureProviderAssemblyResolver()
		{
			if (providerResolverConfigured)
				return;

			providerResolverConfigured = true;
			providerAssemblyMap = BuildProviderAssemblyMap();

			AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
			{
				if (assemblyName == null || string.IsNullOrEmpty(assemblyName.Name))
					return null;

				if (assemblyName.Name.StartsWith("FuseCP.", StringComparison.OrdinalIgnoreCase))
				{
					if (providerAssemblyMap.TryGetValue(assemblyName.Name, out var assemblyPath) && File.Exists(assemblyPath))
						return context.LoadFromAssemblyPath(assemblyPath);

					return null;
				}

				// Allow specific provider dependencies that are not FuseCP.* but are shipped in provider folders.
				if (assemblyName.Name.Equals("Microsoft.Management.Infrastructure", StringComparison.OrdinalIgnoreCase)
					|| assemblyName.Name.Equals("Microsoft.Management.Infrastructure.Native", StringComparison.OrdinalIgnoreCase)
					|| assemblyName.Name.Equals("System.ServiceProcess.ServiceController", StringComparison.OrdinalIgnoreCase))
				{
					var providerDependencyPath = ResolveDependencyFromProviderRoots(assemblyName.Name);
					if (!string.IsNullOrEmpty(providerDependencyPath) && File.Exists(providerDependencyPath))
						return context.LoadFromAssemblyPath(providerDependencyPath);
				}

				return null;
			};
		}

		private static string ResolveDependencyFromProviderRoots(string assemblySimpleName)
		{
			foreach (var providerRoot in GetProviderProbeRoots().Where(Directory.Exists))
			{
				var directPath = Path.Combine(providerRoot, assemblySimpleName + ".dll");
				if (File.Exists(directPath))
					return directPath;

				var nestedPath = Directory.EnumerateFiles(providerRoot, assemblySimpleName + ".dll", SearchOption.AllDirectories)
					.FirstOrDefault();
				if (!string.IsNullOrEmpty(nestedPath))
					return nestedPath;
			}

			return null;
		}

		private static Dictionary<string, string> BuildProviderAssemblyMap()
		{
			var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			var versionMap = new Dictionary<string, Version>(StringComparer.OrdinalIgnoreCase);
			foreach (var providerRoot in GetProviderProbeRoots().Where(Directory.Exists))
			{
				foreach (var file in Directory.EnumerateFiles(providerRoot, "*.dll", SearchOption.AllDirectories))
				{
					var name = Path.GetFileNameWithoutExtension(file);
					if (!name.StartsWith("FuseCP.", StringComparison.OrdinalIgnoreCase))
						continue;

					Version version;
					try
					{
						version = AssemblyName.GetAssemblyName(file).Version ?? new Version(0, 0, 0, 0);
					}
					catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
					{
						continue;
					}

					if (!map.TryGetValue(name, out var existingPath))
					{
						map[name] = file;
						versionMap[name] = version;
						continue;
					}

					if (!versionMap.TryGetValue(name, out var existingVersion))
						existingVersion = new Version(0, 0, 0, 0);

					if (version > existingVersion)
					{
						map[name] = file;
						versionMap[name] = version;
					}
				}
			}

			return map;
		}

		private static IEnumerable<string> GetProviderProbeRoots()
		{
			var baseDir = AppContext.BaseDirectory;
			yield return Path.Join(baseDir, "bin", "Providers");
			yield return Path.Join(baseDir, "bin", "OS");
			yield return Path.Join(baseDir, "bin", "DNS");
			yield return Path.Join(baseDir, "bin", "Providers", "OS");
			yield return Path.Join(baseDir, "Providers");
			yield return Path.Join(baseDir, "OS");
			yield return Path.Join(baseDir, "DNS");
			yield return Path.Join(baseDir, "ProvidersLegacy");
			yield return Path.Join(baseDir, "netstandard");
		}
	}
}

#endif
