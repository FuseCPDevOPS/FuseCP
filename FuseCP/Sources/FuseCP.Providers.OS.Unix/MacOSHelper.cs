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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace FuseCP.Providers.OS
{
	public static class MacOSHelper
	{
		const int CPU_STATE_USER = 0;
		const int CPU_STATE_SYSTEM = 1;
		const int CPU_STATE_IDLE = 2;
		const int CPU_STATE_NICE = 3;

		public static short GetProcessorTotalProcessorTimeMac()
		{
			var (idle1, total1) = GetCpuStatsMac();
			Thread.Sleep(1000);
			var (idle2, total2) = GetCpuStatsMac();

			int idleDiff = (int)idle2 - (int)idle1;
			int totalDiff = (int)total2 - (int)total1;

			int idlePercent = 100 * idleDiff / totalDiff;

			return (short)(100 - idlePercent);
		}

		static (ulong idleTicks, ulong totalTicks) GetCpuStatsMac()
		{
			ulong[] ticks = ReadKernelCpuTicks();
			if (ticks.Length <= CPU_STATE_NICE)
				throw new Exception("Unexpected kern.cp_time format.");

			ulong user = ticks[CPU_STATE_USER];
			ulong system = ticks[CPU_STATE_SYSTEM];
			ulong idle = ticks[CPU_STATE_IDLE];
			ulong nice = ticks[CPU_STATE_NICE];

			ulong total = user + system + idle + nice;

			return (idle, total);
		}

		static ulong[] ReadKernelCpuTicks()
		{
			using var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "sysctl",
					Arguments = "-n kern.cp_time",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};

			proc.Start();
			string output = proc.StandardOutput.ReadToEnd();
			string error = proc.StandardError.ReadToEnd();
			proc.WaitForExit();

			if (proc.ExitCode != 0)
				throw new Exception($"sysctl kern.cp_time failed: {error}");

			return output
				.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
				.Select(v => ulong.Parse(v, CultureInfo.InvariantCulture))
				.ToArray();
		}
	}
}
