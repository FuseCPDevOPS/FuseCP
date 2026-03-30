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
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.Providers.HostedSolution;
using System.Linq;
using FuseCP.Providers.Web;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.Providers.RemoteDesktopServices;
using AjaxControlToolkit;

namespace FuseCP.Portal.RDS.UserControls
{
    public partial class RDSCollectionServers : FuseCPControlBase
	{
        public const string DirectionString = "DirectionString";
        public event EventHandler OnRefreshClicked;

		protected enum SelectedState
		{
			All,
			Selected,
			Unselected
		}

        public void SetServers(RdsServer[] servers)
		{
            BindServers(servers, false);
		}

        public void BindServers(RdsServer[] servers)
        {
            gvServers.DataSource = servers;
            gvServers.DataBind();
        }

        public void HideRefreshButton()
        {
            btnRefresh.Visible = false;
        }
        
        public List<RdsServer> GetServers()
        {
            return GetGridViewServers(SelectedState.All);
        }

		protected void Page_Load(object sender, EventArgs e)
		{            
			// register javascript
			if (!Page.ClientScript.IsClientScriptBlockRegistered("SelectAllCheckboxes"))
			{
				string script = @"    function SelectAllCheckboxes(box)
                {
		            var state = box.checked;
                    var elm = box.parentElement.parentElement.parentElement.parentElement.getElementsByTagName(""INPUT"");
                    for(i = 0; i < elm.length; i++)
                        if(elm[i].type == ""checkbox"" && elm[i].id != box.id && elm[i].checked != state && !elm[i].disabled)
		                    elm[i].checked = state;
                }";
                Page.ClientScript.RegisterClientScriptBlock(typeof(RDSCollectionUsers), "SelectAllCheckboxes",
					script, true);
			}

            if (!IsPostBack && PanelRequest.CollectionID > 0)
            {
                BindOrganizationServers();
            }
		}

        
		protected void btnAdd_Click(object sender, EventArgs e)
		{
			// bind all servers
			BindPopupServers(); 
     
			// show modal
			AddServersModal.Show();
		}

		protected void btnDelete_Click(object sender, EventArgs e)
		{
            List<RdsServer> selectedServers = GetGridViewServers(SelectedState.Unselected);

            BindServers(selectedServers.ToArray(), false);
		}

		protected void btnAddSelected_Click(object sender, EventArgs e)
		{
            List<RdsServer> selectedServers = GetPopUpGridViewServers();

            BindServers(selectedServers.ToArray(), true);
		}

        protected void BindPopupServers()
		{
            //TODO supply correct value for parameter rdsControllerServiceID.
            throw new NotImplementedException("This feture has to be corrected.");
		}

        protected void BindServers(RdsServer[] newServers, bool preserveExisting)
		{
			// get binded addresses
            List<RdsServer> servers = new List<RdsServer>();
			if(preserveExisting)
                servers.AddRange(GetGridViewServers(SelectedState.All));

            // add new servers

            if (newServers != null)
			{
                foreach (RdsServer newServer in newServers)
				{
					// check if exists
					if (servers.Any(server => server.Id == newServer.Id))
						continue;

                    servers.Add(newServer);
				}
			}

            gvServers.DataSource = servers;
            gvServers.DataBind();
		}

        protected List<RdsServer> GetGridViewServers(SelectedState state)
        {
            List<RdsServer> servers = new List<RdsServer>();
            for (int i = 0; i < gvServers.Rows.Count; i++)
            {
                GridViewRow row = gvServers.Rows[i];
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect == null)
                    continue;

                DataKey dataKey = gvServers.DataKeys[i];
                if (dataKey?.Value == null)
                    continue;

                RdsServer server = new RdsServer();
                server.Id = (int)dataKey.Value;
                server.FqdName = ((Literal)row.FindControl("litFqdName")).Text;
                server.Status = ((Literal)row.FindControl("litStatus")).Text;
                var rdsCollectionId = ((HiddenField)row.FindControl("hdnRdsCollectionId")).Value;

                if (!string.IsNullOrEmpty(rdsCollectionId))
                {
                    server.RdsCollectionId = Convert.ToInt32(rdsCollectionId);
                }

                if (state == SelectedState.All ||
                    (state == SelectedState.Selected && chkSelect.Checked) ||
                    (state == SelectedState.Unselected && !chkSelect.Checked))
                    servers.Add(server);
            }

            return servers;
        }

        protected List<RdsServer> GetPopUpGridViewServers()
        {
            List<RdsServer> servers = new List<RdsServer>();
            for (int i = 0; i < gvPopupServers.Rows.Count; i++)
            {
                GridViewRow row = gvPopupServers.Rows[i];
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect == null)
                    continue;

                if (chkSelect.Checked)
                {
                    DataKey dataKey = gvPopupServers.DataKeys[i];
                    if (dataKey?.Value == null)
                        continue;

                    servers.Add(new RdsServer
                    {
                        Id = (int)dataKey.Value,
                        FqdName = ((Literal)row.FindControl("litName")).Text
                    });
                }
            }

            return servers;

        }

        protected void BindOrganizationServers()
        {
            //TODO supply correct value for parameter rdsControllerServiceID.
            throw new NotImplementedException("This feture has to be corrected.");
        }

		protected void cmdSearch_Click(object sender, EventArgs e)
		{
			BindPopupServers();
		}

        protected SortDirection Direction
        {
            get { return ViewState[DirectionString] == null ? SortDirection.Descending : (SortDirection)ViewState[DirectionString]; }
            set { ViewState[DirectionString] = value; }
        }

        protected static int CompareAccount(RdsServer server1, RdsServer server2)
        {
            return string.Compare(server1.FqdName, server2.FqdName);
        }

        protected void gvServers_RowCommand(object sender, GridViewCommandEventArgs e)
        {            
            if (e.CommandName == "ViewInfo")
            {        
                try
                {
                    ShowInfo(e.CommandArgument.ToString());                 
                }
                catch (System.Exception swallowedEx) when (!(swallowedEx is System.OutOfMemoryException) && !(swallowedEx is System.StackOverflowException) && !(swallowedEx is System.AccessViolationException))
                {
                    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
                }
            }
            else if (e.CommandName == "Restart")
            {
                Restart(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "ShutDown")
            {
                ShutDown(e.CommandArgument.ToString());
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {            
            if (OnRefreshClicked != null)
            {
                OnRefreshClicked(GetServers(), new EventArgs());                
            }                        
        }

        private void ShowInfo(string serverName)
        {
            ViewInfoModal.Show();
            var serverInfo = ES.Services.RDS.GetRdsServerInfo(PanelRequest.ItemID, serverName);
            litProcessor.Text = string.Format("{0}x{1} MHz", serverInfo.NumberOfCores, serverInfo.MaxClockSpeed);
            litLoadPercentage.Text = string.Format("{0}%", serverInfo.LoadPercentage);
            litMemoryAllocated.Text = string.Format("{0} MB", serverInfo.MemoryAllocatedMb);
            litFreeMemory.Text = string.Format("{0} MB", serverInfo.FreeMemoryMb);
            rpServerDrives.DataSource = serverInfo.Drives;
            rpServerDrives.DataBind(); 
        }

        private void Restart(string serverName)
        {
            ES.Services.RDS.RestartRdsServer(PanelRequest.ItemID, serverName);
            Response.Redirect(EditUrl("SpaceID", PanelSecurity.PackageId.ToString(), "rds_edit_collection", "CollectionId=" + PanelRequest.CollectionID, "ItemID=" + PanelRequest.ItemID));
        }

        private void ShutDown(string serverName)
        {
            ES.Services.RDS.ShutDownRdsServer(PanelRequest.ItemID, serverName);
            Response.Redirect(EditUrl("SpaceID", PanelSecurity.PackageId.ToString(), "rds_edit_collection", "CollectionId=" + PanelRequest.CollectionID, "ItemID=" + PanelRequest.ItemID));
        }
	}
}
