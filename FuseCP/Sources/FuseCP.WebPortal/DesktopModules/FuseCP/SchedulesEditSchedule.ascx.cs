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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using FuseCP.EnterpriseServer;
using FuseCP.EnterpriseServer.Base.Scheduling;
using FuseCP.Portal.Code.Framework;

namespace FuseCP.Portal
{
    public partial class SchedulesEditSchedule : FuseCPModuleBase
    {
        private static readonly string ScheduleViewEnvironment = "ASP.NET";
        private const string SchedulerWeightParameterId = "SCHEDULER_WEIGHT";
        private const string SchedulerAffinityParameterId = "SCHEDULER_AFFINITY";
        private static readonly string[] SchedulerWeightAliases = { "SCHEDULER_WEIGHT", "TASK_WEIGHT", "WEIGHT" };
        private static readonly string[] SchedulerAffinityAliases = { "SCHEDULER_AFFINITY", "SERVER_ID", "AFFINITY" };
        private const string SchedulerExecutionModeParameterId = "SCHEDULER_EXECUTION_MODE";
        private static readonly string[] SchedulerExecutionModeAliases = { "SCHEDULER_EXECUTION_MODE", "EXECUTION_MODE", "SCHEDULER_MODE" };
        private const string SchedulerParallelismModeParameterId = "SCHEDULER_PARALLELISM_MODE";
        private const string SchedulerParallelismMaxParameterId = "SCHEDULER_PARALLELISM_MAX";
        private static readonly string[] SchedulerParallelismModeAliases = { "SCHEDULER_PARALLELISM_MODE", "PARALLELISM_MODE", "SCHEDULER_TASK_PARALLELISM_MODE" };
        private static readonly string[] SchedulerParallelismMaxAliases = { "SCHEDULER_PARALLELISM_MAX", "PARALLELISM_MAX", "SCHEDULER_TASK_PARALLELISM_MAX" };

        private const string ExecutionModeAuto = "AUTO";
        private const string ExecutionModeServerPreferred = "SERVER_PREFERRED";
        private const string ExecutionModeEnterpriseOnly = "ENTERPRISE_ONLY";
        private const string ParallelismModeAuto = "AUTO";
        private const string ParallelismModeManual = "MANUAL";

        private ISchedulerTaskView configurationView;
        private string cachedTaskIdsToLoad = String.Empty;

        private Control FindControlRecursive(Control rootControl, string controlID)
        {
            if (rootControl == null || String.IsNullOrEmpty(controlID))
                return null;

            if (rootControl.ID == controlID)
                return rootControl;

            foreach (Control controlToSearch in rootControl.Controls)
            {
                Control foundControl = FindControlRecursive(controlToSearch, controlID);
                if (foundControl != null)
                    return foundControl;
            }

            return null;
        }

        private HtmlTableRow ExecutionModeRow => FindControlRecursive(this, "rowExecutionMode") as HtmlTableRow;

        private DropDownList ExecutionModeDropDown => FindControlRecursive(this, "ddlExecutionMode") as DropDownList;

        private DropDownList ParallelismModeDropDown => FindControlRecursive(this, "ddlParallelismMode") as DropDownList;

        private TextBox ParallelismMaxTextBox => FindControlRecursive(this, "txtParallelismMax") as TextBox;

        public int PackageId
        {
            get { return (int)ViewState["PackageId"]; }
            set { ViewState["PackageId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnsureScheduleAccess())
                return;

            btnDelete.Visible = (PanelRequest.ScheduleID > 0);
            rowAdvancedScheduler.Visible = PanelSecurity.SelectedUser.Role != UserRole.User;
            HtmlTableRow executionModeRow = ExecutionModeRow;
            if (executionModeRow != null)
                executionModeRow.Visible = PanelSecurity.LoggedUser.Role == UserRole.Administrator;

            this.ControlToLoad.Value = this.cachedTaskIdsToLoad;
            if (!IsPostBack)
            {
                try
                {
                    // bind controls
                    BindTasks();

                    // bind schedule
                    BindSchedule();
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_INIT_FORM", ex);
                    return;
                }
            }
        }

        private bool EnsureScheduleAccess()
        {
            if (PanelRequest.ScheduleID <= 0)
                return true;

            ScheduleInfo schedule = ES.Services.Scheduler.GetSchedule(PanelRequest.ScheduleID);
            if (schedule == null)
            {
                ShowErrorMessage("ACCESS_DENIED");
                RedirectSpaceHomePage();
                return false;
            }

            if (PanelSecurity.SelectedUser.Role == UserRole.Administrator)
                return true;

            if (schedule.PackageId != PanelSecurity.PackageId)
            {
                ShowErrorMessage("ACCESS_DENIED");
                RedirectSpaceHomePage();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Overridden. Dynamically loads configuration view.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Make sure control is loaded before view state and post back data are loaded.
            string taskIdsToLoad = HttpContext.Current.Request.Params[this.ControlToLoad.Name];
            if (taskIdsToLoad == null)
            {
                taskIdsToLoad = String.Empty;
            }

            string selectedTaskId = HttpContext.Current.Request.Params[this.ddlTaskType.UniqueID];
            ScheduleInfo sc = (PanelRequest.ScheduleID != 0)
                ? ES.Services.Scheduler.GetSchedule(PanelRequest.ScheduleID)
                : null;
            if (!IsPostBack && PanelRequest.ScheduleID != 0 && sc != null)
            {
                        selectedTaskId = sc.TaskId;
            }

            List<string> tasksListToLoad = new List<string>(taskIdsToLoad.Split(new char[] { ';' }));
            if (!String.IsNullOrEmpty(selectedTaskId) && !tasksListToLoad.Contains(selectedTaskId))
            {
                    tasksListToLoad.Add(selectedTaskId);
            }

            foreach (string taskId in tasksListToLoad)
            {
                ISchedulerTaskView view = LoadScheduleTaskConfigurationView(taskId, taskId == selectedTaskId);
                if (taskId == selectedTaskId)
                {
                    this.configurationView = view;
                }
            }

            cachedTaskIdsToLoad = String.Join(";", tasksListToLoad.ToArray());
        }

        /// <summary>
        /// Loads control that is intended to provide user ability to configure schedule task.
        /// </summary>
        /// <remarks>
        /// Returns loaded configuration view.
        /// </remarks>
        private ISchedulerTaskView LoadScheduleTaskConfigurationView(string taskId, bool visible)
        {
            //this.TaskParametersPlaceHolder.Controls.Clear();

            string selectedTaskId = taskId;

            if (!String.IsNullOrEmpty(selectedTaskId))
            {
                // Try to find view configuration
                ScheduleTaskViewConfiguration aspNetEnvironmentViewConfiguration = ES.Services.Scheduler.GetScheduleTaskViewConfiguration(selectedTaskId, ScheduleViewEnvironment);
                // If no configuration found ignore view 
                if (aspNetEnvironmentViewConfiguration == null)
                {
                    return null;
                }
                // Description contains relative path to control to be loaded.
                Control view = this.LoadControl(aspNetEnvironmentViewConfiguration.Description);
                if (!(view is ISchedulerTaskView))
                {
                    // The view does not provide ability to set and get parameters.
                    return null;
                }
                view.ID = taskId;
                view.Visible = visible;
                view.EnableTheming = true;
                this.TaskParametersPlaceHolder.Controls.Add(view);
                return (ISchedulerTaskView)view;
            }
            return null;
        }

        private void BindTasks()
        {
            ScheduleTaskInfo[] tasks = ES.Services.Scheduler.GetScheduleTasks();

            ddlTaskType.Items.Add(new ListItem("<Select Task>", ""));

            foreach (ScheduleTaskInfo task in tasks)
            {
                string localizedTaskName = GetSharedLocalizedString(Utils.ModuleName, "SchedulerTask." + task.TaskId);
                if (localizedTaskName == null)
                    localizedTaskName = task.TaskId;

                ddlTaskType.Items.Add(new ListItem(localizedTaskName, task.TaskId));
            }
        }

        private void BindSchedule()
        {
            txtStartDate.Text = DateTime.Now.ToString("d");
            timeFromTime.SelectedValue = new DateTime(2000, 1, 1, 0, 0, 0);
            timeToTime.SelectedValue = new DateTime(2000, 1, 1, 23, 59, 59);
            intMaxExecutionTime.Interval = 3600;

            if (PanelRequest.ScheduleID == 0)
            {
                ApplyPackageContextRestrictions(PanelSecurity.PackageId);
                PackageId = PanelSecurity.PackageId;
            }
            else
            {

                ScheduleInfo sc = ES.Services.Scheduler.GetSchedule(PanelRequest.ScheduleID);
                if (sc == null)
                    return;

                ApplyPackageContextRestrictions(sc.PackageId);
                PackageId = sc.PackageId;

                txtTaskName.Text = sc.ScheduleName;

                Utils.SelectListItem(ddlTaskType, sc.TaskId);

                Utils.SelectListItem(ddlSchedule, sc.ScheduleTypeId);
                timeFromTime.SelectedValue = sc.FromTime;
                timeToTime.SelectedValue = sc.ToTime;

                timeStartTime.SelectedValue = sc.StartTime;
                intInterval.Interval = sc.Interval;

                // run once
                if (ddlSchedule.SelectedIndex == 3)
                {
                    txtStartDate.Text = sc.StartTime.ToString("d");
                }

                txtWeekDay.Text = sc.WeekMonthDay.ToString();
                txtMonthDay.Text = sc.WeekMonthDay.ToString();

                chkEnabled.Checked = sc.Enabled;
                Utils.SelectListItem(ddlPriority, sc.PriorityId);
                intMaxExecutionTime.Interval = sc.MaxExecutionTime;
            }


            // bind schedule parameters
            BindScheduleParameters();

            // toggle
            ToggleControls();
        }

        private void ApplyPackageContextRestrictions(int packageId)
        {
            // load context
            PackageContext cntx = PackagesHelper.GetCachedPackageContext(packageId);

            bool intervalTasksAllowed = cntx.Quotas.TryGetValue(Quotas.OS_SCHEDULEDINTERVALTASKS, out var intervalTasksQuota)
                && intervalTasksQuota.QuotaAllocatedValue != 0;
            if (!intervalTasksAllowed)
                ddlSchedule.Items.Remove(ddlSchedule.Items.FindByValue("Interval"));

            // check if this an admin
            if (PanelSecurity.LoggedUser.Role != UserRole.Administrator)
            {
                // remove "high" priorities
                ddlPriority.Items.Remove(ddlPriority.Items.FindByValue("Highest"));
                ddlPriority.Items.Remove(ddlPriority.Items.FindByValue("AboveNormal"));
                ddlPriority.Items.Remove(ddlPriority.Items.FindByValue("Normal"));
            }
        }

        /// <summary>
        /// Binds schedule task parameters to configuration view.
        /// </summary>
        private void BindScheduleParameters()
        {
            ScheduleTaskParameterInfo[] parameters = ES.Services.Scheduler.GetScheduleParameters(ddlTaskType.SelectedValue,
                PanelRequest.ScheduleID);

            BindAdvancedSchedulerParameters(parameters);
            parameters = FilterAdvancedSchedulerParameters(parameters);

            gvTaskParameters.DataSource = parameters;
            gvTaskParameters.DataBind();

            if (this.configurationView != null)
            {
                this.configurationView.SetParameters(parameters);
            }
        }

        private static bool IsParameterId(string parameterId, IEnumerable<string> aliases)
        {
            if (String.IsNullOrEmpty(parameterId))
                return false;

            foreach (string alias in aliases)
            {
                if (String.Equals(parameterId, alias, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static string FindParameterValue(ScheduleTaskParameterInfo[] parameters, IEnumerable<string> aliases)
        {
            if (parameters == null)
                return String.Empty;

            foreach (ScheduleTaskParameterInfo parameter in parameters)
            {
                if (parameter == null)
                    continue;

                if (IsParameterId(parameter.ParameterId, aliases))
                    return parameter.ParameterValue ?? String.Empty;
            }

            return String.Empty;
        }

        private void BindAdvancedSchedulerParameters(ScheduleTaskParameterInfo[] parameters)
        {
            txtSchedulerWeight.Text = FindParameterValue(parameters, SchedulerWeightAliases);
            txtSchedulerAffinity.Text = FindParameterValue(parameters, SchedulerAffinityAliases);

            string mode = NormalizeExecutionMode(FindParameterValue(parameters, SchedulerExecutionModeAliases));
            DropDownList executionModeDropDown = ExecutionModeDropDown;
            if (executionModeDropDown != null)
                Utils.SelectListItem(executionModeDropDown, mode);

            string parallelismMode = NormalizeParallelismMode(FindParameterValue(parameters, SchedulerParallelismModeAliases));
            DropDownList parallelismModeDropDown = ParallelismModeDropDown;
            if (parallelismModeDropDown != null)
                Utils.SelectListItem(parallelismModeDropDown, parallelismMode);

            TextBox parallelismMaxTextBox = ParallelismMaxTextBox;
            if (parallelismMaxTextBox != null)
                parallelismMaxTextBox.Text = NormalizeParallelismMax(FindParameterValue(parameters, SchedulerParallelismMaxAliases));
        }

        private static ScheduleTaskParameterInfo[] FilterAdvancedSchedulerParameters(ScheduleTaskParameterInfo[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return parameters;

            List<ScheduleTaskParameterInfo> filtered = new List<ScheduleTaskParameterInfo>();
            foreach (ScheduleTaskParameterInfo parameter in parameters)
            {
                if (parameter == null)
                    continue;

                bool isAdvancedScheduler = IsParameterId(parameter.ParameterId, SchedulerWeightAliases)
                    || IsParameterId(parameter.ParameterId, SchedulerAffinityAliases)
                    || IsParameterId(parameter.ParameterId, SchedulerExecutionModeAliases)
                    || IsParameterId(parameter.ParameterId, SchedulerParallelismModeAliases)
                    || IsParameterId(parameter.ParameterId, SchedulerParallelismMaxAliases);

                if (!isAdvancedScheduler)
                    filtered.Add(parameter);
            }

            return filtered.ToArray();
        }

        private static void RemoveParametersByAliases(List<ScheduleTaskParameterInfo> parameters, IEnumerable<string> aliases)
        {
            if (parameters == null)
                return;

            for (int index = parameters.Count - 1; index >= 0; index--)
            {
                ScheduleTaskParameterInfo existing = parameters[index];
                if (existing == null || IsParameterId(existing.ParameterId, aliases))
                    parameters.RemoveAt(index);
            }
        }

        private static void UpsertParameter(List<ScheduleTaskParameterInfo> parameters, string parameterId, string parameterValue, IEnumerable<string> aliases)
        {
            if (parameters == null || String.IsNullOrWhiteSpace(parameterId))
                return;

            RemoveParametersByAliases(parameters, aliases);

            if (!String.IsNullOrWhiteSpace(parameterValue))
            {
                parameters.Add(new ScheduleTaskParameterInfo
                {
                    ParameterId = parameterId,
                    ParameterValue = parameterValue.Trim()
                });
            }
        }

        protected void gvTaskParameters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ParameterEditor txtValue = (ParameterEditor)e.Row.FindControl("txtValue");
            if (txtValue == null)
                return;

            ScheduleTaskParameterInfo prm = (ScheduleTaskParameterInfo)e.Row.DataItem;
            txtValue.DataType = prm.DataTypeId;
            txtValue.DefaultValue = prm.DefaultValue;
            txtValue.Value = prm.ParameterValue;
        }

        public string GetHistoryFinishTime(DateTime dt)
        {
            return (dt == DateTime.MinValue) ? "" : dt.ToString();
        }

        private void ToggleControls()
        {
            tblWeekly.Visible = (ddlSchedule.SelectedIndex == 1);
            tblMonthly.Visible = (ddlSchedule.SelectedIndex == 2);
            tblOneTime.Visible = (ddlSchedule.SelectedIndex == 3);
            tblInterval.Visible = (ddlSchedule.SelectedIndex == 4);
            timeStartTime.Enabled = (ddlSchedule.SelectedIndex != 4);
        }

        protected void ddlSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleControls();
        }

        protected void ddlTaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.configurationView = this.LoadScheduleTaskConfigurationView(this.ddlTaskType.SelectedValue);
            BindScheduleParameters();
        }

        private void SaveTask()
        {
            if (!EnsureScheduleAccess())
                return;

            // gather form parameters
            ScheduleInfo sc = new ScheduleInfo();
            sc.ScheduleId = PanelRequest.ScheduleID;
            sc.ScheduleName = txtTaskName.Text.Trim();
            sc.TaskId = ddlTaskType.SelectedValue;

            sc.PackageId = PanelSecurity.PackageId;

            sc.ScheduleTypeId = ddlSchedule.SelectedValue;
            sc.FromTime = timeFromTime.SelectedValue;
            sc.ToTime = timeToTime.SelectedValue;

            sc.StartTime = timeStartTime.SelectedValue;
            sc.Interval = intInterval.Interval;

            // check maximum interval
            // load context
            PackageContext cntx = PackagesHelper.GetCachedPackageContext(PackageId);
if (cntx.Quotas.TryGetValue(Quotas.OS_MINIMUMTASKINTERVAL, out var _ckv))
            {
                int minInterval = _ckv.QuotaAllocatedValue;
                if (minInterval != -1 && sc.Interval < (minInterval * 60))
                    sc.Interval = (minInterval * 60);
            }

            // run once
            if (ddlSchedule.SelectedIndex == 3)
            {
                DateTime tm = timeStartTime.SelectedValue;
                DateTime dt = DateTime.Parse(txtStartDate.Text);
                DateTime startTime = new DateTime(dt.Year, dt.Month, dt.Day, tm.Hour, tm.Minute, tm.Second);
                sc.StartTime = startTime;
            }

            sc.WeekMonthDay = Utils.ParseInt(txtWeekDay.Text, 0);
            if (ddlSchedule.SelectedIndex == 2)
                sc.WeekMonthDay = Utils.ParseInt(txtMonthDay.Text, 0);


            sc.Enabled = chkEnabled.Checked;
            sc.PriorityId = ddlPriority.SelectedValue;
            sc.HistoriesNumber = 0;
            sc.MaxExecutionTime = intMaxExecutionTime.Interval;

            // gather parameters
            List<ScheduleTaskParameterInfo> parameters = new List<ScheduleTaskParameterInfo>();
            foreach (GridViewRow row in gvTaskParameters.Rows)
            {
                ParameterEditor txtValue = (ParameterEditor)row.FindControl("txtValue");
                if (txtValue == null)
                    continue;

                string prmId = (string)gvTaskParameters.DataKeys[row.RowIndex][0];

                ScheduleTaskParameterInfo parameter = new ScheduleTaskParameterInfo();
                parameter.ParameterId = prmId;
                parameter.ParameterValue = txtValue.Value;
                parameters.Add(parameter);
            }

            sc.Parameters = parameters.ToArray();

            // Gather parameters from view.
            if (this.configurationView != null)
            {
                sc.Parameters = this.configurationView.GetParameters();
            }

            List<ScheduleTaskParameterInfo> mergedParameters = new List<ScheduleTaskParameterInfo>(sc.Parameters ?? Array.Empty<ScheduleTaskParameterInfo>());
            UpsertParameter(mergedParameters, SchedulerWeightParameterId, txtSchedulerWeight.Text, SchedulerWeightAliases);
            UpsertParameter(mergedParameters, SchedulerAffinityParameterId, txtSchedulerAffinity.Text, SchedulerAffinityAliases);
            UpsertParameter(mergedParameters, SchedulerExecutionModeParameterId, GetExecutionModeForSave(sc), SchedulerExecutionModeAliases);
            string parallelismModeForSave = GetParallelismModeForSave(sc);
            UpsertParameter(mergedParameters, SchedulerParallelismModeParameterId, parallelismModeForSave, SchedulerParallelismModeAliases);
            string parallelismMaxForSave = String.Equals(parallelismModeForSave, ParallelismModeManual, StringComparison.OrdinalIgnoreCase)
                ? GetParallelismMaxForSave(sc)
                : String.Empty;
            UpsertParameter(mergedParameters, SchedulerParallelismMaxParameterId, parallelismMaxForSave, SchedulerParallelismMaxAliases);
            sc.Parameters = mergedParameters.ToArray();

            // save
            if (PanelRequest.ScheduleID == 0)
            {
                // add new schedule
                try
                {
                    int result = ES.Services.Scheduler.AddSchedule(sc);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_ADD_TASK", ex);
                    return;
                }
            }
            else
            {
                // update existing
                try
                {
                    int result = ES.Services.Scheduler.UpdateSchedule(sc);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_UPDATE_TASK", ex);
                    return;
                }
            }

            // redirect
            RedirectSpaceHomePage();
        }

        private static string NormalizeExecutionMode(string mode)
        {
            string candidate = (mode ?? String.Empty).Trim();
            if (String.Equals(candidate, ExecutionModeServerPreferred, StringComparison.OrdinalIgnoreCase))
                return ExecutionModeServerPreferred;

            if (String.Equals(candidate, ExecutionModeEnterpriseOnly, StringComparison.OrdinalIgnoreCase))
                return ExecutionModeEnterpriseOnly;

            return ExecutionModeAuto;
        }

        private string GetExecutionModeForSave(ScheduleInfo schedule)
        {
            if (PanelSecurity.LoggedUser.Role == UserRole.Administrator)
            {
                DropDownList executionModeDropDown = ExecutionModeDropDown;
                if (executionModeDropDown != null)
                    return NormalizeExecutionMode(executionModeDropDown.SelectedValue);

                return ExecutionModeAuto;
            }

            if (PanelRequest.ScheduleID <= 0)
                return ExecutionModeAuto;

            ScheduleTaskParameterInfo[] existingParameters = ES.Services.Scheduler.GetScheduleParameters(schedule.TaskId, PanelRequest.ScheduleID);
            return NormalizeExecutionMode(FindParameterValue(existingParameters, SchedulerExecutionModeAliases));
        }

        private static string NormalizeParallelismMode(string mode)
        {
            string candidate = (mode ?? String.Empty).Trim();
            if (String.Equals(candidate, ParallelismModeManual, StringComparison.OrdinalIgnoreCase))
                return ParallelismModeManual;

            return ParallelismModeAuto;
        }

        private static string NormalizeParallelismMax(string value)
        {
            if (!Int32.TryParse((value ?? String.Empty).Trim(), out int parsed))
                return String.Empty;

            parsed = Math.Max(1, Math.Min(100, parsed));
            return parsed.ToString();
        }

        private string GetParallelismModeForSave(ScheduleInfo schedule)
        {
            if (PanelSecurity.LoggedUser.Role == UserRole.Administrator)
            {
                DropDownList parallelismModeDropDown = ParallelismModeDropDown;
                if (parallelismModeDropDown != null)
                    return NormalizeParallelismMode(parallelismModeDropDown.SelectedValue);

                return ParallelismModeAuto;
            }

            if (PanelRequest.ScheduleID <= 0)
                return ParallelismModeAuto;

            ScheduleTaskParameterInfo[] existingParameters = ES.Services.Scheduler.GetScheduleParameters(schedule.TaskId, PanelRequest.ScheduleID);
            return NormalizeParallelismMode(FindParameterValue(existingParameters, SchedulerParallelismModeAliases));
        }

        private string GetParallelismMaxForSave(ScheduleInfo schedule)
        {
            if (PanelSecurity.LoggedUser.Role == UserRole.Administrator)
            {
                TextBox parallelismMaxTextBox = ParallelismMaxTextBox;
                if (parallelismMaxTextBox != null)
                    return NormalizeParallelismMax(parallelismMaxTextBox.Text);

                return String.Empty;
            }

            if (PanelRequest.ScheduleID <= 0)
                return String.Empty;

            ScheduleTaskParameterInfo[] existingParameters = ES.Services.Scheduler.GetScheduleParameters(schedule.TaskId, PanelRequest.ScheduleID);
            return NormalizeParallelismMax(FindParameterValue(existingParameters, SchedulerParallelismMaxAliases));
        }

        private void DeleteTask()
        {
            if (!EnsureScheduleAccess())
                return;

            try
            {
                // delete
                if (PanelRequest.ScheduleID == 0)
                    return;

                // delete schedule
                int result = ES.Services.Scheduler.DeleteSchedule(PanelRequest.ScheduleID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }

                // redirect
                RedirectSpaceHomePage();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("SCHEDULE_DELETE_TASK", ex);
                return;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SaveTask();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            RedirectSpaceHomePage();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteTask();
        }
    }
}
