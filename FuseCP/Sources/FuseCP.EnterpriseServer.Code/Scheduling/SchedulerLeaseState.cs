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
using System.Globalization;

namespace FuseCP.EnterpriseServer
{
	internal sealed class SchedulerLeaseState
	{
		private const char Separator = '|';

		public int ScheduleId { get; }
		public string Owner { get; }
		public string RunToken { get; }
		public DateTime HeartbeatUtc { get; }
		public DateTime LeaseUntilUtc { get; }

		public SchedulerLeaseState(int scheduleId, string owner, string runToken, DateTime heartbeatUtc, DateTime leaseUntilUtc)
		{
			ScheduleId = scheduleId;
			Owner = owner ?? string.Empty;
			RunToken = runToken ?? string.Empty;
			HeartbeatUtc = DateTime.SpecifyKind(heartbeatUtc, DateTimeKind.Utc);
			LeaseUntilUtc = DateTime.SpecifyKind(leaseUntilUtc, DateTimeKind.Utc);
		}

		public bool IsOwnedBy(string owner, string runToken)
		{
			return string.Equals(Owner, owner, StringComparison.OrdinalIgnoreCase)
				&& string.Equals(RunToken, runToken, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsExpired(DateTime utcNow)
		{
			return LeaseUntilUtc <= DateTime.SpecifyKind(utcNow, DateTimeKind.Utc);
		}

		public string Serialize()
		{
			return string.Join(Separator,
				Owner,
				RunToken,
				HeartbeatUtc.Ticks.ToString(CultureInfo.InvariantCulture),
				LeaseUntilUtc.Ticks.ToString(CultureInfo.InvariantCulture));
		}

		public static SchedulerLeaseState Parse(int scheduleId, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return null;

			string[] parts = value.Split(Separator);
			if (parts.Length != 4)
				return null;

			if (!long.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out long heartbeatTicks))
				return null;

			if (!long.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out long leaseUntilTicks))
				return null;

			return new SchedulerLeaseState(
				scheduleId,
				parts[0],
				parts[1],
				new DateTime(heartbeatTicks, DateTimeKind.Utc),
				new DateTime(leaseUntilTicks, DateTimeKind.Utc));
		}
	}
}