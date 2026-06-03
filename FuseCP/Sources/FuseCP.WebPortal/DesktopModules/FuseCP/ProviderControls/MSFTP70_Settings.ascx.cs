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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using FuseCP.EnterpriseServer;
using FuseCP.Providers.FTP;

namespace FuseCP.Portal.ProviderControls
{
    public partial class MSFTP70_Settings : FuseCPControlBase, IHostingServiceProviderSettings
    {
        private const string HardenedLabel = "Hardened";
        private const string NotHardenedLabel = "Not hardened";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BindSettings(StringDictionary settings)
        {
			int selectedAddressid = this.FindAddressByText(settings["SharedIP"]);
			ipAddress.AddressId = (selectedAddressid > 0) ? selectedAddressid : 0;
            BindSiteId(settings["SiteId"]);
            txtAdFtpRoot.Text = settings["AdFtpRoot"];
            txtFtpGroupName.Text = settings["FtpGroupName"];
			chkBuildUncFilesPath.Checked = Utils.ParseBool(settings["BuildUncFilesPath"], false);
            ActiveDirectoryIntegration.BindSettings(settings);
            UpdateHardeningUi();
        }

        public void SaveSettings(StringDictionary settings)
        {
			if (ipAddress.AddressId > 0)
			{
				IPAddressInfo address = ES.Services.Servers.GetIPAddress(ipAddress.AddressId);
                settings["SharedIP"] = String.IsNullOrEmpty(address.InternalIP)
                    ? address.ExternalIP
                    : address.InternalIP;
			}
			else
			{
				settings["SharedIP"] = String.Empty;
			}
        	settings["SiteId"] = ddlSite.SelectedValue;
            if (!string.IsNullOrWhiteSpace(txtAdFtpRoot.Text))
            {
                settings["AdFtpRoot"] = txtAdFtpRoot.Text.Trim();
            }
            settings["FtpGroupName"] = txtFtpGroupName.Text.Trim();
			settings["BuildUncFilesPath"] = chkBuildUncFilesPath.Checked.ToString();
            ActiveDirectoryIntegration.SaveSettings(settings);
        }

		private int FindAddressByText(string address)
		{
		    if (string.IsNullOrEmpty(address))
		    {
		        return 0;
		    }

            foreach (IPAddressInfo addressInfo in ES.Services.Servers.GetIPAddresses(IPAddressPool.General, PanelRequest.ServerId).Where(addressInfo => addressInfo.InternalIP == address || addressInfo.ExternalIP == address))
			{
					return addressInfo.AddressId;
			}
			return 0;
		}

        private void BindSiteId(string selectedSiteId)
        {
            ddlSite.Items.Clear();
            var sites = ES.Services.FtpServers.GetFtpSites(PanelRequest.ServiceId);

            foreach (var item in sites.Select(site => new ListItem(site.Name + " (User Isolation Mode: " + site["UserIsolationMode"] + ")", site.Name)))
            {

                if (item.Value == selectedSiteId)
                {
                    item.Selected = true;
                }

                ddlSite.Items.Add(item);
            }

            if (ddlSite.Items.Count == 0)
            {
                ddlSite.Items.Add(new ListItem("Default FTP Site (not yet created)", "Default FTP Site"));
            }
            else
            {
                if (ddlSite.SelectedItem == null)
                {
                    ddlSite.SelectedIndex = 0;
                }
                ddlSite_SelectedIndexChanged(this, null);
            }
        }

        protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            var isActiveDirectoryUserIsolated = ddlSite.SelectedItem.Text.Contains("ActiveDirectory");
            FtpRootRow.Visible = isActiveDirectoryUserIsolated;
            txtAdFtpRootReqValidator.Enabled= isActiveDirectoryUserIsolated;
            UpdateHardeningUi();
        }

        protected void cmdHardenNow_Click(object sender, EventArgs e)
        {
            litHardeningMessage.Text = String.Empty;

            try
            {
                string[] installResults = ES.Services.Servers.InstallService(PanelRequest.ServiceId);
                if (installResults != null && installResults.Length > 0)
                {
                    litHardeningMessage.Text = "<br /><span class='text-danger'>Hardening failed: " + String.Join(" | ", installResults.Select(Server.HtmlEncode)) + "</span>";
                }
                else
                {
                    litHardeningMessage.Text = "<br /><span class='text-success'>Hardening applied successfully.</span>";
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                litHardeningMessage.Text = "<br /><span class='text-danger'>Hardening failed: " + Server.HtmlEncode(ex.Message) + "</span>";
            }

            string selectedSiteId = ddlSite.SelectedValue;
            BindSiteId(selectedSiteId);
            UpdateHardeningUi();
        }

        private void UpdateHardeningUi()
        {
            var selectedSite = GetSelectedSite();
            if (selectedSite == null)
            {
                litHardeningStatus.Text = "<span class='text-warning'>Not installed</span>";
                cmdHardenNow.Visible = true;
                litHardeningDetails.Text = "<div class='text-muted'>No FTP site found yet for this service.</div>";
                return;
            }

            bool isHardened = IsIisFtpSiteHardened(selectedSite);
            litHardeningStatus.Text = isHardened
                ? "<span class='text-success fw-semibold'>" + HardenedLabel + "</span>"
                : "<span class='text-danger fw-semibold'>" + NotHardenedLabel + "</span>";
            cmdHardenNow.Visible = !isHardened;
            litHardeningDetails.Text = BuildHardeningDetailsHtml(selectedSite);
        }

        private FtpSite GetSelectedSite()
        {
            if (ddlSite.SelectedItem == null)
            {
                return null;
            }

            string selectedSiteId = ddlSite.SelectedValue;
            var sites = ES.Services.FtpServers.GetFtpSites(PanelRequest.ServiceId);
            return sites.FirstOrDefault(site => String.Equals(site.Name, selectedSiteId, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsIisFtpSiteHardened(FtpSite site)
        {
            if (site == null)
            {
                return false;
            }

            bool anonymousDisabled = !site.AllowAnonymous;
            bool userIsolated = String.Equals(site["UserIsolationMode"], "StartInUsersDirectory", StringComparison.OrdinalIgnoreCase);
            return anonymousDisabled && userIsolated;
        }

        private static string BuildHardeningDetailsHtml(FtpSite site)
        {
            bool anonymousDisabled = !site.AllowAnonymous;
            bool userIsolated = String.Equals(site["UserIsolationMode"], "StartInUsersDirectory", StringComparison.OrdinalIgnoreCase);
            string isolationMode = String.IsNullOrEmpty(site["UserIsolationMode"]) ? "Unknown" : site["UserIsolationMode"];

            string anonymousLine = anonymousDisabled
                ? "<li class='text-success'>Anonymous authentication: disabled</li>"
                : "<li class='text-danger'>Anonymous authentication: enabled</li>";

            string isolationLine = userIsolated
                ? "<li class='text-success'>User isolation mode: StartInUsersDirectory</li>"
                : "<li class='text-danger'>User isolation mode: " + HttpUtility.HtmlEncode(isolationMode) + "</li>";

            return "<ul class='mb-0 ps-3'>" + anonymousLine + isolationLine + "</ul>";
        }
    }
}
