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

﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.Providers.Common;
using FuseCP.Providers.Virtualization;
using FuseCP.EnterpriseServer;
using System.Linq;

namespace FuseCP.Portal.VPS2012
{
    public partial class VpsDetailsAddExternalAddress : FuseCPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ToggleControls();

            if (!IsPostBack)
            {
                BindExternalIPAddresses();
            }
        }

        private void BindExternalIPAddresses()
        {
            int adaptervlan = ES.Services.VPS2012.GetExternalNetworkVLAN(PanelRequest.ItemID);

            PackageIPAddress[] ips = ES.Services.Servers.GetPackageUnassignedIPAddresses(PanelSecurity.PackageId, 0, IPAddressPool.VpsExternalNetwork) ?? Array.Empty<PackageIPAddress>();
            foreach (PackageIPAddress ip in ips.Where(ip => ip.VLAN == adaptervlan))
            {
                    string txt = ip.ExternalIP;
                    if (!String.IsNullOrEmpty(ip.DefaultGateway))
                        txt += "/" + ip.DefaultGateway;
                    listExternalAddresses.Items.Add(new ListItem(txt, ip.PackageAddressID.ToString()));
            }

            // toggle controls
            int maxAddresses = listExternalAddresses.Items.Count;
            litMaxExternalAddresses.Text = String.Format(GetLocalizedString("litMaxExternalAddresses.Text"), maxAddresses);

            bool empty = maxAddresses == 0;
            EmptyExternalAddressesMessage.Visible = empty;
            ExternalAddressesTable.Visible = !empty;
            btnAdd.Enabled = btnAddByInject.Enabled = !empty;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectBack();
        }

        private void RedirectBack()
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "vps_network",
                "SpaceID=" + PanelSecurity.PackageId));
        }

        private void ToggleControls()
        {
            // external network
            ExternalAddressesNumberRow.Visible = radioExternalRandom.Checked;
            ExternalAddressesListRow.Visible = radioExternalSelected.Checked;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            AddIP(sender, e, false);
        }
        protected void btnAddByInject_Click(object sender, EventArgs e)
        {
            AddIP(sender, e, true);
        }

        protected void AddIP(object sender, EventArgs e, bool byNewMethod)
        {
            int number = Utils.ParseInt(txtExternalAddressesNumber.Text.Trim(), 0);
            List<int> addressIds = listExternalAddresses.Items.Cast<ListItem>()
                .Where(li => li.Selected)
                .Select(li => Utils.ParseInt(li.Value, 0))
                .ToList();

            try
            {
                ResultObject res = null;
                if (byNewMethod)
                    res = ES.Services.VPS2012.AddVirtualMachineExternalIPAddressesByInjection(PanelRequest.ItemID,
                            radioExternalRandom.Checked, number, addressIds.ToArray());
                else
                    res = ES.Services.VPS2012.AddVirtualMachineExternalIPAddresses(PanelRequest.ItemID,
                            radioExternalRandom.Checked, number, addressIds.ToArray());

                if (res.IsSuccess)
                {
                    RedirectBack();
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_ADDING_IP_ADDRESS", "VPS");
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_ADDING_IP_ADDRESS", ex);
            }
        }        
    }
}
