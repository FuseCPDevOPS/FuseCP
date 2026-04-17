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
using Microsoft.Win32;

namespace FuseCP.Server.Utils
{
	public class PInvoke
	{
		public static class RegistryHive
		{
			/// <summary>
			/// Implements common methods to manipulate on a registry section's sub keys and their values
			/// </summary>
			public class RegistryHiveSection
			{
				public RegistryHiveSection() { }

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to test the registry hive for a key existence on x86/x64 platforms.
				/// </summary>
				/// <param name="name">Registry key path being tested</param>
				/// <returns></returns>
				private static RegistryView GetRegistryView(bool isX64)
				{
					return isX64 ? RegistryView.Registry64 : RegistryView.Registry32;
				}

				private bool SubKeyExists(string name, bool isX64)
				{
					using var baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, GetRegistryView(isX64));
					using var subKey = baseKey.OpenSubKey(name);
					return subKey != null;
				}

				private string GetSubKeyValue(string keyPath, string keyValue, bool isX64)
				{
					if (SubKeyExists(keyPath, isX64))
					{
						using var baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, GetRegistryView(isX64));
						using var subKey = baseKey.OpenSubKey(keyPath);
						if (subKey != null)
						{
							try
							{
								return subKey.GetValue(keyValue) as string;
							}
							catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
							{
								Log.WriteError(ex);
							}
						}
					}

					return null;
				}

				private int GetDwordSubKeyValue(string keyPath, string keyValue, bool isX64)
				{
					if (SubKeyExists(keyPath, isX64))
					{
						using var baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, GetRegistryView(isX64));
						using var subKey = baseKey.OpenSubKey(keyPath);
						if (subKey != null)
						{
							try
							{
								object value = subKey.GetValue(keyValue);
								if (value == null)
									return 0;

								return Convert.ToInt32(value);
							}
							catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
							{
								Log.WriteError(ex);
							}
						}

						return 0;
					}

					return 0;
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to test the registry for a key existence beneath WOW6432Node on x64 platform (even for 64-bit ASP.NET app pool).
				/// Works seamlessly on x64 platform.
				/// </summary>
				/// <param name="name">Registry key path being tested</param>
				/// <returns></returns>
				public bool SubKeyExists_x86(string name)
				{
					return SubKeyExists(name, false);
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to test the registry for a key existence beneath WOW6464Node on x64 platform (even for 32-bit ASP.NET app pool).
				/// Works seamlessly on x86 platform.
				/// </summary>
				/// <param name="name">Registry key path being tested</param>
				/// <returns></returns>
				public bool SubKeyExists_x64(string name)
				{
					return SubKeyExists(name, true);
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to get registry key value beneath WOW6464Node on x64 platform (even for 32-bit ASP.NET app pool).
				/// Works seamlessly on x86 platform.
				/// </summary>
				/// <param name="keyPath">Registry key path being queried</param>
				/// <param name="keyValue">Registry key value name</param>
				/// <returns></returns>
				public int GetDwordSubKeyValue_x64(string keyPath, string keyValue)
				{
					return GetDwordSubKeyValue(keyPath, keyValue, true);
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to get registry key value beneath WOW6464Node on x64 platform (even for 32-bit ASP.NET app pool).
				/// Works seamlessly on x86 platform.
				/// </summary>
				/// <param name="keyPath">Registry key path being queried</param>
				/// <param name="keyValue">Registry key value name</param>
				/// <returns></returns>
				public string GetSubKeyValue_x64(string keyPath, string keyValue)
				{
					return GetSubKeyValue(keyPath, keyValue, true);
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to get registry key value beneath WOW6432Node section on x64 platform (even for 64-bit ASP.NET app pool).
				/// Works seamlessly on x86 platform.
				/// </summary>
				/// <param name="keyPath">Registry key path being queried</param>
				/// <param name="keyValue">Registry key value name</param>
				/// <returns></returns>
				public string GetSubKeyValue_x86(string keyPath, string keyValue)
				{
					return GetSubKeyValue(keyPath, keyValue, false);
				}

				/// <summary>
				/// Provide seamless dev experience bypassing Registry Redirector feature 
				/// to get registry key value beneath WOW6432Node section on x64 platform (even for 64-bit ASP.NET app pool).
				/// Works seamlessly on x86 platform.
				/// </summary>
				/// <param name="keyPath">Registry key path being queried</param>
				/// <param name="keyValue">Registry key value name</param>
				/// <returns></returns>
				public int GetDwordSubKeyValue_x86(string keyPath, string keyValue)
				{
					return GetDwordSubKeyValue(keyPath, keyValue, false);
				}
			};

			/// <summary>
			/// Represents HKEY_LOCAL_MACHINE section
			/// </summary>
			public static RegistryHiveSection HKLM = new RegistryHiveSection();
		};
	}
}


