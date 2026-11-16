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
using System.Text;

namespace FuseCP.Portal
{
    public partial class SearchBox : FuseCPControlBase
    {
        public string AjaxData { get; set; }

        public string FilterColumn
        {
            get
            {
                return ddlFilterColumn.SelectedValue;
            }
            set
            {
                ddlFilterColumn.SelectedIndex = -1;
                ListItem li = ddlFilterColumn.Items.FindByValue(value);
                if (li != null)
                    li.Selected = true;
            }
        }

        public string FilterValue
        {
            get
            {
                string val = tbSearchText.Text.Trim();
                string valText = tbSearch.Text.Trim();
                if (valText.Length == 0)
                    val = valText;
                if (val.Length == 0)
                    val = tbSearch.Text.Trim();
                val = val.Replace("%", "");
                return "%" + val + "%";
            }
            set
            {
                if (value != null)
                {
                    value = value.Replace("%", "");
                    tbSearch.Text = value;
                    tbSearchText.Text = value;
                }
            }
        }

        // F-042: WFFC does not persist list items added in code to ViewState, so on
        // postback the filter column dropdown renders without items and, after an
        // UpdatePanel async postback rebuilds the __EVENTVALIDATION store from the
        // rendered (empty) control, the next full postback posting a filter column
        // fails event validation ("Invalid postback or callback argument").
        // The added criteria are remembered in this control's ViewState and re-applied
        // in LoadViewState (before post-data processing) so the dropdown has items
        // on every request.
        private const string CriteriaViewStateKey = "FilterColumnCriteria";

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            // Re-apply the criteria remembered in ViewState so the dropdown has
            // items on postback (see CriteriaViewStateKey comment above).
            ArrayList criteria = ViewState[CriteriaViewStateKey] as ArrayList;
            if (criteria != null)
            {
                foreach (string[] c in criteria)
                {
                    if (ddlFilterColumn.Items.FindByValue(c[0]) == null)
                        ddlFilterColumn.Items.Add(new ListItem(c[1], c[0]));
                }
            }
        }

        public void AddCriteria(string columnName, string columnTitle)
        {
            if (ddlFilterColumn.Items.FindByValue(columnName) == null)
                ddlFilterColumn.Items.Add(new ListItem(columnTitle, columnName));

            // Remember the criteria so LoadViewState can re-add them on postback.
            ArrayList criteria = ViewState[CriteriaViewStateKey] as ArrayList;
            if (criteria == null)
            {
                criteria = new ArrayList();
                ViewState[CriteriaViewStateKey] = criteria;
            }
            bool remembered = false;
            foreach (string[] c in criteria)
            {
                if (c[0] == columnName)
                {
                    remembered = true;
                    break;
                }
            }
            if (!remembered)
                criteria.Add(new string[] { columnName, columnTitle });
        }

        public string GetCriterias()
        {
            StringBuilder res = new StringBuilder();
            bool isFirst = true;
            foreach (ListItem itm in ddlFilterColumn.Items)
            {
                if (!isFirst)
                {
                    res.Append(", ");
                }

                res.Append('\'').Append(itm.Value).Append('\'');
                isFirst = false;
            }
            return res.ToString();
        }

        public override void Focus()
        {
            base.Focus();
            tbSearch.Focus();
        }

        protected void cmdSearch_Click(object sender, ImageClickEventArgs e)
        {
            if (tbObjectId.Text.Length > 0)
            {
                Response.Redirect(PortalUtils.GetUserHomePageUrl(Int32.Parse(tbObjectId.Text)));
            }
            else
            {
                String strText = tbSearchText.Text;
                if (strText.Length > 0)
                {
                    Response.Redirect(NavigatePageURL(PortalUtils.GetUserCustomersPageId(),
                        PortalUtils.USER_ID_PARAM, PanelSecurity.SelectedUserId.ToString(),
                        "FilterColumn=" + ddlFilterColumn.SelectedValue,
                        "FilterValue=" + Server.UrlEncode(FilterValue)));
                }
                else
                {
                    Response.Redirect(PortalUtils.NavigatePageURL(PortalUtils.GetObjectSearchPageId(),
                        PortalUtils.USER_ID_PARAM, PanelSecurity.SelectedUserId.ToString(),
                        "Query=" + Server.UrlEncode(tbSearch.Text),"FullType=Users"));
                }
            }
        }
    }
}
