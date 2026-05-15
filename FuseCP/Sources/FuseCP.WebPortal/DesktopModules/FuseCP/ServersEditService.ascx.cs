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

using Nito.AsyncEx;
using FuseCP.EnterpriseServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace FuseCP.Portal
{
	public partial class ServersEditService : FuseCPModuleBase
	{
		int ServiceId;
		Task<ServiceInfo> service = null;
		Task<ProviderInfo> provider = null;
		Task<ResourceGroupInfo> resourceGroup = null;

		public AsyncLock ServiceLock = new AsyncLock();
		async Task<ServiceInfo> Service()
		{
			using (await ServiceLock.LockAsync())
			{
				if (service == null)
				{
					service = ES.Services.Servers.GetServiceInfoAsync(ServiceId);
				}
			}
			return await service;
		}
        public AsyncLock ProviderLock = new AsyncLock();
        async Task<ProviderInfo> Provider()
		{
			var local_service = await Service();
            using (await ProviderLock.LockAsync())
            {
                if (provider == null)
					provider = ES.Services.Servers.GetProviderAsync(local_service.ProviderId);
			}
			return await provider;
		}
        public AsyncLock ResourceLock = new AsyncLock();
        async Task<ResourceGroupInfo> ResourceGroup()
		{
			var local_provider = await Provider();
            using (await ResourceLock.LockAsync())
            {
                if (resourceGroup == null)
				{
					resourceGroup = ES.Services.Servers.GetResourceGroupAsync(local_provider.GroupId);
				}
			}
			return await resourceGroup;
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			Page.Load += PageLoadAsync;
		}

		Task loadService, loadSettingsControl;
		protected async void PageLoadAsync(object sender, EventArgs e)
		{
			try
			{
				ServiceId = PanelRequest.ServiceId;
				// load service settings control
				loadService = LoadService();
				loadSettingsControl = LoadSettingsControl();

				rowInstallResults.Visible = false;

				if (!IsPostBack)
				{
					await Task.WhenAll(
						BindClusters(),
						BindService(),
						BindServiceProperties(),
						BindServiceQuota(),
						ToggleGlobalDNS());
				}

				await Task.WhenAll(loadService, loadSettingsControl);
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_GET_SERVICE", ex);
				return;
			}
		}

		private async Task LoadService()
		{
			var local_service = await Service();

			if (local_service == null)
				// return
				RedirectBack();

			// load local_provider details
			await Provider();

			// load resource group details
			await ResourceGroup();
		}

		private async Task BindService()
		{
			var local_resourceGroup = await ResourceGroup();
			var local_provider = await Provider();

			litGroup.Text = PanelFormatter.GetLocalizedResourceGroupName(local_resourceGroup.GroupName);

            if (ResourceGroups.Mail == local_resourceGroup.GroupName && local_provider.ProviderName.StartsWith("SmarterMail", StringComparison.OrdinalIgnoreCase))
            {
				textProvider.Visible = false;
                var providers = await ES.Services.Servers.GetProvidersByGroupIdAsync(local_provider.GroupId);
                var filteredProviders = providers
                    .Where(p => p.ProviderName != null && p.ProviderName.StartsWith("SmarterMail", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ddlProviders.DataSource = filteredProviders;
                ddlProviders.DataBind();

                ddlProviders.SelectedValue = local_provider.ProviderId.ToString();
            }
            else if (ResourceGroups.Ftp == local_resourceGroup.GroupName || ResourceGroups.Mail == local_resourceGroup.GroupName || ResourceGroups.Dns == local_resourceGroup.GroupName)
			{
                selectProvider.Visible = false;
                litProvider.Text = local_provider.DisplayName;
            }
			else
			{
				textProvider.Visible = false;
				ddlProviders.DataSource = await ES.Services.Servers.GetProvidersByGroupIdAsync(local_provider.GroupId);
				ddlProviders.DataBind();
				ddlProviders.SelectedValue = local_provider.ProviderId.ToString();
			}

			var local_service = await Service();

			txtServiceName.Text = local_service.ServiceName;
			txtQuotaValue.Text = local_service.ServiceQuotaValue.ToString();
			Utils.SelectListItem(ddlClusters, local_service.ClusterId);
			txtComments.Text = local_service.Comments;
		}

		private async Task BindServiceQuota()
		{
			var local_provider = await Provider();

			QuotaInfo quota = await ES.Services.Servers.GetProviderServiceQuotaAsync(local_provider.ProviderId);
			if (quota != null)
			{
				lblQuotaName.Text = GetSharedLocalizedString(Utils.ModuleName, "Quota." + quota.QuotaName);
			}
			else
			{
				pnlQuota.Visible = false;
			}
		}

		private async Task LoadSettingsControl()
		{
			try
			{
				var local_provider = await Provider();

				// try to locate suitable control
				string currPath = this.AppRelativeVirtualPath;
				currPath = currPath.Substring(0, currPath.LastIndexOf("/"));
				string ctrlPath = currPath + "/ProviderControls/" + local_provider.EditorControl + "_Settings.ascx";

				IHostingServiceProviderSettings ctrl =
					 (IHostingServiceProviderSettings)Page.LoadControl(ctrlPath);

				// add control to the placeholder
				serviceProps.Controls.Add((Control)ctrl);
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_LOAD_SERVICE_CONTROL", ex);
				return;
			}
		}

		private async Task BindServiceProperties()
		{
			await loadSettingsControl;

			// find control
			IHostingServiceProviderSettings ctrl = serviceProps.Controls
				.OfType<Control>()
				.FirstOrDefault()
				as IHostingServiceProviderSettings;
			if (ctrl == null)
				return;

			// load service properties and bind them
			string[] settings = await ES.Services.Servers.GetServiceSettingsAsync(ServiceId);

			// bind
			ctrl.BindSettings(ConvertArrayToDictionary(settings));
		}

		private async Task ToggleGlobalDNS()
		{
			var local_resourceGroup = await ResourceGroup();

			DnsRecrodsPanel.Visible = DnsRecrodsHeader.Visible = ((local_resourceGroup.GroupName == ResourceGroups.BlackBerry) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.OCS) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Os) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.HostedOrganizations) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.SharepointFoundationServer) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.SharepointEnterpriseServer) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Mail) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Lync) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.SfB) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Exchange) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Web) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Dns) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Ftp) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2000) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2005) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2008) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2012) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2014) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2016) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2017) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2019) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2022) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MsSql2025) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MySql4) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MySql5) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MySql8) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MySql9) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.MariaDB) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Statistics) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.VPS) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.VPS2012) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.VPSForPC) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.RDS) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.EnterpriseStorage) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.Filters) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.SharePoint) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.SharepointServer) ||
																				 (local_resourceGroup.GroupName == ResourceGroups.StorageSpaces)
																				 );
		}


		private void SaveServiceProperties()
		{
			// find control
			try
			{
				if (serviceProps.Controls.Count == 0)
					return;

				IHostingServiceProviderSettings ctrl = serviceProps.Controls[0] as IHostingServiceProviderSettings;
				if (ctrl == null)
					return;

				// grab settings
				StringDictionary settings = new StringDictionary();
				ctrl.SaveSettings(settings);

				// save settings
				int result = ES.Services.Servers.UpdateServiceSettings(PanelRequest.ServiceId,
					 ConvertDictionaryToArray(settings));

				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_UPDATE_SERVICE_PROPS", ex);
				return;
			}
		}

		protected void btnUpdate_Click(object sender, EventArgs e)
		{
			// validate input
			if (!Page.IsValid)
				return;

			var local_service = new ServiceInfo();
			local_service.ServiceId = PanelRequest.ServiceId;
			local_service.ServiceName = txtServiceName.Text.Trim();
			local_service.ProviderId = ddlProviders.Items.Count > 0 ? Utils.ParseInt(ddlProviders.SelectedValue, 0) : 0; //just to be sure that here is 0;
			local_service.ServiceQuotaValue = Utils.ParseInt(txtQuotaValue.Text, 0);
			local_service.ClusterId = Utils.ParseInt(ddlClusters.SelectedValue, 0);
			local_service.Comments = txtComments.Text;

			// update local_service
			try
			{
				int result = ES.Services.Servers.UpdateService(local_service);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_UPDATE_SERVICE", ex);
				return;
			}

			// save properties
			SaveServiceProperties();

			// install local_service
			string[] installResults = null;
			try
			{
				installResults = ES.Services.Servers.InstallService(PanelRequest.ServiceId);
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_INSTALL_SERVICE", ex);
				return;
			}

			// check results
			if (installResults != null && installResults.Length > 0)
			{
				rowInstallResults.Visible = true;
				blInstallResults.Items.Clear();
				foreach (string installResult in installResults)
					blInstallResults.Items.Add(installResult);

				return;
			}
			// save quotas
			//SaveServiceQuotas();

			// return
			RedirectBack();
		}
		protected void btnCancel_Click(object sender, EventArgs e)
		{
			// return
			RedirectBack();
		}
		protected void btnDelete_Click(object sender, EventArgs e)
		{
			if (PanelRequest.ServiceId != 0)
			{
				// delete service
				try
				{
					int result = ES.Services.Servers.DeleteService(PanelRequest.ServiceId);
					if (result < 0)
					{
						ShowResultMessage(result);
						return;
					}
				}
				catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
				{
					ShowErrorMessage("SERVER_DELETE_SERVICE", ex);
					return;
				}
			}

			// return
			RedirectBack();
		}

		private void RedirectBack()
		{
			// redirect to the previous page
			Response.Redirect(EditUrl("ServerID", PanelRequest.ServerId.ToString(), "edit_server"), false);
			Context.ApplicationInstance.CompleteRequest();
		}

		#region Cluster methods
		private async Task BindClusters()
		{
			try
			{
				ddlClusters.DataSource = await ES.Services.Servers.GetClustersAsync();
				ddlClusters.DataBind();

				ddlClusters.Items.Insert(0, new ListItem("<Not Included>", ""));
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_GET_CLUSTER", ex);
				return;
			}
		}
		protected async void cmdAddCluster_Click(object sender, EventArgs e)
		{
			ClusterInfo cluster = new ClusterInfo();
			cluster.ClusterName = txtClusterName.Text.Trim();

			try
			{
				int result = ES.Services.Servers.AddCluster(cluster);
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_ADD_CLUSTER", ex);
				return;
			}

			// rebind
			await BindClusters();
			txtClusterName.Text = "";
		}
		protected async void cmdDeleteCluster_Click(object sender, EventArgs e)
		{
			try
			{
				int result = ES.Services.Servers.DeleteCluster(Utils.ParseInt(ddlClusters.SelectedValue, 0));
				if (result < 0)
				{
					ShowResultMessage(result);
					return;
				}
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				ShowErrorMessage("SERVER_DELETE_CLUSTER", ex);
				return;
			}

			// rebind
			await BindClusters();
		}
		#endregion

		#region Helper methods
		private string[] ConvertDictionaryToArray(StringDictionary settings)
		{
			List<string> r = new List<string>();
			foreach (string key in settings.Keys)
				r.Add(key + "=" + settings[key]);
			return r.ToArray();
		}

		private StringDictionary ConvertArrayToDictionary(string[] settings)
		{
			StringDictionary r = new StringDictionary();
			foreach (string setting in settings)
			{
				int idx = setting.IndexOf('=');
				r.Add(setting.Substring(0, idx), setting.Substring(idx + 1));
			}
			return r;
		}
		#endregion
	}
}
