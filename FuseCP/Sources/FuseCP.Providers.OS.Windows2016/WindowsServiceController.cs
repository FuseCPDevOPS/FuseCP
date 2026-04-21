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
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace FuseCP.Providers.OS
{
	public class WindowsServiceController : ServiceController
	{
		private static readonly Regex ServiceIdPattern = new Regex(@"^[A-Za-z0-9._\-]+$", RegexOptions.Compiled);

		private static void RunProcess(string fileName, params string[] arguments)
		{
			using (var process = new Process())
			{
				process.StartInfo = new ProcessStartInfo
				{
					FileName = fileName,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				foreach (var argument in arguments)
				{
					process.StartInfo.ArgumentList.Add(argument);
				}

				if (!process.Start())
				{
					throw new InvalidOperationException($"Failed to start process '{fileName}'.");
				}

				process.WaitForExit();
				if (process.ExitCode != 0)
				{
					var error = process.StandardError.ReadToEnd();
					throw new InvalidOperationException($"Process '{fileName}' failed with exit code {process.ExitCode}: {error}");
				}
			}
		}

		private static string ValidateServiceId(string serviceId)
		{
			if (string.IsNullOrWhiteSpace(serviceId))
			{
				throw new ArgumentException("Service identifier cannot be empty.", nameof(serviceId));
			}

			if (!ServiceIdPattern.IsMatch(serviceId))
			{
				throw new ArgumentException("Service identifier contains invalid characters.", nameof(serviceId));
			}

			return serviceId;
		}

		private static bool IsManagedServiceId(string serviceId)
		{
			if (string.IsNullOrWhiteSpace(serviceId))
			{
				return false;
			}

			return serviceId.StartsWith("fusecp.", StringComparison.OrdinalIgnoreCase)
				|| serviceId.StartsWith("fusecp-", StringComparison.OrdinalIgnoreCase);
		}

		public override bool IsInstalled => true;

		public override IEnumerable<OSService> All() => OSInfo.Windows.GetOSServices();

		public override void ChangeStatus(string serviceId, OSServiceStatus status)
		{
			serviceId = ValidateServiceId(serviceId);
			if (!IsManagedServiceId(serviceId))
			{
				throw new UnauthorizedAccessException("Changing unmanaged system services is not permitted.");
			}

			var service = All().FirstOrDefault(s => string.Equals(s.Id, serviceId, StringComparison.Ordinal));
			if (service == null)
			{
				throw new ArgumentException("Service not found.", nameof(serviceId));
			}

			if (!IsManagedServiceId(service.Id))
			{
				throw new UnauthorizedAccessException("Resolved service is not managed by FuseCP.");
			}

			OSInfo.Windows.ChangeOSServiceStatus(service.Id, status);
		}

		public override OSService Info(string serviceId)
			=> All().FirstOrDefault(s => string.Equals(s.Id, ValidateServiceId(serviceId), StringComparison.Ordinal));

		public override ServiceManager Install(ServiceDescription service)
		{
			if (service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			var serviceId = ValidateServiceId(service.ServiceId);
			if (string.IsNullOrWhiteSpace(service.Executable))
			{
				throw new ArgumentException("Executable path cannot be empty.", nameof(service));
			}

			var winService = service as WindowsServiceDescription;
			var arguments = new List<string> { "create", serviceId, "binPath=", service.Executable };
			if (winService != null)
			{
				if (!string.IsNullOrEmpty(winService.DisplayName))
				{
					arguments.Add("DisplayName=");
					arguments.Add(winService.DisplayName);
				}
				if (winService.DependsOn != null && winService.DependsOn.Any())
				{
					arguments.Add("depend=");
					arguments.Add(string.Join("/", winService.DependsOn.Select(dep => dep.Trim())));
				}
				if (!string.IsNullOrEmpty(winService.Object))
				{
					arguments.Add("obj=");
					arguments.Add(winService.Object);
				}
				if (winService.Type != WindowsServiceType.Own)
				{
					arguments.Add("type=");
					arguments.Add(winService.Type.ToString().ToLowerInvariant());
				}
				if (winService.Error != WindowsServiceErrorHandling.Normal)
				{
					arguments.Add("error=");
					arguments.Add(winService.Error.ToString().ToLowerInvariant());
				}
				if (winService.Start != WindowsServiceStartMode.Demand)
				{
					arguments.Add("start=");
					arguments.Add(winService.Start.ToString().ToLowerInvariant());
				}
				if (winService.Tag.HasValue)
				{
					arguments.Add("tag=");
					arguments.Add(winService.Tag.Value ? "yes" : "no");
				}
				if (!string.IsNullOrEmpty(winService.Group))
				{
					arguments.Add("group=");
					arguments.Add(winService.Group);
				}
				if (!string.IsNullOrEmpty(winService.Password))
				{
					arguments.Add("password=");
					arguments.Add(winService.Password);
				}
			}

			RunProcess("sc.exe", arguments.ToArray());

			return new ServiceManager(this, serviceId);
		}

		public override void Remove(string serviceId)
		{
			serviceId = ValidateServiceId(serviceId);
			var service = All().FirstOrDefault(s => string.Equals(s.Id, serviceId, StringComparison.Ordinal));
			if (service == null)
			{
				throw new ArgumentException("Service not found.", nameof(serviceId));
			}
			RunProcess("sc.exe", "delete", service.Id);
		}

		public override void SystemReboot() => RunProcess("shutdown.exe", "/r", "/f", "/t", "0");
	}
}
