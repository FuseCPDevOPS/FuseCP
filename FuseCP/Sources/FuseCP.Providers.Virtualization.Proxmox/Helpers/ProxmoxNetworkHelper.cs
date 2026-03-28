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
	public class ProxmoxNetworkHelper
	{
		private const int defaultvlan = 0;

		public static VirtualMachineNetworkAdapter[] Get(String Content)
		{
			if (string.IsNullOrWhiteSpace(Content))
				return Array.Empty<VirtualMachineNetworkAdapter>();

			JObject configvalue;
			try
			{
				JToken jsonResponse = JToken.Parse(Content);
				configvalue = jsonResponse["data"] as JObject;
			}
			catch (JsonException)
			{
				return Array.Empty<VirtualMachineNetworkAdapter>();
			}

			if (configvalue == null)
				return Array.Empty<VirtualMachineNetworkAdapter>();

			List<VirtualMachineNetworkAdapter> adapters = new List<VirtualMachineNetworkAdapter>();
			foreach (var property in configvalue)
			{
				string val = (string)property.Value;
				if (property.Key.Contains("net"))
				{
					VirtualMachineNetworkAdapter adapter = CreateAdapter(val);
					if (adapter == null)
						continue;

					adapter.Name = String.Format("{0} {1} VLAN {2}", property.Key, adapter.Name, adapter.vlan);
					adapters.Add(adapter);
				}
			}

			return adapters.ToArray();
		}

		private static VirtualMachineNetworkAdapter CreateAdapter(String adapterinfo)
		{
			if (string.IsNullOrEmpty(adapterinfo))
				return null;

			VirtualMachineNetworkAdapter adapter = new VirtualMachineNetworkAdapter();
			adapter.vlan = defaultvlan;
			Array adapterarray = adapterinfo.Split(',');
			foreach (String adapterval in adapterarray)
			{
				if (adapterval.Contains(":"))
				{
					var parts = adapterval.Split('=');
					if (parts.Length > 1)
					{
						adapter.MacAddress = parts[1].Replace(":", "");
						adapter.Name = parts[0];
					}
				}
				else if (adapterval.Contains("bridge"))
				{
					var parts = adapterval.Split('=');
					if (parts.Length > 1)
						adapter.SwitchName = parts[1];
				}
				else if (adapterval.Contains("tag"))
				{
					var parts = adapterval.Split('=');
					if (parts.Length > 1 && Int32.TryParse(parts[1], out var vlan))
						adapter.vlan = vlan;
				}
			}

			return adapter;
		}
	}
}
