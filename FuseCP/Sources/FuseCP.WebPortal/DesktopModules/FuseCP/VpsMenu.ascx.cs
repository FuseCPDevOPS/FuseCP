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
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.EnterpriseServer;
using FuseCP.WebPortal;
namespace FuseCP.Portal
{
    public partial class VpsMenu : FuseCPModuleBase
    {
        private const string PID_SPACE_VPS = "SpaceVPS2012";
        private const string PID_SPACE_PROXMOX = "SpaceProxmox";
        protected void Page_Load(object sender, EventArgs e)
        {
            // organization
            bool vpsVisible = (Request.QueryString[DefaultPage.PAGE_ID_PARAM].Equals(PID_SPACE_VPS, StringComparison.InvariantCultureIgnoreCase) ||
                                Request.QueryString[DefaultPage.PAGE_ID_PARAM].Equals(PID_SPACE_PROXMOX, StringComparison.InvariantCultureIgnoreCase));

            vpsMenu.Visible = vpsVisible;
            if (vpsVisible)
            {
                MenuItem rootItem = new MenuItem(locMenuTitle.Text);
                rootItem.Value = "VPS Menu";
                rootItem.Selectable = false;
                menu.Items.Add(rootItem);
                BindMenu(rootItem.ChildItems);
            }
        }
        virtual public int PackageId
        {
            get { return PanelSecurity.PackageId; }
            set { }
        }
        virtual public int ItemID
        {
            get { return PanelRequest.ItemID; }
            set { }
        }
        private PackageContext cntx = null;
        virtual public PackageContext Cntx
        {
            get
            {
                if (cntx == null) cntx = PackagesHelper.GetCachedPackageContext(PackageId);
                return cntx;
            }
        }
        public void BindMenu(MenuItemCollection items)
        {
            if (PackageId <= 0)
                return;
            // VPS Menu
            if (Cntx.Groups.ContainsKey(ResourceGroups.VPS2012) || Cntx.Groups.ContainsKey(ResourceGroups.Proxmox))
                PrepareVPS2012Menu(items);
        }
        private void PrepareVPS2012Menu(MenuItemCollection vpsItems)
        {
            bool isAdmin = (PanelSecurity.EffectiveUser.Role == UserRole.Administrator);
            bool vps2012ExternalEnabled = cntx.Quotas.TryGetValue(Quotas.VPS2012_EXTERNAL_NETWORK_ENABLED, out var vps2012ExternalQuota)
                && !vps2012ExternalQuota.QuotaExhausted;
            bool proxmoxExternalEnabled = cntx.Quotas.TryGetValue(Quotas.PROXMOX_EXTERNAL_NETWORK_ENABLED, out var proxmoxExternalQuota)
                && !proxmoxExternalQuota.QuotaExhausted;
            bool vps2012PrivateEnabled = cntx.Quotas.TryGetValue(Quotas.VPS2012_PRIVATE_NETWORK_ENABLED, out var vps2012PrivateQuota)
                && !vps2012PrivateQuota.QuotaExhausted;
            bool proxmoxPrivateEnabled = cntx.Quotas.TryGetValue(Quotas.PROXMOX_PRIVATE_NETWORK_ENABLED, out var proxmoxPrivateQuota)
                && !proxmoxPrivateQuota.QuotaExhausted;
            bool vps2012DmzEnabled = cntx.Quotas.TryGetValue(Quotas.VPS2012_DMZ_NETWORK_ENABLED, out var vps2012DmzQuota)
                && !vps2012DmzQuota.QuotaExhausted;

            // add items
            vpsItems.Add(CreateMenuItem("VPSHome", ""));
            if ((vps2012ExternalEnabled || proxmoxExternalEnabled)
                || (PanelSecurity.PackageId == 1 && isAdmin))
                vpsItems.Add(CreateMenuItem("ExternalNetwork", "vdc_external_network"));
            if (isAdmin)
                vpsItems.Add(CreateMenuItem("ManagementNetwork", "vdc_management_network"));
            if (vps2012PrivateEnabled || proxmoxPrivateEnabled)
                vpsItems.Add(CreateMenuItem("PrivateNetwork", "vdc_private_network"));
            if (vps2012DmzEnabled)
                vpsItems.Add(CreateMenuItem("DmzNetwork", "vdc_dmz_network"));
            vpsItems.Add(CreateMenuItem("AuditLog", "vdc_audit_log"));
        }
        private MenuItem CreateMenuItem(string text, string key)
        {
            return CreateMenuItem(text, key, null);
        }
        protected virtual MenuItem CreateMenuItem(string text, string key, string img)
        {
            MenuItem item = new MenuItem();
            item.Text = GetLocalizedString("Text." + text);
            var hostModule = GetAllControlsOfType<FuseCPModuleBase>(this.Page);
            if (hostModule.Count > 0)
            {
                item.NavigateUrl = hostModule.LastOrDefault().EditUrl("SpaceID", PanelSecurity.PackageId.ToString(), key); // PortalUtils.EditUrl("ItemID", ItemID.ToString(), key, "SpaceID=" + PackageId);
            }
            //if (img == null)
            //    item.ImageUrl = PortalUtils.GetThemedIcon("Icons/tool_48.png");
            //else
            //    item.ImageUrl = PortalUtils.GetThemedIcon(img);

            string pid = Request.QueryString[DefaultPage.PAGE_ID_PARAM];
            if (!String.IsNullOrEmpty(pid)
                && (pid.Equals(PID_SPACE_VPS, StringComparison.InvariantCultureIgnoreCase)
                    || pid.Equals(PID_SPACE_PROXMOX, StringComparison.InvariantCultureIgnoreCase)))
            {
            string ctl = Request.QueryString["ctl"] ?? String.Empty;
            bool isHome = String.IsNullOrEmpty(key);

            // VPSHome should be selected for any VPS server tab (vps_general, vps_config, vps_dvd, etc.)
            // or when no specific ctl is provided (empty ctl = default home)
            if ((isHome && (String.IsNullOrEmpty(ctl) || ctl.StartsWith("vps_", StringComparison.InvariantCultureIgnoreCase)))
                || (!isHome && ctl.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                item.Selected = true;
            }
            }

            return item;
        }
        public static List<T> GetAllControlsOfType<T>(Control parent) where T : Control
        {
            var result = new List<T>();
            foreach (Control control in parent.Controls)
            {
                if (control is T)
                {
                    result.Add((T)control);
                }
                if (control.HasControls())
                {
                    result.AddRange(GetAllControlsOfType<T>(control));
                }
            }
            return result;
        }
    }
}
