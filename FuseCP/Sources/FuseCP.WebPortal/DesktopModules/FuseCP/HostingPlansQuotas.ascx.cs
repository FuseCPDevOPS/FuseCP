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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class HostingPlansQuotas : FuseCPControlBase
    {
        DataSet dsQuotas = null;

        public bool IsPlan = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ToggleQuotaControls();
            }
        }

        public void BindPackageQuotas(int packageId)
        {
            try
            {
                dsQuotas = ES.Services.Packages.GetPackageQuotasForEdit(packageId);
                DataTable groupTable = (dsQuotas != null && dsQuotas.Tables.Count > 0) ? dsQuotas.Tables[0] : null;
                dlGroups.DataSource = groupTable;
                dlGroups.DataBind();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                System.Diagnostics.Trace.TraceWarning("Exception while binding package quotas: " + ex.Message);
                Response.Write(HttpUtility.HtmlEncode("An unexpected error occurred while loading quotas."));
            }

            ToggleQuotaControls();
        }

        public void BindPlanQuotas(int packageId, int planId, int serverId)
        {
            try
            {
                dsQuotas = ES.Services.Packages.GetHostingPlanQuotas(packageId, planId, serverId);
                DataTable groupTable = (dsQuotas != null && dsQuotas.Tables.Count > 0) ? dsQuotas.Tables[0] : null;
                if (groupTable != null)
                    groupTable.DefaultView.RowFilter = "ParentEnabled =True";

                dlGroups.DataSource = groupTable != null ? groupTable.DefaultView : null;
                dlGroups.DataBind();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                System.Diagnostics.Trace.TraceWarning("Exception while binding hosting plan quotas: " + ex.Message);
                Response.Write(HttpUtility.HtmlEncode("An unexpected error occurred while loading quotas."));
            }

            ToggleQuotaControls();
        }

        private void ToggleQuotaControls()
        {
            foreach (RepeaterItem item in dlGroups.Items)
            {
                CheckBox chkEnabled = (CheckBox)item.FindControl("chkEnabled");

                CheckBox chkCountDiskspace = (CheckBox)item.FindControl("chkCountDiskspace");
                CheckBox chkCountBandwidth = (CheckBox)item.FindControl("chkCountBandwidth");
                chkCountDiskspace.Enabled = chkEnabled.Checked && IsPlan;
                chkCountBandwidth.Enabled = chkEnabled.Checked && IsPlan;

                // iterate quotas
                Control quotaPanel = item.FindControl("QuotaPanel");
                quotaPanel.Visible = chkEnabled.Checked;

                DataList dlQuotas = (DataList)item.FindControl("dlQuotas");
                foreach (DataListItem quotaItem in dlQuotas.Items)
                {
                    if (!chkEnabled.Checked)
                    {
                        QuotaEditor quotaEditor = (QuotaEditor)quotaItem.FindControl("quotaEditor");
                        quotaEditor.QuotaValue = 0;
                    }
                }

                // hide group if quotas == 0
                Control groupPanel = item.FindControl("GroupPanel");
                groupPanel.Visible = (IsPlan || dlQuotas.Items.Count > 0);
            }
        }

        List<HostingPlanGroupInfo> groups;
        List<HostingPlanQuotaInfo> quotas;

        public HostingPlanGroupInfo[] Groups
        {
            get
            {
                if (groups == null)
                    CollectFormData();

                return groups.ToArray();
            }
        }

        public HostingPlanQuotaInfo[] Quotas
        {
            get
            {
                if (quotas == null)
                    CollectFormData();

                return quotas.ToArray();
            }
        }

        public void CollectFormData()
        {
            groups = new List<HostingPlanGroupInfo>();
            quotas = new List<HostingPlanQuotaInfo>();

            // gather info
            foreach (RepeaterItem item in dlGroups.Items)
            {
                Literal litGroupId = (Literal)item.FindControl("groupId");
                CheckBox chkEnabled = (CheckBox)item.FindControl("chkEnabled");
                CheckBox chkCountDiskspace = (CheckBox)item.FindControl("chkCountDiskspace");
                CheckBox chkCountBandwidth = (CheckBox)item.FindControl("chkCountBandwidth");

                if (!chkEnabled.Checked)
                    continue; // disabled group

                HostingPlanGroupInfo group = new HostingPlanGroupInfo();
                group.GroupId = Utils.ParseInt(litGroupId.Text, 0);
                group.Enabled = chkEnabled.Checked;
                group.CalculateDiskSpace = chkCountDiskspace.Checked;
                group.CalculateBandwidth = chkCountBandwidth.Checked;
                groups.Add(group);

                // iterate quotas
                DataList dlQuotas = (DataList)item.FindControl("dlQuotas");
                foreach (DataListItem quotaItem in dlQuotas.Items)
                {
                    QuotaEditor quotaEditor = (QuotaEditor)quotaItem.FindControl("quotaEditor");
                    HostingPlanQuotaInfo quota = new HostingPlanQuotaInfo();
                    quota.QuotaId = quotaEditor.QuotaId;
                    quota.QuotaValue = quotaEditor.QuotaValue;
                    quotas.Add(quota);
                }
            }
        }

        /*
        public void SavePlanQuotas(int planId)
        {
            CollectFormData();

            // update plan quotas
            ES.Packages.UpdateHostingPlanQuotas(planId, groups.ToArray(), quotas.ToArray());
        }

        public void SavePackageQuotas(int packageId)
        {
            CollectFormData();

            // update plan quotas
            ES.Packages.UpdatePackageQuotas(packageId, groups.ToArray(), quotas.ToArray());
        }
         * */

        public DataView GetGroupQuotas(int groupId)
        {
            return new DataView(dsQuotas.Tables[1], "GroupID=" + groupId, "", DataViewRowState.CurrentRows);
        }

        public string GetSharedLocalizedStringNotEmpty(string resourceKey, object resourceDescription)
        {
            string result = GetSharedLocalizedString("Quota." + resourceKey);
            if (string.IsNullOrEmpty(result))
            {
                result = resourceKey;

                string resourceDescriptionString = resourceDescription as string;
                if (!string.IsNullOrEmpty(resourceDescriptionString))
                {
                    result = resourceDescriptionString;
                }
                else if (result.IndexOf('.') > 0 && result.Substring(result.IndexOf('.')).Length > 1)
                {
                    // drop Quota name prefix
                    // HeliconZoo.python -> python

                    result = result.Substring(result.IndexOf('.')+1);
                }
            }

            return result;
        }
    }
}
