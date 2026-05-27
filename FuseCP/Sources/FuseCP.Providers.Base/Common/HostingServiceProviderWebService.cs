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
using System.IO;
using System.Linq;
using System.Reflection;

namespace FuseCP.Providers
{
    public abstract class HostingServiceProviderWebService: IDisposable
    {
        static HostingServiceProviderWebService()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromRequestingAssemblyDirectory;
        }

        public ServiceProviderSettingsSoapHeader settings = new ServiceProviderSettingsSoapHeader();

        private RemoteServerSettings serverSettings;
        private ServiceProviderSettings providerSettings;

        private IHostingServiceProvider provider;
        protected virtual IHostingServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    // try to create provider class
                    Type providerType = ResolveProviderType(ProviderSettings.ProviderType);
                    try
                    {
                        provider = CreateProviderInstance(providerType, ProviderSettings.ProviderType);

                        ((HostingServiceProviderBase)provider).ServerSettings = ServerSettings;
                        ((HostingServiceProviderBase)provider).ProviderSettings = ProviderSettings;
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        var root = ex.GetBaseException();
                        throw new Exception(String.Format("Can not create '{0}' provider instance with '{1}' type",
                            ProviderSettings.ProviderName, ProviderSettings.ProviderType)
                            + String.Format(". Root cause: {0}: {1}", root.GetType().FullName, root.Message), ex);
                    }
                }
                return provider;
            }
        }

        private IHostingServiceProvider CreateProviderInstance(Type providerType, string providerTypeName)
        {
            try
            {
                return (IHostingServiceProvider)Activator.CreateInstance(providerType);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                var platformEx = ex as PlatformNotSupportedException ?? ex.GetBaseException() as PlatformNotSupportedException;
                if (!ShouldFallbackToLegacyDnsProvider(providerTypeName, platformEx))
                    throw;

                var fallbackTypeName = "FuseCP.Providers.DNS.MsDNS, FuseCP.Providers.DNS.MsDNS";
                var fallbackType = ResolveProviderType(fallbackTypeName);
                if (fallbackType == null)
                    throw;

                return (IHostingServiceProvider)Activator.CreateInstance(fallbackType);
            }
        }

        private static Type ResolveProviderType(string providerTypeName)
        {
            if (string.IsNullOrWhiteSpace(providerTypeName))
                return null;

            // First try default type resolution.
            var providerType = Type.GetType(providerTypeName, throwOnError: false);
            if (providerType != null)
                return providerType;

            var typeParts = providerTypeName.Split(',');
            if (typeParts.Length < 2)
                return null;

            var className = typeParts[0].Trim();
            var assemblySimpleName = typeParts[1].Trim();

            // Attempt explicit assembly loading from known provider probe folders.
            foreach (var root in GetProviderProbeRoots().Where(Directory.Exists))
            {
                var assemblyPath = Path.Combine(root, assemblySimpleName + ".dll");
                if (!File.Exists(assemblyPath))
                    continue;

                try
                {
                    var assembly = System.Reflection.Assembly.LoadFrom(assemblyPath);
                    providerType = assembly.GetType(className, throwOnError: false, ignoreCase: false);
                    if (providerType != null)
                        return providerType;
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    // Keep probing other locations.
                }
            }

            return null;
        }

        private static string[] GetProviderProbeRoots()
        {
            var baseDir = AppContext.BaseDirectory;
            return new[]
            {
                Path.Combine(baseDir, "Providers"),
                Path.Combine(baseDir, "DNS"),
                Path.Combine(baseDir, "ProvidersLegacy"),
                Path.GetFullPath(Path.Combine(baseDir, "..", "bin", "Providers")),
                Path.GetFullPath(Path.Combine(baseDir, "..", "bin", "DNS")),
                Path.GetFullPath(Path.Combine(baseDir, "..", "bin", "ProvidersLegacy")),
                Path.Combine(baseDir, "netstandard"),
                Path.GetFullPath(Path.Combine(baseDir, "..", "bin", "netstandard"))
            };
        }

        private static bool ShouldFallbackToLegacyDnsProvider(string providerTypeName, PlatformNotSupportedException ex)
        {
            if (ex == null)
                return false;

            if (!string.Equals(providerTypeName, "FuseCP.Providers.DNS.MsDNSPS, FuseCP.Providers.DNS.MsDNSPS", StringComparison.OrdinalIgnoreCase))
                return false;

            return ex.Message?.IndexOf("ReflectionOnly loading is not supported on this platform", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static Assembly ResolveFromRequestingAssemblyDirectory(object sender, ResolveEventArgs args)
        {
            if (args?.RequestingAssembly == null)
                return null;

            string requestingAssemblyPath;
            try
            {
                requestingAssemblyPath = args.RequestingAssembly.Location;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(requestingAssemblyPath))
                return null;

            string directory = Path.GetDirectoryName(requestingAssemblyPath);
            if (string.IsNullOrWhiteSpace(directory))
                return null;

            string assemblySimpleName;
            try
            {
                assemblySimpleName = new AssemblyName(args.Name).Name;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(assemblySimpleName))
                return null;

            string candidatePath = Path.Combine(directory, assemblySimpleName + ".dll");
            if (!File.Exists(candidatePath))
                return null;

            try
            {
                return Assembly.LoadFrom(candidatePath);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return null;
            }
        }

        public RemoteServerSettings ServerSettings
        {
            get
            {
                if (serverSettings == null)
                {
                    // parse server settings
                    serverSettings = new RemoteServerSettings(settings.Settings);
                }
                return serverSettings;
            }
            set => serverSettings = value;
        }

        public ServiceProviderSettings ProviderSettings
        {
            get
            {
                if (providerSettings == null)
                {
                    // parse provider settings
                    providerSettings = new ServiceProviderSettings(settings.Settings);
                }
                return providerSettings;
            }
            set { providerSettings = value; }
        }

        public void Dispose()
        {
            if (Provider is IDisposable disposableProvider) disposableProvider.Dispose();    
        }
    }
}



