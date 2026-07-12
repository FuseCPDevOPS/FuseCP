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
using System.Web;
using System.Web.UI.WebControls;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class HostingPlansEditPlan : FuseCPModuleBase
    {
        private const string InlineMessageHiddenCssClass = "alert d-none mb-3 d-block";
        private const string InlineMessageBaseCssClass = "alert mb-3 d-block";

        private int CurrentPlanPackageId
        {
            get { return ViewState["CurrentPlanPackageId"] != null ? (int)ViewState["CurrentPlanPackageId"] : 0; }
            set { ViewState["CurrentPlanPackageId"] = value; }
        }

        private int CurrentPlanServerId
        {
            get { return ViewState["CurrentPlanServerId"] != null ? (int)ViewState["CurrentPlanServerId"] : 0; }
            set { ViewState["CurrentPlanServerId"] = value; }
        }

		protected bool ShouldCopyCurrentHostingPlan()
		{
			return (HttpContext.Current.Request.QueryString["TargetAction"] == "Copy");
		}

        protected void Page_Load(object sender, EventArgs e)
        {
			btnDelete.Visible = (PanelRequest.PlanID > 0) && (!ShouldCopyCurrentHostingPlan());

            if (!IsPostBack)
                ClearInlineMessage();

			bool isUserAdmin = PanelSecurity.SelectedUser.Role == UserRole.Administrator;
			ConfigureTargetSelection(isUserAdmin);
            RestoreCurrentTargetSelection();

            if (!IsPostBack)
            {
                try
                {
                    BindPlan();
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("PLAN_GET_PLAN", ex);
                    return;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
                return;

            // Recreate list and quota controls early so posted values are loaded into dynamic controls.
            BindServers();
            BindSpaces();

            int serverId = Utils.ParseInt(Request.Form[ddlServer.UniqueID], 0);
            int packageId = Utils.ParseInt(Request.Form[ddlSpace.UniqueID], -1);

            if (PanelRequest.PlanID > 0 && (serverId <= 0 || packageId <= 0))
            {
                HostingPlanInfo existingPlan = ES.Services.Packages.GetHostingPlan(PanelRequest.PlanID);
                if (existingPlan != null)
                {
                    if (serverId <= 0)
                        serverId = existingPlan.ServerId;

                    if (packageId <= 0)
                        packageId = existingPlan.PackageId > 0 ? existingPlan.PackageId : -1;
                }
            }

            hostingPlansQuotas.BindPlanQuotas(packageId, PanelRequest.PlanID, serverId);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Keep action buttons interactive on the hosting plan editor.
            btnSave.Enabled = true;
            btnSave.CssClass = RemoveDisabledCssClass(btnSave.CssClass);

            if (btnDelete.Visible)
            {
                btnDelete.Enabled = true;
                btnDelete.CssClass = RemoveDisabledCssClass(btnDelete.CssClass);
            }
        }

        private static string RemoveDisabledCssClass(string cssClass)
        {
            if (String.IsNullOrWhiteSpace(cssClass))
                return cssClass;

            string[] tokens = cssClass.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] filteredTokens = Array.FindAll(tokens, token => !String.Equals(token, "disabled", StringComparison.OrdinalIgnoreCase));
            return String.Join(" ", filteredTokens);
        }

        private void BindServers()
        {
            ddlServer.DataSource = ES.Services.Servers.GetRawAllServers();
            ddlServer.DataBind();
            ddlServer.Items.Insert(0, new ListItem("<Select Server>", ""));
        }

        private void BindSpaces()
        {
            ddlSpace.DataSource = ES.Services.Packages.GetMyPackages(PanelSecurity.SelectedUserId);
            ddlSpace.DataBind();
            ddlSpace.Items.Insert(0, new ListItem("<Select Space>", ""));
        }

        private void BindPlan()
        {
            bool isUserAdmin = PanelSecurity.SelectedUser.Role == UserRole.Administrator;

            BindServers();
            BindSpaces();

            if (PanelRequest.PlanID == 0)
            {
                // new plan
                BindQuotas();
                return;
            }

            HostingPlanInfo plan = ES.Services.Packages.GetHostingPlan(PanelRequest.PlanID);
            if (plan == null)
            {
                // plan not found
                RedirectBack();
                return;
            }

			CurrentPlanPackageId = plan.PackageId;
			CurrentPlanServerId = plan.ServerId;

			if (ShouldCopyCurrentHostingPlan())
			{
				plan.PlanId = 0;
				plan.PlanName = "Copy of " + plan.PlanName;
			}

            // bind plan
            txtPlanName.Text = PortalAntiXSS.DecodeOld(plan.PlanName);
            txtPlanDescription.Text = PortalAntiXSS.DecodeOld(plan.PlanDescription);
            //chkAvailable.Checked = plan.Available;

            //txtSetupPrice.Text = plan.SetupPrice.ToString("0.00");
            //txtRecurringPrice.Text = plan.RecurringPrice.ToString("0.00");
            //txtRecurrenceLength.Text = plan.RecurrenceLength.ToString();
            //Utils.SelectListItem(ddlRecurrenceUnit, plan.RecurrenceUnit);

            Utils.SelectListItem(ddlServer, plan.ServerId);
            Utils.SelectListItem(ddlSpace, plan.PackageId);

            // bind quotas
            BindQuotas();
        }

        private void ConfigureTargetSelection(bool isUserAdmin)
        {
            // Only one target selector is applicable at a time.
            rowTargetServer.Visible = isUserAdmin;
            rowTargetSpace.Visible = !isUserAdmin;

            // Target validation is handled explicitly in SavePlan to avoid stale client validation during edit flows.
            valRequireServer.Enabled = false;
            valRequireSpace.Enabled = false;
        }

        private void RestoreCurrentTargetSelection()
        {
            if (PanelRequest.PlanID <= 0 || ShouldCopyCurrentHostingPlan())
                return;

            if (ddlServer.SelectedIndex <= 0 && CurrentPlanServerId > 0)
                Utils.SelectListItem(ddlServer, CurrentPlanServerId);

            if (ddlSpace.SelectedIndex <= 0 && CurrentPlanPackageId > 0)
                Utils.SelectListItem(ddlSpace, CurrentPlanPackageId);
        }

        private void BindQuotas()
        {
            int serverId = Utils.ParseInt(ddlServer.SelectedValue, 0);
            int packageId = Utils.ParseInt(ddlSpace.SelectedValue, -1);
            hostingPlansQuotas.BindPlanQuotas(packageId, PanelRequest.PlanID, serverId);
        }

        private void ClearInlineMessage()
        {
            lblMessage.Text = String.Empty;
            lblMessage.CssClass = InlineMessageHiddenCssClass;
            lblMessage.Attributes.Remove("role");
        }

        private void ShowInlineMessage(string message, string bootstrapAlertType = "warning")
        {
            lblMessage.Text = PortalAntiXSS.Encode(message);
            lblMessage.CssClass = String.Format("{0} alert-{1}", InlineMessageBaseCssClass, bootstrapAlertType);
            lblMessage.Attributes["role"] = "alert";
        }

        private void SavePlan()
        {
            ClearInlineMessage();

            if (!Page.IsValid)
                return;

            // gather form info
            HostingPlanInfo plan = new HostingPlanInfo();
            plan.UserId = PanelSecurity.SelectedUserId;
            plan.PlanId = PanelRequest.PlanID;
            plan.IsAddon = false;
            plan.PlanName = txtPlanName.Text;
            plan.PlanDescription = txtPlanDescription.Text;
            plan.Available = true; // always available

            plan.SetupPrice = 0;
            plan.RecurringPrice = 0;
            plan.RecurrenceLength = 1;
            plan.RecurrenceUnit = 2; // month

            bool isNewOrCopy = (PanelRequest.PlanID == 0) || ShouldCopyCurrentHostingPlan();
            bool isAdmin = PanelSecurity.SelectedUser.Role == UserRole.Administrator;

            if (isNewOrCopy)
            {
                if (isAdmin && String.IsNullOrEmpty(ddlServer.SelectedValue))
                {
                    ShowInlineMessage("Select target server", "warning");
                    return;
                }

                if (!isAdmin && String.IsNullOrEmpty(ddlSpace.SelectedValue))
                {
                    ShowInlineMessage("Select target space", "warning");
                    return;
                }
            }

            plan.PackageId = Utils.ParseInt(ddlSpace.SelectedValue, 0);
            plan.ServerId = Utils.ParseInt(ddlServer.SelectedValue, 0);

            if ((PanelRequest.PlanID > 0) && !ShouldCopyCurrentHostingPlan() && (plan.PackageId == 0 || plan.ServerId == 0))
            {
                HostingPlanInfo existingPlan = ES.Services.Packages.GetHostingPlan(PanelRequest.PlanID);
                if (existingPlan != null)
                {
                    if (plan.PackageId == 0)
                        plan.PackageId = existingPlan.PackageId;

                    if (plan.ServerId == 0)
                        plan.ServerId = existingPlan.ServerId;
                }
            }

            // if this is non-admin
            // get server info from parent package
            if (PanelSecurity.EffectiveUser.Role != UserRole.Administrator)
            {
                try
                {
                    PackageInfo package = ES.Services.Packages.GetPackage(plan.PackageId);
                    if (package != null)
                        plan.ServerId = package.ServerId;
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("PACKAGE_GET_PACKAGE", ex);
                    return;
                }
            }

            plan.Groups = hostingPlansQuotas.Groups;
            plan.Quotas = hostingPlansQuotas.Quotas;

            if ((PanelRequest.PlanID > 0) && !ShouldCopyCurrentHostingPlan() && plan.Groups.Length == 0)
            {
                ShowInlineMessage("No quotas were submitted. Please reload the page and try saving again.", "warning");
                return;
            }

            if ((PanelRequest.PlanID == 0) || ShouldCopyCurrentHostingPlan())
            {
                // new plan
                try
                {
                    int planId = ES.Services.Packages.AddHostingPlan(plan);
                    if (planId < 0)
                    {
                        ShowResultMessage(planId);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("PLAN_ADD_PLAN", ex);
                    return;
                }
            }
            else
            {
                // update plan
                try
                {
                    PackageResult result = ES.Services.Packages.UpdateHostingPlan(plan);
                    if (result.Result < 0)
                    {
                        ShowResultMessage(result.Result);
                        ShowInlineMessage(GetExceedingQuotasMessage(result.ExceedingQuotas), "danger");
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("PLAN_UPDATE_PLAN", ex);
                    return;
                }
            }

            // redirect
            RedirectBack();
        }

        private void DeletePlan()
        {
            try
            {
                int result = ES.Services.Packages.DeleteHostingPlan(PanelRequest.PlanID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("PLAN_DELETE_PLAN", ex);
                return;
            }

            // redirect
            RedirectBack();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SavePlan();
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectBack();
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeletePlan();
        }

        protected void planTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindQuotas();
        }

        private void RedirectBack()
        {
            Response.Redirect(NavigateURL(PortalUtils.USER_ID_PARAM, PanelSecurity.SelectedUserId.ToString()));
        }
    }
}
