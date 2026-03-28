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
using FuseCP.Providers.Virtualization;
using FuseCP.EnterpriseServer;
using FuseCP.Providers.Common;
using System.Text;
using System.Data;
using System.Linq;

namespace FuseCP.Portal.VPS2012
{
    public partial class VpsDetailsNetwork : FuseCPModuleBase
    {
        VirtualMachine vm = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindRealAssignedAddresses();
                BindVirtualMachine();
                ToggleButtons();
            }
        }

        private void BindVirtualMachine()
        {
            vm = ES.Services.VPS2012.GetVirtualMachineItem(PanelRequest.ItemID);
            if (vm == null)
            {
                secExternalNetwork.Visible = false;
                ExternalNetworkPanel.Visible = false;
                secPrivateNetwork.Visible = false;
                PrivateNetworkPanel.Visible = false;
                secDmzNetwork.Visible = false;
                DmzNetworkPanel.Visible = false;
                btnRestoreExternalAddress.Visible = false;
                btnRestorePrivateAddress.Visible = false;
                btnRestoreDmzAddress.Visible = false;
                return;
            }

            // external network
            if (vm.ExternalNetworkEnabled)
            {
                BindExternalAddresses();
            }
            else
            {
                secExternalNetwork.Visible = false;
                ExternalNetworkPanel.Visible = false;
                btnRestoreExternalAddress.Visible = false;
            }

            // private network
            if (vm.PrivateNetworkEnabled)
            {
                BindPrivateAddresses();
            }
            else
            {
                secPrivateNetwork.Visible = false;
                PrivateNetworkPanel.Visible = false;
                btnRestorePrivateAddress.Visible = false;
            }

            // dmz network
            if (vm.DmzNetworkEnabled)
            {
                BindDmzAddresses();
            }
            else
            {
                secDmzNetwork.Visible = false;
                DmzNetworkPanel.Visible = false;
                btnRestoreDmzAddress.Visible = false;
            }
        }

        private void BindRealAssignedAddresses()
        {
            //VirtualMachine itemVM = VirtualMachines2012Helper.GetCachedVirtualMachine(PanelRequest.ItemID);
            //VirtualMachine _vm = null;
            VirtualMachineNetworkAdapter[] virtualMachineNetworkAdapters = null;
            try
            {
                virtualMachineNetworkAdapters = ES.Services.VPS2012.GetVirtualMachinesNetwordAdapterSettings(PanelRequest.ItemID);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_GET_VM_DETAILS", ex);
            }

            try
            {
                repVMNetwork.DataSource = virtualMachineNetworkAdapters;//new VirtualMachineNetworkAdapter[vm.Adapters.Length];
                repVMNetwork.DataBind();
                BindGridViewOfVmIPs(virtualMachineNetworkAdapters);
                CheckIfPossibleToDoIpInjection(virtualMachineNetworkAdapters);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException)) //TODO: replace by messageBox ????
            {
                VMNetworkError.Text = "Error - " + ex;
                VMNetworkError.Visible = true;
            }                
        }

        private void CheckIfPossibleToDoIpInjection(VirtualMachineNetworkAdapter[] Adapters)
        {
            Adapters ??= Array.Empty<VirtualMachineNetworkAdapter>();
            btnDeletePrivateByInject.Visible = 
                btnDeleteExternalByInject.Visible =
                btnDeleteDmzByInject.Visible =
                btnRestoreExternalAddress.Visible = 
                btnRestorePrivateAddress.Visible =
                btnRestoreDmzAddress.Visible = false;
            foreach (VirtualMachineNetworkAdapter adapter in Adapters.Where(adapter => adapter.IPAddresses != null && adapter.IPAddresses.Length > 0))
            {
                    btnDeletePrivateByInject.Visible =
                        btnDeleteExternalByInject.Visible =
                        btnDeleteDmzByInject.Visible =
                        btnRestoreExternalAddress.Visible =
                        btnRestorePrivateAddress.Visible =
                        btnRestoreDmzAddress.Visible = true;
                    break;
            }
        }

        private void BindGridViewOfVmIPs(VirtualMachineNetworkAdapter[] Adapters)
        {
            Adapters ??= Array.Empty<VirtualMachineNetworkAdapter>();
            int i = 0;
            foreach (RepeaterItem item in repVMNetwork.Items)
            {
                if (i >= Adapters.Length)
                    break;

                DataTable dt = new DataTable();
                dt.Columns.Add("N", typeof(int));
                dt.Columns.Add("IP", typeof(string));
                string[] adapterIPs = Adapters[i].IPAddresses ?? Array.Empty<string>();
                for (int j = 0; j < adapterIPs.Length; j++)
                {
                    DataRow NewRow = dt.NewRow();
                    NewRow["N"] = j + 1;
                    NewRow["IP"] = adapterIPs[j];
                    dt.Rows.Add(NewRow);
                }
                (item.FindControl("gvVMNetwork") as GridView).DataSource = dt;
                (item.FindControl("gvVMNetwork") as GridView).DataBind();
                i++;
            }
        }

        private void BindExternalAddresses()
        {
            // load details
            NetworkAdapterDetails nic = ES.Services.VPS2012.GetExternalNetworkAdapterDetails(PanelRequest.ItemID);
            NetworkAdapterIPAddress[] ipAddresses = nic?.IPAddresses ?? Array.Empty<NetworkAdapterIPAddress>();

            // bind details
            foreach (NetworkAdapterIPAddress ip in ipAddresses.Where(ip => ip.IsPrimary))
            {
                    litExtAddress.Text = ip.IPAddress;
                    litExtSubnet.Text = ip.SubnetMask;
                    litExtGateway.Text = ip.DefaultGateway;
                    break;
            }
            litExtVLAN.Text = nic != null ? nic.VLAN.ToString() : string.Empty;
            locExtVLAN.Visible = nic != null && nic.VLAN > 0;
            litExtVLAN.Visible = locExtVLAN.Visible;
            lblTotalExternal.Text = ipAddresses.Length.ToString();

            // bind IP addresses
            gvExternalAddresses.DataSource = ipAddresses;
            gvExternalAddresses.DataBind();
        }

        protected bool IsVlanEnabled(Object VLAN)
        {
            int vlan = 0;
            if (VLAN != null) Int32.TryParse(VLAN.ToString(), out vlan);
            return vlan > 0;
        }

        private void BindPrivateAddresses()
        {
            // load details
            NetworkAdapterDetails nic = ES.Services.VPS2012.GetPrivateNetworkAdapterDetails(PanelRequest.ItemID);
            NetworkAdapterIPAddress[] ipAddresses = nic?.IPAddresses ?? Array.Empty<NetworkAdapterIPAddress>();

            // bind details
            foreach (NetworkAdapterIPAddress ip in ipAddresses.Where(ip => ip.IsPrimary))
            {
                    litPrivAddress.Text = ip.IPAddress;
                    break;
            }
            litPrivSubnet.Text = nic != null ? nic.SubnetMask : string.Empty;
            litPrivGateway.Text = nic != null ? nic.DefaultGateway : string.Empty;
            litPrivVLAN.Text = nic != null ? nic.VLAN.ToString() : string.Empty;
            locPrivVLAN.Visible = nic != null && nic.VLAN > 0;
            litPrivVLAN.Visible = locPrivVLAN.Visible;
            lblTotalPrivate.Text = ipAddresses.Length.ToString();

            // bind IP addresses
            gvPrivateAddresses.DataSource = ipAddresses;
            gvPrivateAddresses.DataBind();

            if (nic != null && nic.IsDHCP)
            {
                PrivateAddressesPanel.Visible = false;
                litPrivAddress.Text = GetLocalizedString("Automatic.Text");
            }
        }

        private void BindDmzAddresses()
        {
            // load details
            NetworkAdapterDetails nic = ES.Services.VPS2012.GetDmzNetworkAdapterDetails(PanelRequest.ItemID);
            NetworkAdapterIPAddress[] ipAddresses = nic?.IPAddresses ?? Array.Empty<NetworkAdapterIPAddress>();

            // bind details
            foreach (NetworkAdapterIPAddress ip in ipAddresses.Where(ip => ip.IsPrimary))
            {
                    litDmzAddress.Text = ip.IPAddress;
                    break;
            }
            litDmzSubnet.Text = nic != null ? nic.SubnetMask : string.Empty;
            litDmzGateway.Text = nic != null ? nic.DefaultGateway : string.Empty;
            litDmzVLAN.Text = nic != null ? nic.VLAN.ToString() : string.Empty;
            locDmzVLAN.Visible = nic != null && nic.VLAN > 0;
            litDmzVLAN.Visible = locDmzVLAN.Visible;
            lblTotalDmz.Text = ipAddresses.Length.ToString();

            // bind IP addresses
            gvDmzAddresses.DataSource = ipAddresses;
            gvDmzAddresses.DataBind();

            if (nic != null && nic.IsDHCP)
            {
                DmzAddressesPanel.Visible = false;
                litDmzAddress.Text = GetLocalizedString("Automatic.Text");
            }
        }

        private void ToggleButtons()
        {
            bool manageAllowed = VirtualMachines2012Helper.IsVirtualMachineManagementAllowed(PanelSecurity.PackageId);

            btnAddExternalAddress.Visible = manageAllowed;
            btnSetPrimaryExternal.Visible = manageAllowed;
            btnDeleteExternal.Visible = manageAllowed;
            if (gvExternalAddresses.Columns.Count > 0)
                gvExternalAddresses.Columns[0].Visible = manageAllowed;

            btnAddPrivateAddress.Visible = manageAllowed;
            btnSetPrimaryPrivate.Visible = manageAllowed;
            btnDeletePrivate.Visible = manageAllowed;
            if (gvPrivateAddresses.Columns.Count > 0)
                gvPrivateAddresses.Columns[0].Visible = manageAllowed;

            btnAddDmzAddress.Visible = manageAllowed;
            btnSetPrimaryDmz.Visible = manageAllowed;
            btnDeleteDmz.Visible = manageAllowed;
            if (gvDmzAddresses.Columns.Count > 0)
                gvDmzAddresses.Columns[0].Visible = manageAllowed;
        }

        protected void btnRestoreExternalAddress_Click(object sender, EventArgs e)
        {
            try
            {
                ResultObject res = ES.Services.VPS2012.RestoreVirtualMachineExternalIPAddressesByInjection(PanelRequest.ItemID);
                if (res.IsSuccess)
                {
                    BindRealAssignedAddresses();
                    BindVirtualMachine();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_RESTORE_EXTERNAL_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_RESTORE_EXTERNAL_IP", ex);
            }
        }

        protected void btnRestorePrivateByInject_Click(object sender, EventArgs e)
        {
            try
            {
                ResultObject res = ES.Services.VPS2012.RestoreVirtualMachinePrivateIPAddressesByInjection(PanelRequest.ItemID);
                if (res.IsSuccess)
                {
                    BindRealAssignedAddresses();
                    BindVirtualMachine();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_RESTORE_PRIVATE_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_RESTORE_PRIVATE_IP", ex);
            }
        }

        protected void btnRestoreDmzByInject_Click(object sender, EventArgs e)
        {
            try
            {
                ResultObject res = ES.Services.VPS2012.RestoreVirtualMachineDmzIPAddressesByInjection(PanelRequest.ItemID);
                if (res.IsSuccess)
                {
                    BindRealAssignedAddresses();
                    BindVirtualMachine();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_RESTORE_DMZ_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_RESTORE_DMZ_IP", ex);
            }
        }

        protected void btnAddExternalAddress_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "vps_add_external_ip",
                "SpaceID=" + PanelSecurity.PackageId));
        }

        protected void btnAddPrivateAddress_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "vps_add_private_ip",
                "SpaceID=" + PanelSecurity.PackageId));
        }

        protected void btnAddDmzAddress_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "vps_add_dmz_ip",
                "SpaceID=" + PanelSecurity.PackageId));
        }

        protected void btnSetPrimaryPrivate_Click(object sender, EventArgs e)
        {
            int[] addressIds = GetSelectedItems(gvPrivateAddresses);
            
            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = ES.Services.VPS2012.SetVirtualMachinePrimaryPrivateIPAddress(PanelRequest.ItemID, addressIds[0]);

                if (res.IsSuccess)
                {
                    BindPrivateAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_SETTING_PRIMARY_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_SETTING_PRIMARY_IP", ex);
            }
        }

        protected void btnSetPrimaryDmz_Click(object sender, EventArgs e)
        {
            int[] addressIds = GetSelectedItems(gvDmzAddresses);

            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = ES.Services.VPS2012.SetVirtualMachinePrimaryDmzIPAddress(PanelRequest.ItemID, addressIds[0]);

                if (res.IsSuccess)
                {
                    BindDmzAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_SETTING_PRIMARY_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_SETTING_PRIMARY_IP", ex);
            }
        }

        protected void btnDeletePrivateByInject_Click(object sender, EventArgs e)
        {
            DeletePrivate(sender, e, true);
        }

        protected void btnDeletePrivate_Click(object sender, EventArgs e)
        {
            DeletePrivate(sender, e, false);
        }

        protected void DeletePrivate(object sender, EventArgs e, bool byNewMethod)
        {
            int[] addressIds = GetSelectedItems(gvPrivateAddresses);

            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = null;
                res = byNewMethod ? ES.Services.VPS2012.DeleteVirtualMachinePrivateIPAddressesByInject(PanelRequest.ItemID, addressIds) : ES.Services.VPS2012.DeleteVirtualMachinePrivateIPAddresses(PanelRequest.ItemID, addressIds);




                if (res.IsSuccess)
                {
                    BindPrivateAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_DELETING_IP_ADDRESS", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_DELETING_IP_ADDRESS", ex);
            }
        }

        protected void btnDeleteDmzByInject_Click(object sender, EventArgs e)
        {
            DeleteDmz(sender, e, true);
        }

        protected void btnDeleteDmz_Click(object sender, EventArgs e)
        {
            DeleteDmz(sender, e, false);
        }

        protected void DeleteDmz(object sender, EventArgs e, bool byNewMethod)
        {
            int[] addressIds = GetSelectedItems(gvDmzAddresses);

            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = null;
                res = byNewMethod ? ES.Services.VPS2012.DeleteVirtualMachineDmzIPAddressesByInject(PanelRequest.ItemID, addressIds) : ES.Services.VPS2012.DeleteVirtualMachineDmzIPAddresses(PanelRequest.ItemID, addressIds);




                if (res.IsSuccess)
                {
                    BindDmzAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_DELETING_IP_ADDRESS", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_DELETING_IP_ADDRESS", ex);
            }
        }

        protected void btnSetPrimaryExternal_Click(object sender, EventArgs e)
        {
            int[] addressIds = GetSelectedItems(gvExternalAddresses);

            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = ES.Services.VPS2012.SetVirtualMachinePrimaryExternalIPAddress(PanelRequest.ItemID, addressIds[0]);

                if (res.IsSuccess)
                {
                    BindExternalAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_SETTING_PRIMARY_IP", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_SETTING_PRIMARY_IP", ex);
            }
        }

        protected void btnDeleteExternal_Click(object sender, EventArgs e)
        {
            DeleteExternal(sender, e, false);
        }
        protected void btnDeleteExternalByInject_Click(object sender, EventArgs e)
        {
            DeleteExternal(sender, e, true);
        }

        protected void DeleteExternal(object sender, EventArgs e, bool byNewMethod)
        {
            int[] addressIds = GetSelectedItems(gvExternalAddresses);

            // check if at least one is selected
            if (addressIds.Length == 0)
            {
                messageBox.ShowWarningMessage("IP_ADDRESS_NOT_SELECTED");
                return;
            }

            try
            {
                ResultObject res = null;
                res = byNewMethod ? ES.Services.VPS2012.DeleteVirtualMachineExternalIPAddressesByInjection(PanelRequest.ItemID, addressIds) : ES.Services.VPS2012.DeleteVirtualMachineExternalIPAddresses(PanelRequest.ItemID, addressIds);




                if (res.IsSuccess)
                {
                    BindExternalAddresses();
                    return;
                }
                else
                {
                    messageBox.ShowMessage(res, "VPS_ERROR_DELETING_IP_ADDRESS", "VPS");
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_ERROR_DELETING_IP_ADDRESS", ex);
            }
        }        

        private int[] GetSelectedItems(GridView gv)
        {
            List<int> items = new List<int>();
            if (gv == null || gv.DataKeys == null)
                return items.ToArray();

            for (int i = 0; i < gv.Rows.Count; i++)
            {
                GridViewRow row = gv.Rows[i];
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect == null || !chkSelect.Checked)
                    continue;

                DataKey dataKey = gv.DataKeys[i];
                if (dataKey?.Value == null)
                    continue;

                items.Add((int)dataKey.Value);
            }

            return items.ToArray();
        }
    }
}
