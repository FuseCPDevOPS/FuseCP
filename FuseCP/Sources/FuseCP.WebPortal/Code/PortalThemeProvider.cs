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
using System.Text;
using System.Configuration;
using System.Linq;

namespace FuseCP.WebPortal
{
	public abstract class PortalThemeProvider
	{
		private const string ThemeProviderKey = "WebPortal.ThemeProvider";
		private const string DefaultThemeProviderType = "FuseCP.Portal.WebPortalThemeProvider, FuseCP.Portal.Modules";
		private static readonly object InstanceLock = new object();
		private static PortalThemeProvider _instance;
		public static PortalThemeProvider Instance
		{
			get
			{
				if (_instance != null)
					return _instance;

				lock (InstanceLock)
				{
					if (_instance != null)
						return _instance;

					string configuredTypeName = ConfigurationManager.AppSettings[ThemeProviderKey];
					if (string.IsNullOrWhiteSpace(configuredTypeName))
						configuredTypeName = DefaultThemeProviderType;

					try
					{
						Type providerType = ResolveProviderType(configuredTypeName);
						if (providerType == null && !configuredTypeName.Equals(DefaultThemeProviderType, StringComparison.OrdinalIgnoreCase))
							providerType = ResolveProviderType(DefaultThemeProviderType);

						if (providerType == null)
							throw new InvalidOperationException(string.Format("Could not resolve theme provider type '{0}'.", configuredTypeName));

						if (!typeof(PortalThemeProvider).IsAssignableFrom(providerType))
							throw new InvalidOperationException(string.Format("Configured theme provider type '{0}' does not inherit from PortalThemeProvider.", providerType.FullName));

						_instance = (PortalThemeProvider)Activator.CreateInstance(providerType);
					}
					catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
					{
						throw new Exception(string.Format("Could not create '{0}' theme provider", configuredTypeName), ex);
					}
				}

				return _instance;
			}
		}

		private static Type ResolveProviderType(string configuredTypeName)
		{
			Type providerType = Type.GetType(configuredTypeName, false);
			if (providerType != null)
				return providerType;

			string shortTypeName = configuredTypeName.Split(',')[0].Trim();
			providerType = AppDomain.CurrentDomain
				.GetAssemblies()
				.Select(a => a.GetType(shortTypeName, false, true))
				.FirstOrDefault(t => t != null);

			if (providerType != null)
				return providerType;

			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Select(a => a.GetType(configuredTypeName, false, true))
				.FirstOrDefault(t => t != null);
		}

		public abstract string GetTheme();
	}
}
