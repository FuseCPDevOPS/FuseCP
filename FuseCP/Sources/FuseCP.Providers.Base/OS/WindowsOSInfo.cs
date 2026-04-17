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
using System.Runtime.InteropServices;
#if !NETSTANDARD2_0
using System.Runtime.Versioning;
#endif
using System.Security.Principal;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace FuseCP.Providers.OS
{
    public enum WindowsVersion
    {
        Unknown = 0,
        NonWindows,
        Windows95,
        Windows98,
        WindowsMe,
        WindowsNT351,
        WindowsNT4,
        Windows2000,
        WindowsXP,
        WindowsServer2003,
        Vista,
        WindowsServer2008,
        Windows7,
        WindowsServer2008R2,
        Windows8,
        WindowsServer2012,
        Windows81,
        WindowsServer2012R2,
        WindowsServer2016,
        Windows10,
        WindowsServer2019,
        Windows11,
        WindowsServer2022,
        WindowsServer2025
    }

#if !NETSTANDARD2_0
    [SupportedOSPlatform("windows")]
#endif
    public sealed class WindowsOSInfo
    {
        // Legacy interop-based OS detection was removed in favor of managed-only APIs.

        public static int GetCurrentBuild()
        {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            var currentBuildStr = reg?.GetValue("CurrentBuild") as string;
            return int.Parse(currentBuildStr ?? "0");
        }

        /// <summary>
        /// Determine OS version
        /// </summary>
        /// <returns></returns>
        static WindowsVersion? version;
        public static WindowsVersion GetVersion()
        {
            if (version.HasValue) return version.Value;

            if (!OSInfo.IsWindows)
            {
                version = WindowsVersion.NonWindows;
                return WindowsVersion.NonWindows;
            }

            WindowsVersion ret = WindowsVersion.Unknown;
            System.OperatingSystem osInfo = System.Environment.OSVersion;

            Version ver = osInfo.Version;
            string osDesc = RuntimeInformation.OSDescription;
            var match = Regex.Match(osDesc, @"[0-9]+(?:\.[0-9]+){0,3}");
            if (match.Success) ver = new Version(match.Value);

            bool isServer = osDesc.IndexOf("server", StringComparison.OrdinalIgnoreCase) >= 0;

            switch (osInfo.Platform)
            {
                case System.PlatformID.Win32Windows:
                    switch (osInfo.Version.Minor)
                    {
                        case 0:
                            ret = WindowsVersion.Windows95;
                            break;
                        case 10:
                            ret = WindowsVersion.Windows98;
                            break;
                        case 90:
                            ret = WindowsVersion.WindowsMe;
                            break;
                    }
                    break;

                case System.PlatformID.Win32NT:
                    switch (ver.Major)
                    {
                        case 3:
                            ret = WindowsVersion.WindowsNT351;
                            break;
                        case 4:
                            ret = WindowsVersion.WindowsNT4;
                            break;
                        case 5:
                            switch (ver.Minor)
                            {
                                case 0:
                                    ret = WindowsVersion.Windows2000;
                                    break;
                                case 1:
                                    ret = WindowsVersion.WindowsXP;
                                    break;
                                case 2:
                                    ret = isServer ? WindowsVersion.WindowsServer2003 : WindowsVersion.WindowsXP;
                                    break;
                            }
                            break;
                        case 6:
                            switch (ver.Minor)
                            {
                                case 0:
                                    ret = isServer ? WindowsVersion.WindowsServer2008 : WindowsVersion.Vista;
                                    break;
                                case 1:
                                    ret = isServer ? WindowsVersion.WindowsServer2008R2 : WindowsVersion.Windows7;
                                    break;
                                case 2:
                                    ret = isServer ? WindowsVersion.WindowsServer2012 : WindowsVersion.Windows8;
                                    break;
                                case 3:
                                    ret = isServer ? WindowsVersion.WindowsServer2012R2 : WindowsVersion.Windows81;
                                    break;
                            }
                            break;
                        case 10:
                            int releaseId = GetReleaseId();
                            if ((releaseId == 1607 || releaseId == 1709 || releaseId == 1803) && isServer)
                            {
                                ret = WindowsVersion.WindowsServer2016;
                            }
                            else if (releaseId == 1507 || releaseId == 1511 || releaseId == 1607 || releaseId == 1703 || releaseId == 1709 || releaseId == 1803)
                            {
                                ret = WindowsVersion.Windows10;
                            }
                            else
                            {
                                int currentBuild = GetCurrentBuild();
                                if (currentBuild >= 22000 && !isServer)
                                {
                                    ret = WindowsVersion.Windows11;
                                }
                                else if (currentBuild >= 20348 && isServer)
                                {
                                    ret = currentBuild >= 26000 ? WindowsVersion.WindowsServer2025 : WindowsVersion.WindowsServer2022;
                                }
                                else
                                {
                                    ret = isServer ? WindowsVersion.WindowsServer2019 : WindowsVersion.Windows10;
                                }
                            }
                            break;
                    }
                    break;
            }

            version = ret;
            return ret;
        }

        public static bool IsWindows11()
        {
            try
            {
                var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

                var currentBuildStr = (string)reg.GetValue("CurrentBuild");
                var currentBuild = int.Parse(currentBuildStr);

                return currentBuild >= 22000;
            }
            catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
            return false;
        }
        public static int GetReleaseId()
        {
            return Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "0"));
        }

        /// <summary>
        /// Returns Windows directory
        /// </summary>
        /// <returns></returns>
        public static string GetWindowsDirectory()
        {
            return Environment.GetEnvironmentVariable("windir");
        }
        public static string NetFXVersion
        {
            get
            {
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    return ndpKey != null && ndpKey.GetValue("Release") != null ? CheckFor45PlusVersion((int)ndpKey.GetValue("Release")) : $"{Environment.Version.Major}.{Environment.Version.Minor}";







                }
            }
        }
        // Checking the version using >= enables forward compatibility.
        static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 533320)
                return "4.8.1";
            if (releaseKey >= 528040)
                return "4.8";
            if (releaseKey >= 461808)
                return "4.7.2";
            if (releaseKey >= 461308)
                return "4.7.1";
            if (releaseKey >= 460798)
                return "4.7";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
                return "4.6.1";
            if (releaseKey >= 393295)
                return "4.6";
            if (releaseKey >= 379893)
                return "4.5.2";
            if (releaseKey >= 378675)
                return "4.5.1";
            if (releaseKey >= 378389)
                return "4.5";
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "4.0";
        }
    }
}



