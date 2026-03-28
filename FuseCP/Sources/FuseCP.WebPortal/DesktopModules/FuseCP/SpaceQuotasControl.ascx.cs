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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class SpaceQuotasControl : FuseCPControlBase
    {
        DataSet dsQuotas = null;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void BindQuotas(int packageId)
        {
            try
            {
                dsQuotas = ES.Services.Packages.GetPackageQuotas(packageId);
                DataTable groupTable = (dsQuotas != null && dsQuotas.Tables.Count > 0) ? dsQuotas.Tables[0] : null;
                DataTable quotaTable = (dsQuotas != null && dsQuotas.Tables.Count > 1) ? dsQuotas.Tables[1] : null;
                if (quotaTable != null)
                {
                    if (!quotaTable.Columns.Contains("QuotaAvailable"))
                        quotaTable.Columns.Add("QuotaAvailable", typeof(int));

                    foreach (DataRow r in quotaTable.Rows) r["QuotaAvailable"] = -1;
                }

                dlGroups.DataSource = groupTable;
                dlGroups.DataBind();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                Response.Write(HttpUtility.HtmlEncode(ex.ToString()));
            }
        }

        public bool IsGroupVisible(int groupId)
        {
            DataTable quotaTable = (dsQuotas != null && dsQuotas.Tables.Count > 1) ? dsQuotas.Tables[1] : null;
            if (quotaTable == null)
                return false;

            return new DataView(quotaTable, "GroupID=" + groupId, "", DataViewRowState.CurrentRows).Count > 0;
        }

        public DataView GetGroupQuotas(int groupId)
        {
            DataTable quotaTable = (dsQuotas != null && dsQuotas.Tables.Count > 1) ? dsQuotas.Tables[1] : null;
            if (quotaTable == null)
                return new DataView(new DataTable());

            return new DataView(quotaTable, "GroupID=" + groupId, "", DataViewRowState.CurrentRows);
        }

        public string GetQuotaTitle(string quotaName, object quotaDescription)
        {
            string description = (quotaDescription.GetType() == typeof(System.DBNull)) ? string.Empty : (string)quotaDescription;

            return quotaName.Contains("ServiceLevel") ? description
                                                      : GetSharedLocalizedString("Quota." + quotaName);
        }
    }
}
