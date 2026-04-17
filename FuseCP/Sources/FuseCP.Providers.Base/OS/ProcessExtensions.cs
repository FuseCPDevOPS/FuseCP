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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace FuseCP.Providers.OS
{
	public static class ProcessExtensions
	{
		public static string ExecutableFile(this Process process)
		{
			if (OSInfo.IsWindows) return process.MainModule.FileName;
			else if (OSInfo.IsMac) return process.MainModule?.FileName;
			else
			{
				var procexe = $"/proc/{process.Id}/exe";
				if ((OSInfo.IsLinux || File.Exists(procexe)) && OSInfo.IsCore)
				{
					var m = typeof(Directory).GetMethod("ResolveLinkTarget", BindingFlags.Static | BindingFlags.Public);
					if (m != null) {
						try
						{
							var info = m.Invoke(null, new object[] { procexe, true }) as FileSystemInfo;
							if (info != null) return info.FullName;
						}
						catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException)) { System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message); }
					}
				}
				else if (Shell.Default.Find("ps") != null)
				{
					var psout = Shell.Default.Exec("ps -ef").Output().Result;
					var match = Regex.Match(psout, @$"(?<=^\s*[^ \t]*\s+{process.Id}(\s+[^ \t]+){5}\s+)[^ \t]+", RegexOptions.Multiline);
					if (match.Success) return match.Value;
				}
				return null;
			}
		}
	}
}


