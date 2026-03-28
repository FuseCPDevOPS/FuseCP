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
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace FuseCP.Providers.Virtualization.Proxmox
{
	public class ProxmoxVHDHelper
	{

		public static VirtualHardDiskInfo[] Get(String Content)
		{
			if (string.IsNullOrWhiteSpace(Content))
				return Array.Empty<VirtualHardDiskInfo>();

			JObject configvalue;
			try
			{
				JToken jsonResponse = JToken.Parse(Content);
				configvalue = jsonResponse["data"] as JObject;
			}
			catch (JsonException)
			{
				return Array.Empty<VirtualHardDiskInfo>();
			}

			if (configvalue == null)
				return Array.Empty<VirtualHardDiskInfo>();

			List<VirtualHardDiskInfo> disks = new List<VirtualHardDiskInfo>();
			foreach (var property in configvalue)
			{
				string val = (string)property.Value;
				if (string.IsNullOrEmpty(val))
					continue;

				if ((property.Key.Contains("ide") || property.Key.Contains("sata") || property.Key.Contains("virtio") || property.Key.Contains("scsi")) && val.Contains(":"))
				{
					VirtualHardDiskInfo disk = new VirtualHardDiskInfo();
					disk.ControllerNumber = 1;
					disk.ControllerLocation = 1;
					disk.VHDControllerType = property.Key.Contains("ide") || property.Key.Contains("virtio") ? ControllerType.IDE : ControllerType.SCSI;
					disk.Path = parsepath(val);
					disk.Name = property.Key;
					disks.Add(disk);
				}
			}

			return disks.ToArray();
		}


		static String parsepath(String io)
		{
			if (io == null)
				return io;

			String path = "";
			foreach (String ioval in io.Split(','))
			{
				if (ioval.Contains(':'))
					path = ioval;
			}

			return path;
		}
	}
}
