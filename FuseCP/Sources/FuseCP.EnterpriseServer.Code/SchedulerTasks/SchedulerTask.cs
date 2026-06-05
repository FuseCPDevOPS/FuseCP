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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuseCP.EnterpriseServer
{
	public abstract class SchedulerTask: ControllerAsyncBase, ISchedulerTask
	{
		public abstract void DoWork();

		private static readonly string[] RoutedServerParameterNames =
		{
			"SCHEDULER_TARGET_SERVER_ID",
			"SERVER_ID",
			"SCHEDULER_TARGET_AFFINITY",
			"SCHEDULER_AFFINITY"
		};

		protected bool TryGetRoutedServerId(BackgroundTask task, out int serverId)
		{
			serverId = 0;

			if (task == null)
				return false;

			foreach (string parameterName in RoutedServerParameterNames)
			{
				string raw = Convert.ToString(task.GetParamValue(parameterName), CultureInfo.InvariantCulture);
				if (!TryParseServerId(raw, out int parsedServerId))
					continue;

				serverId = parsedServerId;
				return true;
			}

			return false;
		}

		protected bool TryResolveTargetServer(BackgroundTask task, string fallbackServerName, out ServerInfo server)
		{
			server = null;

			if (TryGetRoutedServerId(task, out int routedServerId))
			{
				server = ServerController.GetServerById(routedServerId, false);
				if (server != null)
					return true;
			}

			if (!String.IsNullOrWhiteSpace(fallbackServerName))
			{
				server = ServerController.GetServerByName(fallbackServerName);
				if (server != null)
					return true;
			}

			if (task != null && task.PackageId > 0)
			{
				PackageInfo package = PackageController.GetPackage(task.PackageId);
				if (package != null && package.ServerId > 0)
				{
					server = ServerController.GetServerById(package.ServerId, false);
					if (server != null)
						return true;
				}
			}

			return false;
		}

		private static bool TryParseServerId(string raw, out int serverId)
		{
			serverId = 0;
			if (String.IsNullOrWhiteSpace(raw))
				return false;

			string value = raw.Trim();
			const string serverPrefix = "server:";
			if (value.StartsWith(serverPrefix, StringComparison.OrdinalIgnoreCase))
				value = value.Substring(serverPrefix.Length).Trim();

			if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
				return false;

			if (parsed <= 0)
				return false;

			serverId = parsed;
			return true;
		}

	}
}
