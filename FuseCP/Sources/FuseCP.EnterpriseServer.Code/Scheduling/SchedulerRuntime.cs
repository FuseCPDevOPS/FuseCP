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

namespace FuseCP.EnterpriseServer
{
	internal static class SchedulerRuntime
	{
		public static bool IsStaleStartingTask(BackgroundTask task)
		{
			return task != null
				&& task.Status == BackgroundTaskStatus.Starting
				&& (DateTime.Now - task.StartDate).TotalSeconds > 180;
		}

		public static string GetLeaseOwner()
		{
			return Environment.MachineName;
		}

		public static TimeSpan GetLeaseDuration(int? maxExecutionTime)
		{
			int leaseSeconds = maxExecutionTime.HasValue && maxExecutionTime.Value > 0
				? maxExecutionTime.Value + 120
				: 300;

			leaseSeconds = Math.Max(300, Math.Min(leaseSeconds, 3600));
			return TimeSpan.FromSeconds(leaseSeconds);
		}
	}
}