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
using FuseCP.Providers.Virtualization;
using FuseCP.EnterpriseServer;
using System.Web.UI.WebControls;

namespace FuseCP.Portal.Proxmox.RemoteDesktop
{
    public partial class Connect : System.Web.UI.Page
    {
        protected Literal resolution;
        protected Literal username;
        protected Literal password;
        protected Literal serverName;

        protected void Page_Load(object sender, EventArgs e)
        {
            resolution.Text = Request["Resolution"];

            // load server info
            VirtualMachine vm = VirtualMachinesProxmoxHelper.GetCachedVirtualMachine(PanelRequest.ItemID);
            if (vm == null)
            {
                return;
            }

            litServerName.Text = vm.Name + " - ";
            username.Text = "Administrator";
            password.Text = vm.AdministratorPassword;
            
            // load external network parameters
            NetworkAdapterDetails nic = ES.Services.Proxmox.GetExternalNetworkAdapterDetails(PanelRequest.ItemID);
            NetworkAdapterIPAddress[] ipAddresses = nic?.IPAddresses ?? Array.Empty<NetworkAdapterIPAddress>();
            if (ipAddresses.Length > 0)
            {
                NetworkAdapterIPAddress ip = ipAddresses[0];
                serverName.Text = !String.IsNullOrEmpty(ip.NATAddress) ? ip.NATAddress : ip.IPAddress;
            }
        }
    }
}
