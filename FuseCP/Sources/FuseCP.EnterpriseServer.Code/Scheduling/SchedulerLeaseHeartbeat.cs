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
using System.Threading;

namespace FuseCP.EnterpriseServer
{
	internal sealed class SchedulerLeaseHeartbeat : IDisposable
	{
		private readonly SchedulerController schedulerController;
		private readonly int scheduleId;
		private readonly string owner;
		private readonly string runToken;
		private readonly TimeSpan leaseDuration;
		private readonly Timer timer;
		private int disposed;

		public SchedulerLeaseHeartbeat(SchedulerController schedulerController, int scheduleId, string owner, string runToken, TimeSpan leaseDuration)
		{
			this.schedulerController = schedulerController;
			this.scheduleId = scheduleId;
			this.owner = owner;
			this.runToken = runToken;
			this.leaseDuration = leaseDuration;

			var interval = TimeSpan.FromSeconds(Math.Max(15, Math.Min(leaseDuration.TotalSeconds / 2, 60)));
			timer = new Timer(_ => Renew(), null, interval, interval);
			Renew();
		}

		private void Renew()
		{
			if (Volatile.Read(ref disposed) != 0)
				return;

			try
			{
				schedulerController.RenewScheduleLease(scheduleId, owner, runToken, leaseDuration);
			}
			catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
			{
				System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
			}
		}

		public void Dispose()
		{
			if (Interlocked.Exchange(ref disposed, 1) != 0)
				return;

			timer.Dispose();

			try
			{
				schedulerController.ReleaseScheduleLease(scheduleId, owner, runToken);
			}
			catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
			{
				System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
			}
		}
	}
}