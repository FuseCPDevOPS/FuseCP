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
using FuseCP.Providers.Common;
using FuseCP.EnterpriseServer;
﻿using FuseCP.Portal.Code.Helpers;
using System.IO;

namespace FuseCP.Portal.VPS2012
{
    public partial class VpsDetailsEditConfiguration : FuseCPModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool manageAllowed = VirtualMachines2012Helper.IsVirtualMachineManagementAllowed(PanelSecurity.PackageId);
            if (!manageAllowed) //block access for user if they don't have permission.
                Response.Redirect(EditUrl("SpaceID", PanelSecurity.PackageId.ToString(), ""));

            secHddQOS.Visible = QOSManag.Visible = PanelSecurity.EffectiveUser.Role != UserRole.User;

            // check snapshots
            VirtualMachineSnapshot[] snapshots = ES.Services.VPS2012.GetVirtualMachineSnapshots(PanelRequest.ItemID) ?? Array.Empty<VirtualMachineSnapshot>();
            if (snapshots.Length > 0)
            {
                messageBox.ShowWarningMessage("VPS_CHANGE_VM_CONFIGURATION_SNAPSHOT");
                btnUpdate.Enabled = false;
            }
            else
            {
                btnUpdate.Enabled = true;
            }

            if (!IsPostBack)
            {
                BindConfiguration();
            }
        }

        private void BindConfiguration()
        {
            VirtualMachine vm = null;

            try
            {
                // load machine
                vm = ES.Services.VPS2012.GetVirtualMachineItem(PanelRequest.ItemID);

                if (vm == null)
                {
                    messageBox.ShowErrorMessage("VPS_LOAD_VM_META_ITEM");
                    return;
                }

                // bind CPU cores
                int maxCores = ES.Services.VPS2012.GetMaximumCpuCoresNumber(vm.PackageId);
                PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);

                if (cntx != null && cntx.Quotas.TryGetValue(Quotas.VPS2012_CPU_NUMBER, out QuotaValueInfo cpuQuota2))
                {
                    int cpuQuotausable = (cpuQuota2.QuotaAllocatedValue - cpuQuota2.QuotaUsedValue) + vm.CpuCores;

                    if (cpuQuota2.QuotaAllocatedValue == -1)
                    {
                        for (int i = 1; i < maxCores + 1; i++)
                            ddlCpu.Items.Add(i.ToString());

                        ddlCpu.SelectedIndex = ddlCpu.Items.Count - 1; // select last (maximum) item
                    }
                    else if (cpuQuota2.QuotaAllocatedValue >= cpuQuota2.QuotaUsedValue)
                    {
                        if (cpuQuotausable > maxCores)
                        {
                            for (int i = 1; i < maxCores + 1; i++)
                                ddlCpu.Items.Add(i.ToString());

                            ddlCpu.SelectedIndex = ddlCpu.Items.Count - 1; // select last (maximum) item
                        }
                        else
                        {
                            for (int i = 1; i < cpuQuotausable + 1; i++)
                                ddlCpu.Items.Add(i.ToString());

                            ddlCpu.SelectedIndex = ddlCpu.Items.Count - 1; // select last (maximum) item
                        }
                    }
                    else
                    {
                        for (int i = 1; i < vm.CpuCores + 1; i++)
                            ddlCpu.Items.Add(i.ToString());

                        ddlCpu.SelectedIndex = ddlCpu.Items.Count - 1; // select last (maximum) item
                    }
                }
                else
                {
                    for (int i = 1; i < maxCores + 1; i++)
                        ddlCpu.Items.Add(i.ToString());

                    ddlCpu.SelectedIndex = ddlCpu.Items.Count - 1; // select last (maximum) item
                }

                // bind item
                ddlCpu.SelectedValue = vm.CpuCores.ToString();
                txtRam.Text = vm.RamSize.ToString();
                int firstHddSize = (vm.HddSize != null && vm.HddSize.Length > 0) ? vm.HddSize[0] : 0;
                txtHdd.Text = firstHddSize.ToString();
                hiddenTxtValHdd.Value = firstHddSize.ToString();
                BindAdditionalHdd(vm);
                txtHddMinIOPS.Text = vm.HddMinimumIOPS.ToString();
                txtHddMaxIOPS.Text = vm.HddMaximumIOPS.ToString();
                txtSnapshots.Text = vm.SnapshotsNumber.ToString();

                chkDvdInstalled.Checked = vm.DvdDriveInstalled;
                chkBootFromCd.Checked = vm.BootFromCD;
                chkNumLock.Checked = vm.NumLockEnabled;
                chkSecureBoot.Checked = vm.EnableSecureBoot;
                if (vm.Generation == 1)
                {
                    chkSecureBoot.Checked = false;
                    chkSecureBoot.Enabled = false;
                }

                chkStartShutdown.Checked = vm.StartTurnOffAllowed;
                chkPauseResume.Checked = vm.PauseResumeAllowed;
                chkReset.Checked = vm.ResetAllowed;
                chkReboot.Checked = vm.RebootAllowed;
                chkReinstall.Checked = vm.ReinstallAllowed;

                chkExternalNetworkEnabled.Checked = vm.ExternalNetworkEnabled;
                chkPrivateNetworkEnabled.Checked = vm.PrivateNetworkEnabled;
                chkDmzNetworkEnabled.Checked = vm.DmzNetworkEnabled;

                chkIgnoreHddWarning.Visible = (PanelSecurity.EffectiveUser.Role != UserRole.User);

                // other quotas
                BindCheckboxOption(chkDvdInstalled, Quotas.VPS2012_DVD_ENABLED);
                chkBootFromCd.Enabled = PackagesHelper.IsQuotaEnabled(PanelSecurity.PackageId, Quotas.VPS2012_BOOT_CD_ALLOWED);

                BindCheckboxOption(chkStartShutdown, Quotas.VPS2012_START_SHUTDOWN_ALLOWED);
                BindCheckboxOption(chkPauseResume, Quotas.VPS2012_PAUSE_RESUME_ALLOWED);
                BindCheckboxOption(chkReset, Quotas.VPS2012_RESET_ALOWED);
                BindCheckboxOption(chkReboot, Quotas.VPS2012_REBOOT_ALLOWED);
                BindCheckboxOption(chkReinstall, Quotas.VPS2012_REINSTALL_ALLOWED);

                BindCheckboxOption(chkExternalNetworkEnabled, Quotas.VPS2012_EXTERNAL_NETWORK_ENABLED);
                if (chkExternalNetworkEnabled.Enabled && !chkExternalNetworkEnabled.Checked)
                {
                    PackageIPAddress[] ips = ES.Services.Servers.GetPackageUnassignedIPAddresses(PanelSecurity.PackageId, 0, IPAddressPool.VpsExternalNetwork) ?? Array.Empty<PackageIPAddress>();
                    if (ips.Length == 0)
                    {
                        chkExternalNetworkEnabled.Enabled = false;
                        EmptyExternalAddressesMessage.Visible = true;
                    }
                }
                BindCheckboxOption(chkPrivateNetworkEnabled, Quotas.VPS2012_PRIVATE_NETWORK_ENABLED);
                BindCheckboxOption(chkDmzNetworkEnabled, Quotas.VPS2012_DMZ_NETWORK_ENABLED);

                this.BindSettingsControls(vm);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_LOAD_VM_META_ITEM", ex);
            }
        }

        private void BindCheckboxOption(CheckBox chk, string quotaName)
        {
            chk.Enabled = PackagesHelper.IsQuotaEnabled(PanelSecurity.PackageId, quotaName);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectBack("cancel");
        }

        private void RedirectBack(string action)
        {
            Response.Redirect(EditUrl("ItemID", PanelRequest.ItemID.ToString(), "vps_config",
                "SpaceID=" + PanelSecurity.PackageId,
                "action=" + action));
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                // check rights
                bool manageAllowed = VirtualMachines2012Helper.IsVirtualMachineManagementAllowed(PanelSecurity.PackageId);
                if (!manageAllowed)
                {
                    return;
                }

                // check snapshots
                VirtualMachineSnapshot[] snapshots = ES.Services.VPS2012.GetVirtualMachineSnapshots(PanelRequest.ItemID) ?? Array.Empty<VirtualMachineSnapshot>();
                if (snapshots.Length > 0)
                {
                    return;
                }

                VirtualMachine virtualMachine = new VirtualMachine();
                VirtualMachine vm = ES.Services.VPS2012.GetVirtualMachineItem(PanelRequest.ItemID);
                if (vm == null)
                {
                    messageBox.ShowErrorMessage("VPS_LOAD_VM_META_ITEM");
                    return;
                }

                if (!chkIgnoreHddWarning.Checked || PanelSecurity.EffectiveUser.Role == UserRole.User)
                {
                    if (Utils.ParseInt(hiddenTxtValHdd.Value) > Utils.ParseInt(txtHdd.Text.Trim()))
                    {
                        messageBox.ShowWarningMessage("VPS_CHANGE_HDD_SIZE");
                        return;
                    }
                    List<AdditionalHdd> hdds = GetAdditionalHdd();
                    foreach (AdditionalHdd hdd in hdds)
                    {
                        string[] vmHddPaths = vm.VirtualHardDrivePath ?? Array.Empty<string>();
                        int[] vmHddSizes = vm.HddSize ?? Array.Empty<int>();
                        int vmHddCount = Math.Min(vmHddPaths.Length, vmHddSizes.Length);
                        for (int i = 0; i < vmHddCount; i++)
                        {
                            if (String.IsNullOrEmpty(vmHddPaths[i])) continue;
                            if (Path.GetFileName(vmHddPaths[i]).ToLower().Equals(Path.GetFileName(hdd.DiskPath).ToLower()) && hdd.DiskSize < vmHddSizes[i])
                            {
                                messageBox.ShowWarningMessage("VPS_CHANGE_HDD_SIZE");
                                return;
                            }
                        }
                    }
                }

                // the custom provider control
                this.SaveSettingsControls(ref virtualMachine);
                virtualMachine.CpuCores = Utils.ParseInt(ddlCpu.SelectedValue);
                virtualMachine.RamSize = Utils.ParseInt(txtRam.Text.Trim());
                List<int> hddSize = new List<int>();
                List<String> hddPath = new List<String>();
                hddSize.Add(Utils.ParseInt(txtHdd.Text.Trim()));
                string firstVmHddPath = (vm.VirtualHardDrivePath != null && vm.VirtualHardDrivePath.Length > 0) ? vm.VirtualHardDrivePath[0] : string.Empty;
                hddPath.Add(firstVmHddPath);
                List<AdditionalHdd> additionalHdd = GetAdditionalHdd();
                foreach (AdditionalHdd hdd in additionalHdd)
                {
                    int size = hdd.DiskSize;
                    if (size > 0)
                    {
                        hddSize.Add(size);
                        hddPath.Add(hdd.DiskPath);
                    }
                }
                virtualMachine.HddSize = hddSize.ToArray();
                virtualMachine.VirtualHardDrivePath = hddPath.ToArray();
                virtualMachine.SnapshotsNumber = Utils.ParseInt(txtSnapshots.Text.Trim());
                virtualMachine.HddMinimumIOPS = Utils.ParseInt(txtHddMinIOPS.Text.Trim());
                virtualMachine.HddMaximumIOPS = Utils.ParseInt(txtHddMaxIOPS.Text.Trim());
                virtualMachine.DvdDriveInstalled = chkDvdInstalled.Checked;
                virtualMachine.BootFromCD = chkBootFromCd.Checked;
                virtualMachine.NumLockEnabled = chkNumLock.Checked;
                virtualMachine.EnableSecureBoot = chkSecureBoot.Checked;
                virtualMachine.StartTurnOffAllowed = chkStartShutdown.Checked;
                virtualMachine.PauseResumeAllowed = chkPauseResume.Checked;
                virtualMachine.RebootAllowed = chkReboot.Checked;
                virtualMachine.ResetAllowed = chkReset.Checked;
                virtualMachine.ReinstallAllowed = chkReinstall.Checked;
                virtualMachine.ExternalNetworkEnabled = chkExternalNetworkEnabled.Checked;
                virtualMachine.PrivateNetworkEnabled = chkPrivateNetworkEnabled.Checked;
                virtualMachine.DmzNetworkEnabled = chkDmzNetworkEnabled.Checked;
                virtualMachine.NeedReboot = chkForceReboot.Checked;
                virtualMachine.defaultaccessvlan = vm.defaultaccessvlan;
                virtualMachine.PrivateNetworkVlan = vm.PrivateNetworkVlan;
                virtualMachine.DmzNetworkVlan = vm.DmzNetworkVlan;

                bool setupExternalNetwork = !vm.ExternalNetworkEnabled && chkExternalNetworkEnabled.Checked;
                bool setupPrivateNetwork = !vm.PrivateNetworkEnabled && chkPrivateNetworkEnabled.Checked;
                bool setupDmzNetwork = !vm.DmzNetworkEnabled && chkDmzNetworkEnabled.Checked;
                int[] ipId = new int[1];
                int privAdrCount = 0;
                int dmzAdrCount = 0;

                if (setupExternalNetwork)
                {
                    PackageIPAddress[] ips = ES.Services.Servers.GetPackageUnassignedIPAddresses(PanelSecurity.PackageId, 0, IPAddressPool.VpsExternalNetwork) ?? Array.Empty<PackageIPAddress>();
                    if (ips.Length > 0)
                    {
                        virtualMachine.defaultaccessvlan = ips[0].VLAN;
                        ipId[0] = ips[0].PackageAddressID;
                    }
                }

                if (setupPrivateNetwork)
                {
                    PackageVLANsPaged vlans = ES.Services.Servers.GetPackagePrivateNetworkVLANs(PanelSecurity.PackageId, "", 0, Int32.MaxValue);
                    if (vlans != null && vlans.Items != null && vlans.Count > 0)
                    {
                        virtualMachine.PrivateNetworkVlan = vlans.Items[0].Vlan;
                    }

                    PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);

if (cntx != null && cntx.Quotas.TryGetValue(Quotas.VPS2012_PRIVATE_IP_ADDRESSES_NUMBER, out var _ckv))
                    {
                        QuotaValueInfo privQuota = _ckv;
                        if (privQuota.QuotaAllocatedValue > 0 || privQuota.QuotaAllocatedValue == -1) privAdrCount = 1;
                    }
                }

                if (setupDmzNetwork)
                {
                    PackageVLANsPaged vlans = ES.Services.Servers.GetPackageDmzNetworkVLANs(PanelSecurity.PackageId, "", 0, Int32.MaxValue);
                    if (vlans != null && vlans.Items != null && vlans.Count > 0)
                    {
                        virtualMachine.DmzNetworkVlan = vlans.Items[0].Vlan;
                    }

                    PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);

if (cntx != null && cntx.Quotas.TryGetValue(Quotas.VPS2012_DMZ_IP_ADDRESSES_NUMBER, out var _ckv))
                    {
                        QuotaValueInfo dmzQuota = _ckv;
                        if (dmzQuota.QuotaAllocatedValue > 0 || dmzQuota.QuotaAllocatedValue == -1) dmzAdrCount = 1;
                    }
                }

                ResultObject res = ES.Services.VPS2012.UpdateVirtualMachineResource(PanelRequest.ItemID, virtualMachine);

                if (res.IsSuccess)
                {
                    if (setupExternalNetwork && ipId[0] != 0)
                    {
                        ES.Services.VPS2012.AddVirtualMachineExternalIPAddresses(PanelRequest.ItemID, false, 1, ipId);
                    }

                    if (setupPrivateNetwork && privAdrCount > 0)
                    {
                        ES.Services.VPS2012.AddVirtualMachinePrivateIPAddresses(PanelRequest.ItemID, true, privAdrCount, new string[0], false, null, null, null, null);
                    }

                    if (setupDmzNetwork && dmzAdrCount > 0)
                    {
                        ES.Services.VPS2012.AddVirtualMachineDmzIPAddresses(PanelRequest.ItemID, true, dmzAdrCount, new string[0], false, null, null, null, null);
                    }

                    // redirect back
                    RedirectBack("changed");
                }
                else
                {
                    // show error
                    messageBox.ShowMessage(res, "VPS_CHANGE_VM_CONFIGURATION", "VPS");
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("VPS_CHANGE_VM_CONFIGURATION", ex);
            }
        }

        protected void btnAddHdd_Click(object sender, EventArgs e)
        {
            var hdds = GetAdditionalHdd();
            PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);
            int freeHddGb = 0;
if (cntx != null && cntx.Quotas.TryGetValue(Quotas.VPS2012_HDD, out var _ckv))
            {
                QuotaValueInfo hddQuota = _ckv;
                if (hddQuota.QuotaAllocatedValue != -1)
                {
                    int availSize = hddQuota.QuotaAllocatedValue - hddQuota.QuotaUsedValue;
                    freeHddGb = availSize < 0 ? 0 : availSize;
                    foreach (AdditionalHdd hdd in hdds)
                    {
                        if (hdd.DiskSize > 0 && String.IsNullOrEmpty(hdd.DiskPath)) freeHddGb -= hdd.DiskSize;
                    }
                }
            }
            hdds.Add(new AdditionalHdd(freeHddGb, ""));
            RebindAdditionalHdd(hdds);
        }

        protected void btnRemoveHdd_OnCommand(object sender, CommandEventArgs e)
        {
            var hdds = GetAdditionalHdd();
            hdds.RemoveAt(Convert.ToInt32(e.CommandArgument));
            RebindAdditionalHdd(hdds);
        }

        private void BindAdditionalHdd(VirtualMachine vm)
        {
            int[] vmHddSizes = vm.HddSize ?? Array.Empty<int>();
            string[] vmHddPaths = vm.VirtualHardDrivePath ?? Array.Empty<string>();
            CheckAdditionalHddQuota(vmHddSizes.Length - 1);
            List<AdditionalHdd> result = new List<AdditionalHdd>();
            if (vmHddSizes.Length > 1)
            {
                int vmHddCount = Math.Min(vmHddSizes.Length, vmHddPaths.Length);
                for (int i = 1; i < vmHddCount; i++)
                {
                    if (vmHddSizes[i] == 0 || String.IsNullOrEmpty(vmHddPaths[i])) continue;
                    AdditionalHdd hdd = new AdditionalHdd(vmHddSizes[i], vmHddPaths[i]);
                    result.Add(hdd);
                }
            }
            repHdd.DataSource = result;
            repHdd.DataBind();
        }

        private void RebindAdditionalHdd(List<AdditionalHdd> hdd)
        {
            CheckAdditionalHddQuota(hdd.Count);
            repHdd.DataSource = hdd;
            repHdd.DataBind();
        }

        private void CheckAdditionalHddQuota(int currCount)
        {
            PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);
if (cntx != null && cntx.Quotas.TryGetValue(Quotas.VPS2012_ADDITIONAL_VHD_COUNT, out var _ckv))
            {
                QuotaValueInfo additionalHddQuota = _ckv;
                int quotaHddCount = additionalHddQuota.QuotaAllocatedValue;
                int maxHddCount;
                VirtualMachine vm = ES.Services.VPS2012.GetVirtualMachineItem(PanelRequest.ItemID);
                maxHddCount = vm != null && vm.Generation > 1 ? 62 : 2;







                if (quotaHddCount == -1 || quotaHddCount > maxHddCount) quotaHddCount = maxHddCount;
                btnAddHdd.Enabled = (currCount < quotaHddCount);
            }
            else
            {
                btnAddHdd.Enabled = false;
            }
        }

        private List<AdditionalHdd> GetAdditionalHdd()
        {
            var result = new List<AdditionalHdd>();

            foreach (RepeaterItem item in repHdd.Items)
            {
                AdditionalHdd hdd = new AdditionalHdd(Utils.ParseInt(GetTextBoxText(item, "txtAdditionalHdd")), GetHiddenFieldValue(item, "txtAdditionalHddPath"));
                result.Add(hdd);
            }

            return result;
        }

        private string GetTextBoxText(RepeaterItem item, string name)
        {
            return (item.FindControl(name) as TextBox).Text;
        }

        private string GetHiddenFieldValue(RepeaterItem item, string name)
        {
            return (item.FindControl(name) as HiddenField).Value;
        }
    }
}
