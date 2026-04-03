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
using System.Web.UI.WebControls;

namespace FuseCP.Portal.SfB.UserControls
{
    public partial class SfBUserPlanSelector : FuseCPControlBase
    {

        private string planToSelect;

        public string planId
        {
                        
            get {
                if (ddlPlan.Items.Count == 0) return "";
                return ddlPlan.SelectedItem.Value; 
            }
            set
            {
                planToSelect = value;
                ListItem selectedPlan = ddlPlan.Items.Cast<ListItem>().FirstOrDefault(li => li.Value == value);
                if (selectedPlan != null)
                {
                    ddlPlan.ClearSelection();
                    selectedPlan.Selected = true;
                }
            }
        }

        public int plansCount
		{
			get
			{
                return this.ddlPlan.Items.Count;
			}
		}


        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
                BindPlans();
			}
        }

        public FuseCP.Providers.HostedSolution.SfBUserPlan plan
        {
            get
            {
                FuseCP.Providers.HostedSolution.SfBUserPlan[] plans = ES.Services.SfB.GetSfBUserPlans(PanelRequest.ItemID);
                return plans.FirstOrDefault(planitem => planitem.SfBUserPlanId.ToString() == planId);
            }
        }

        private void BindPlans()
		{
            FuseCP.Providers.HostedSolution.SfBUserPlan[] plans = ES.Services.SfB.GetSfBUserPlans(PanelRequest.ItemID);

            ddlPlan.Items.AddRange(plans.Select(localPlan => new ListItem
            {
                Text = localPlan.SfBUserPlanName,
                Value = localPlan.SfBUserPlanId.ToString(),
                Selected = localPlan.IsDefault
            }).ToArray());

            ListItem selectedPlan = ddlPlan.Items.Cast<ListItem>().FirstOrDefault(li => li.Value == planToSelect);
            if (selectedPlan != null)
            {
                ddlPlan.ClearSelection();
                selectedPlan.Selected = true;
            }

		}
    }
}
