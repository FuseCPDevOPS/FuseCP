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
using System.Collections.Generic;
using System.Linq;

using FuseCP.EnterpriseServer;
using System.Text;

namespace FuseCP.Portal
{
    public partial class Schedules : FuseCPModuleBase
    {
        private string schedulerOverviewText = String.Empty;
        private string schedulerHealthText = String.Empty;
        private string schedulerExecutionText = String.Empty;
        private string schedulerAutotuneText = String.Empty;
        private string schedulerRoleGuidanceText = String.Empty;
        private string schedulerPlacementText = String.Empty;
        private string schedulerStatusValueText = "Ok";
        private string schedulerStatusLabelText = "View cron status";
        private string schedulerLastInvocationText = "No data";
        private string schedulerNextRunText = "No data";
        private string schedulerStatusCardCssClass = "fcp-scheduler-stat fcp-scheduler-stat-ok";
        private string schedulerOverrideResultText = String.Empty;

        private static readonly string[] SchedulerWeightParameterIds = { "SCHEDULER_WEIGHT", "TASK_WEIGHT", "WEIGHT" };
        private static readonly string[] SchedulerAffinityParameterIds = { "SCHEDULER_AFFINITY", "SERVER_ID", "AFFINITY" };
        private static readonly string[] SchedulerExecutionModeParameterIds = { "SCHEDULER_EXECUTION_MODE", "EXECUTION_MODE", "SCHEDULER_MODE" };
        private static readonly string[] SchedulerParallelismModeParameterIds = { "SCHEDULER_PARALLELISM_MODE", "PARALLELISM_MODE", "SCHEDULER_TASK_PARALLELISM_MODE" };
        private static readonly string[] SchedulerParallelismMaxParameterIds = { "SCHEDULER_PARALLELISM_MAX", "PARALLELISM_MAX", "SCHEDULER_TASK_PARALLELISM_MAX" };
        private static readonly string[] SchedulerTaskParallelismParameterIds = { "MAX_PARALLEL_PACKAGES", "MAX_PARALLEL_ORGANIZATIONS" };
        private static readonly string[] SchedulerRiskLevelParameterIds = { "SCHEDULER_RISK_LEVEL" };
        private static readonly string[] SchedulerApprovalStateParameterIds = { "SCHEDULER_APPROVAL_STATE" };
        private static readonly string[] SchedulerApprovedParameterIds = { "SCHEDULER_APPROVED" };

        private const string ExecutionModeAuto = "AUTO";
        private const string ExecutionModeServerPreferred = "SERVER_PREFERRED";
        private const string ExecutionModeEnterpriseOnly = "ENTERPRISE_ONLY";
        private const string ParallelismModeAuto = "AUTO";
        private const string ParallelismModeManual = "MANUAL";

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

        private void SetLiteralText(string controlId, string value)
        {
            Literal literal = FindControlRecursive(this, controlId) as Literal;
            if (literal != null)
                literal.Text = value ?? String.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //BindServerTime();

            // set display preferences
            gvSchedules.PageSize = UsersHelper.GetDisplayItemsPerPage();

            if (!IsPostBack)
            {
                
                chkRecursive.Visible = (PanelSecurity.EffectiveUser.Role != UserRole.User);
                // toggle controls
                //btnAddItem.Enabled = PackagesHelper.CheckGroupQuotaEnabled(
                 //   PanelSecurity.PackageId, ResourceGroups.Statistics, Quotas.STATS_SITES);

                searchBox.AddCriteria("ScheduleName", GetLocalizedString("Text.ScheduleName"));
                searchBox.AddCriteria("Username", GetLocalizedString("Text.Username"));
                searchBox.AddCriteria("FullName", GetLocalizedString("Text.FullName"));
                searchBox.AddCriteria("Email", GetLocalizedString("Text.Email"));

                bool isUser = PanelSecurity.SelectedUser.Role == UserRole.User;
                gvSchedules.Columns[gvSchedules.Columns.Count - 1].Visible = !isUser;
                gvSchedules.Columns[gvSchedules.Columns.Count - 2].Visible = !isUser;
                gvSchedules.Columns[gvSchedules.Columns.Count - 3].Visible = !isUser;
                rowSchedulerDashboard.Visible = true;
                chkAutoRefresh.Checked = false;
                pnlSchedulerOverrides.Visible = PanelSecurity.SelectedUser.Role == UserRole.Administrator;

                if (pnlSchedulerOverrides.Visible)
                    BindRuntimeOverrideInputs();

                BindOverview();
            }

            tasksTimer.Enabled = chkAutoRefresh.Checked;

            if (String.IsNullOrEmpty(litScheduleOverview.Text))
            {
                BindOverview();
            }

            ApplyOverviewLiterals();
            searchBox.AjaxData = this.GetSearchBoxAjaxData();
        }

        private void ApplyOverviewLiterals()
        {
            SetLiteralText("litScheduleOverview", schedulerOverviewText);
            SetLiteralText("litSchedulerHealth", schedulerHealthText);
            SetLiteralText("litSchedulerExecution", schedulerExecutionText);
            SetLiteralText("litSchedulerAutotune", schedulerAutotuneText);
            SetLiteralText("litSchedulerRoleGuidance", schedulerRoleGuidanceText);
            SetLiteralText("litSchedulerPlacement", schedulerPlacementText);
            SetLiteralText("litCronStatusValue", schedulerStatusValueText);
            SetLiteralText("litCronStatusLabel", schedulerStatusLabelText);
            SetLiteralText("litCronLastInvocationValue", schedulerLastInvocationText);
            SetLiteralText("litCronNextRunValue", schedulerNextRunText);
            SetLiteralText("litSchedulerOverrideResult", schedulerOverrideResultText);

            Control cardCronStatus = FindControlRecursive(this, "cardCronStatus");
            if (cardCronStatus is HtmlControl htmlControl)
                htmlControl.Attributes["class"] = schedulerStatusCardCssClass;
            else if (cardCronStatus is WebControl webControl)
                webControl.CssClass = schedulerStatusCardCssClass;
        }

        private void BindRuntimeOverrideInputs()
        {
            try
            {
                txtPerAffinityConcurrency.Text = ES.Services.Scheduler.GetSchedulerRuntimePerAffinityConcurrency().ToString();
                txtGlobalConcurrency.Text = ES.Services.Scheduler.GetSchedulerRuntimeGlobalConcurrency().ToString();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                schedulerOverrideResultText = "<span class=\"text-danger small\">Unable to read runtime scheduler values.</span>";
            }
        }

        private void BindOverview()
        {
            DataSet dsSchedules = new SchedulesHelper().GetOverviewSchedules(
                chkRecursive != null && chkRecursive.Checked,
                searchBox != null ? searchBox.FilterColumn : String.Empty,
                searchBox != null ? searchBox.FilterValue : String.Empty,
                2000);
            if (dsSchedules == null || dsSchedules.Tables.Count == 0)
            {
                schedulerOverviewText = GetLocalizedString("Text.NoData");
                return;
            }

            DataTable dtSchedules = null;
            if (dsSchedules.Tables.Count > 1 && dsSchedules.Tables[1] != null && dsSchedules.Tables[1].Columns.Contains("StatusID"))
                dtSchedules = dsSchedules.Tables[1];
            else if (dsSchedules.Tables[0] != null && dsSchedules.Tables[0].Columns.Contains("StatusID"))
                dtSchedules = dsSchedules.Tables[0];

            if (dtSchedules == null)
            {
                schedulerOverviewText = GetLocalizedString("Text.NoData");
                schedulerLastInvocationText = "No data";
                schedulerNextRunText = "No upcoming run";
                return;
            }

            int totalSchedules = dtSchedules.Rows.Count;
            int runningSchedules = 0;
            int queuedSchedules = 0;
            int warningSchedules = 0;
            int failedSchedules = 0;
            DateTime mostRecentLastRun = DateTime.MinValue;
            DateTime nearestNextRun = DateTime.MaxValue;
            DateTime now = DateTime.Now;
            Dictionary<string, ServerScheduleTotals> serverTotals = new Dictionary<string, ServerScheduleTotals>(StringComparer.OrdinalIgnoreCase);
            List<ScheduleInfo> authorizedSchedules = new List<ScheduleInfo>();

            foreach (DataRow row in dtSchedules.Rows)
            {
                int statusId = row.Table.Columns.Contains("StatusID") ? Utils.ParseInt(row["StatusID"], 0) : 0;
                int lastResult = row.Table.Columns.Contains("LastResult") ? Utils.ParseInt(row["LastResult"], 0) : 0;
                string serverName = row.Table.Columns.Contains("ServerName") && row["ServerName"] != DBNull.Value
                    ? row["ServerName"].ToString()
                    : String.Empty;
                DateTime rowLastRun = DateTime.MinValue;
                if (row.Table.Columns.Contains("LastRun") && row["LastRun"] != DBNull.Value)
                    DateTime.TryParse(row["LastRun"].ToString(), out rowLastRun);

                if (rowLastRun > DateTime.MinValue && rowLastRun > mostRecentLastRun)
                    mostRecentLastRun = rowLastRun;

                DateTime rowNextRun = DateTime.MinValue;
                if (row.Table.Columns.Contains("NextRun") && row["NextRun"] != DBNull.Value)
                    DateTime.TryParse(row["NextRun"].ToString(), out rowNextRun);

                if (rowNextRun > now && rowNextRun < nearestNextRun)
                    nearestNextRun = rowNextRun;

                if (statusId == (int)ScheduleStatus.Running)
                    runningSchedules++;
                else if (statusId == (int)ScheduleStatus.Queued)
                    queuedSchedules++;

                if (lastResult == 1)
                    warningSchedules++;
                else if (lastResult == 2)
                    failedSchedules++;

                if (!serverTotals.TryGetValue(serverName, out ServerScheduleTotals totals))
                {
                    totals = new ServerScheduleTotals();
                    serverTotals[serverName] = totals;
                }

                totals.Total++;
                if (statusId == (int)ScheduleStatus.Running)
                    totals.Running++;
                else if (statusId == (int)ScheduleStatus.Queued)
                    totals.Queued++;
                if (lastResult == 1)
                    totals.Warning++;
                else if (lastResult == 2)
                    totals.Failed++;

                int scheduleId = row.Table.Columns.Contains("ScheduleID") ? Utils.ParseInt(row["ScheduleID"], 0) : 0;
                if (scheduleId > 0)
                {
                    ScheduleInfo schedule = TryGetAuthorizedSchedule(scheduleId);
                    if (schedule != null)
                    {
                        authorizedSchedules.Add(schedule);

                        if (schedule.LastRun > DateTime.MinValue && schedule.LastRun > mostRecentLastRun)
                            mostRecentLastRun = schedule.LastRun;

                        if (schedule.Enabled)
                        {
                            DateTime candidateNextRun = schedule.NextRun;
                            if (candidateNextRun <= now)
                                candidateNextRun = CalculateUpcomingRun(schedule, now);

                            if (candidateNextRun > now && candidateNextRun < nearestNextRun)
                                nearestNextRun = candidateNextRun;
                        }
                    }
                }
            }

            List<string> serverSummary = new List<string>();
            int serverBoundSchedules = 0;
            int enterpriseBoundSchedules = 0;
            int highRiskPendingApprovals = 0;

            foreach (ScheduleInfo authorizedSchedule in authorizedSchedules)
            {
                if (IsHighRiskPendingApproval(authorizedSchedule))
                    highRiskPendingApprovals++;
            }

            foreach (KeyValuePair<string, ServerScheduleTotals> entry in serverTotals)
            {
                string serverLabel = String.IsNullOrWhiteSpace(entry.Key) ? "Unassigned" : PortalAntiXSS.Encode(entry.Key);
                serverSummary.Add(String.Format("{0}: {1} total, {2} running, {3} queued, {4} warnings, {5} failed", serverLabel,
                    entry.Value.Total, entry.Value.Running, entry.Value.Queued, entry.Value.Warning, entry.Value.Failed));

                if (String.IsNullOrWhiteSpace(entry.Key))
                    enterpriseBoundSchedules += entry.Value.Total;
                else
                    serverBoundSchedules += entry.Value.Total;
            }

            schedulerOverviewText = String.Format(
                "{0} total schedules, {1} running, {2} queued, {3} warnings, {4} failed. {5} Execution placement: {6} server-bound, {7} enterprise-runtime. Scheduler execution is limited to a bounded queue so extra work waits instead of fanning out.",
                totalSchedules,
                runningSchedules,
                queuedSchedules,
                warningSchedules,
                failedSchedules,
                serverSummary.Count > 0 ? "Per-server: " + String.Join("; ", serverSummary.ToArray()) + "." : String.Empty,
                serverBoundSchedules,
                enterpriseBoundSchedules);

            schedulerHealthText = BuildSchedulerHealthHtml(authorizedSchedules);
            schedulerExecutionText = BuildSchedulerExecutionHtml();
            schedulerAutotuneText = BuildSchedulerAutotuneHtml();
            schedulerRoleGuidanceText = BuildRoleGuidanceHtml();
            schedulerPlacementText = BuildSchedulerPlacementHtml(totalSchedules, serverBoundSchedules, enterpriseBoundSchedules, highRiskPendingApprovals);
            if (mostRecentLastRun <= DateTime.MinValue)
            {
                DateTime latestAuditStart = GetLatestSchedulerAuditStart();
                if (latestAuditStart > DateTime.MinValue)
                    mostRecentLastRun = latestAuditStart;
            }

            schedulerLastInvocationText = mostRecentLastRun > DateTime.MinValue ? FormatRelativeTime(mostRecentLastRun) : "No data";
            schedulerNextRunText = nearestNextRun < DateTime.MaxValue ? FormatRelativeTime(nearestNextRun) : "No upcoming run";

            if (failedSchedules > 0)
            {
                schedulerStatusValueText = "Attention";
                schedulerStatusLabelText = "Failures detected in cron tasks";
                schedulerStatusCardCssClass = "fcp-scheduler-stat fcp-scheduler-stat-danger";
            }
            else if (runningSchedules > 0)
            {
                schedulerStatusValueText = "Active";
                schedulerStatusLabelText = "Cron workers are running";
                schedulerStatusCardCssClass = "fcp-scheduler-stat fcp-scheduler-stat-ok";
            }
            else if (queuedSchedules > 0)
            {
                schedulerStatusValueText = "Queued";
                schedulerStatusLabelText = "Cron work is queued";
                schedulerStatusCardCssClass = "fcp-scheduler-stat fcp-scheduler-stat-last";
            }
            else
            {
                schedulerStatusValueText = "Ok";
                schedulerStatusLabelText = "View cron status";
                schedulerStatusCardCssClass = "fcp-scheduler-stat fcp-scheduler-stat-ok";
            }
        }

        private static string FormatRelativeTime(DateTime dateTime)
        {
            TimeSpan delta = dateTime - DateTime.Now;
            bool future = delta.TotalSeconds >= 0;
            TimeSpan span = future ? delta : delta.Negate();

            if (span.TotalSeconds < 60)
            {
                int seconds = Math.Max(1, (int)Math.Round(span.TotalSeconds));
                return future
                    ? String.Format("in {0} second{1}", seconds, seconds == 1 ? String.Empty : "s")
                    : String.Format("{0} second{1} ago", seconds, seconds == 1 ? String.Empty : "s");
            }

            if (span.TotalMinutes < 60)
            {
                int minutes = Math.Max(1, (int)Math.Round(span.TotalMinutes));
                return future
                    ? String.Format("in {0} minute{1}", minutes, minutes == 1 ? String.Empty : "s")
                    : String.Format("{0} minute{1} ago", minutes, minutes == 1 ? String.Empty : "s");
            }

            if (span.TotalHours < 48)
            {
                int hours = Math.Max(1, (int)Math.Round(span.TotalHours));
                return future
                    ? String.Format("in {0} hour{1}", hours, hours == 1 ? String.Empty : "s")
                    : String.Format("{0} hour{1} ago", hours, hours == 1 ? String.Empty : "s");
            }

            int days = Math.Max(1, (int)Math.Round(span.TotalDays));
            return future
                ? String.Format("in {0} day{1}", days, days == 1 ? String.Empty : "s")
                : String.Format("{0} day{1} ago", days, days == 1 ? String.Empty : "s");
        }

        private DateTime GetLatestSchedulerAuditStart()
        {
            try
            {
                DateTime end = DateTime.Now.AddMinutes(1);
                DateTime start = end.AddDays(-30);

                DataSet recordsSet = ES.Services.AuditLog.GetAuditLogRecordsPaged(
                    PanelSecurity.SelectedUserId,
                    PanelSecurity.PackageId,
                    0,
                    String.Empty,
                    start,
                    end,
                    0,
                    "SCHEDULER",
                    "RUN_SCHEDULE",
                    "StartDate DESC",
                    0,
                    1);

                DataTable records = recordsSet != null && recordsSet.Tables.Count > 1 ? recordsSet.Tables[1] : null;
                if (records == null || records.Rows.Count == 0)
                    return DateTime.MinValue;

                return Utils.ParseDate(records.Rows[0]["StartDate"]?.ToString());
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return DateTime.MinValue;
            }
        }

        private string BuildSchedulerHealthHtml(IEnumerable<ScheduleInfo> schedules)
        {
            List<ScheduleInfo> scheduleList = schedules == null ? new List<ScheduleInfo>() : schedules.ToList();
            if (scheduleList.Count == 0)
                return "<span class=\"text-muted\">No schedules available in your scope.</span>";

            int defaultWeight = GetAppSettingInt("SchedulerDefaultTaskWeight", 1, 1, 100);
            var totals = new Dictionary<string, HealthTotals>(StringComparer.OrdinalIgnoreCase);

            foreach (ScheduleInfo schedule in scheduleList)
            {
                string affinity = GetScheduleParameterValue(schedule.Parameters, SchedulerAffinityParameterIds);
                if (String.IsNullOrWhiteSpace(affinity))
                    affinity = ResolveServerName(schedule.PackageId);
                if (String.IsNullOrWhiteSpace(affinity))
                    affinity = "Unassigned";

                int weight = GetScheduleWeight(schedule.Parameters, defaultWeight);

                if (!totals.TryGetValue(affinity, out HealthTotals healthTotals))
                {
                    healthTotals = new HealthTotals();
                    totals[affinity] = healthTotals;
                }

                bool running = IsScheduleRunning(schedule);
                if (running)
                {
                    healthTotals.ActiveTasks++;
                    healthTotals.ActiveUnits += weight;
                }
                else if (schedule.Enabled)
                {
                    healthTotals.QueuedTasks++;
                    healthTotals.QueuedUnits += weight;
                }

                if (weight > healthTotals.PeakWeight)
                    healthTotals.PeakWeight = weight;
            }

            StringBuilder html = new StringBuilder();
            html.Append("<div class=\"table-responsive\"><table class=\"table table-sm align-middle mb-0\">");
            html.Append("<thead><tr><th>Server/Affinity</th><th>Active Tasks</th><th>Estimated Active Units</th><th>Queued Tasks</th><th>Estimated Queued Units</th><th>Peak Task Weight</th></tr></thead><tbody>");

            foreach (KeyValuePair<string, HealthTotals> entry in totals.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                string affinity = PortalAntiXSS.Encode(entry.Key);
                HealthTotals total = entry.Value;
                html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                    affinity,
                    total.ActiveTasks,
                    total.ActiveUnits,
                    total.QueuedTasks,
                    total.QueuedUnits,
                    total.PeakWeight);
            }

            html.Append("</tbody></table></div>");
            html.Append("<div class=\"small text-muted mt-2\">Units are estimated from task parameters (SCHEDULER_WEIGHT/TASK_WEIGHT/WEIGHT). Affinity uses SCHEDULER_AFFINITY/SERVER_ID when provided.</div>");

            return html.ToString();
        }

        private string BuildSchedulerAutotuneHtml()
        {
            bool autoTuneEnabled = GetAppSettingBool("SchedulerAutoTuneEnabled", true);
            int minConcurrent = GetAppSettingInt("SchedulerAutoTuneMinConcurrentExecutions", 1, 1, 256);
            int maxConcurrent = GetAppSettingInt("SchedulerAutoTuneMaxConcurrentExecutions", 16, 1, 1024);
            int globalMax = GetAppSettingInt("SchedulerGlobalMaxConcurrentExecutions", 64, 1, 2048);
            int cpuThreshold = GetAppSettingInt("SchedulerAutoTuneCpuThresholdPercent", 80, 1, 100);
            int memoryThreshold = GetAppSettingInt("SchedulerAutoTuneMemoryThresholdPercent", 80, 1, 100);

            StringBuilder html = new StringBuilder();
            html.Append("<dl class=\"row mb-0\">");
            html.AppendFormat("<dt class=\"col-7\">Auto-Tune</dt><dd class=\"col-5\">{0}</dd>", autoTuneEnabled ? "Enabled" : "Disabled");
            html.AppendFormat("<dt class=\"col-7\">Min Concurrency</dt><dd class=\"col-5\">{0}</dd>", minConcurrent);
            html.AppendFormat("<dt class=\"col-7\">Max Concurrency</dt><dd class=\"col-5\">{0}</dd>", maxConcurrent);
            html.AppendFormat("<dt class=\"col-7\">Global Safety Cap</dt><dd class=\"col-5\">{0}</dd>", globalMax);
            html.AppendFormat("<dt class=\"col-7\">CPU Threshold</dt><dd class=\"col-5\">{0}%</dd>", cpuThreshold);
            html.AppendFormat("<dt class=\"col-7\">Memory Threshold</dt><dd class=\"col-5\">{0}%</dd>", memoryThreshold);
            html.Append("</dl>");
            html.Append("<div class=\"small text-muted mt-2\">This panel reflects current application configuration values. Runtime adaptive decisions are enforced by the server scheduler service.</div>");
            return html.ToString();
        }

        private string BuildSchedulerExecutionHtml()
        {
            try
            {
                DateTime end = DateTime.Now.AddMinutes(1);
                DateTime start = end.AddDays(-2);

                DataSet recordsSet = ES.Services.AuditLog.GetAuditLogRecordsPaged(
                    PanelSecurity.SelectedUserId,
                    PanelSecurity.PackageId,
                    0,
                    String.Empty,
                    start,
                    end,
                    0,
                    "SCHEDULER",
                    "RUN_SCHEDULE",
                    "StartDate DESC",
                    0,
                    10);

                DataTable records = recordsSet != null && recordsSet.Tables.Count > 1 ? recordsSet.Tables[1] : null;
                if (records == null || records.Rows.Count == 0)
                    return "<div class=\"small text-muted mt-3\">No scheduler audit records found yet.</div>";

                StringBuilder html = new StringBuilder();
                html.Append("<div class=\"mt-3\"><div class=\"small fw-semibold mb-2\">Recent Scheduler Runs</div>");
                html.Append("<div class=\"table-responsive\"><table class=\"table table-sm align-middle mb-0\">");
                html.Append("<thead><tr><th>Started</th><th>Task</th><th>Severity</th><th>Finished</th></tr></thead><tbody>");

                foreach (DataRow row in records.Rows)
                {
                    string started = Utils.ParseDate(row["StartDate"].ToString()).ToString();
                    string taskName = PortalAntiXSS.Encode(row["ItemName"].ToString());
                    string severity = GetAuditLogRecordSeverityName(Utils.ParseInt(row["SeverityID"], 0));
                    DateTime finishDate = Utils.ParseDate(row["FinishDate"].ToString());
                    string finished = finishDate > DateTime.MinValue ? finishDate.ToString() : "In progress";

                    html.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", started, taskName, severity, finished);
                }

                html.Append("</tbody></table></div></div>");
                return html.ToString();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                return "<div class=\"small text-muted mt-3\">Unable to load scheduler audit records.</div>";
            }
        }

        private string BuildRoleGuidanceHtml()
        {
            UserRole role = PanelSecurity.SelectedUser.Role;
            if (role == UserRole.Administrator)
            {
                return "<div class=\"fcp-scheduler-role-chip fcp-scheduler-role-admin\">Admin View</div>" +
                    "<div class=\"fcp-scheduler-role-copy\">You can tune runtime concurrency, run queued tasks immediately, and validate execution topology across all tenants.</div>";
            }

            if (role == UserRole.Reseller)
            {
                return "<div class=\"fcp-scheduler-role-chip fcp-scheduler-role-reseller\">Reseller View</div>" +
                    "<div class=\"fcp-scheduler-role-copy\">Focus on package-level schedules and run outcomes for your tenant scope. Runtime engine overrides remain administrator-only.</div>";
            }

            return "<div class=\"fcp-scheduler-role-chip fcp-scheduler-role-user\">User View</div>" +
                "<div class=\"fcp-scheduler-role-copy\">This view is optimized for your selected space: monitor run health, next run times, and recent execution outcomes.</div>";
        }

        private string BuildSchedulerPlacementHtml(int totalSchedules, int serverBoundSchedules, int enterpriseBoundSchedules, int highRiskPendingApprovals)
        {
            int queueDepth = 0;
            int activeUnits = 0;
            int perAffinityCap = 0;
            int globalCap = 0;
            int tenantCap = GetAppSettingInt("SchedulerTenantMaxConcurrentExecutions", 4, 1, 1024);
            int providerCap = GetAppSettingInt("SchedulerProviderMaxConcurrentExecutions", 8, 1, 1024);
            bool freezeEnabled = GetAppSettingBool("SchedulerFreezeEnabled", false);

            try
            {
                queueDepth = ES.Services.Scheduler.GetSchedulerRuntimeQueueDepth();
                activeUnits = ES.Services.Scheduler.GetSchedulerRuntimeActiveUnits();
                perAffinityCap = ES.Services.Scheduler.GetSchedulerRuntimePerAffinityConcurrency();
                globalCap = ES.Services.Scheduler.GetSchedulerRuntimeGlobalConcurrency();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                // keep fallback zeros
            }

            StringBuilder html = new StringBuilder();
            html.Append("<ul class=\"fcp-scheduler-topology-list\">");
            html.AppendFormat("<li><span>Server-bound schedules</span><strong>{0}</strong></li>", serverBoundSchedules);
            html.AppendFormat("<li><span>Enterprise-runtime schedules</span><strong>{0}</strong></li>", enterpriseBoundSchedules);
            html.AppendFormat("<li><span>Queued execution units</span><strong>{0}</strong></li>", queueDepth);
            html.AppendFormat("<li><span>Active execution units</span><strong>{0}</strong></li>", activeUnits);
            html.AppendFormat("<li><span>Per-affinity cap</span><strong>{0}</strong></li>", perAffinityCap);
            html.AppendFormat("<li><span>Global cap</span><strong>{0}</strong></li>", globalCap);
            html.AppendFormat("<li><span>Per-tenant cap</span><strong>{0}</strong></li>", tenantCap);
            html.AppendFormat("<li><span>Per-provider cap</span><strong>{0}</strong></li>", providerCap);
            html.AppendFormat("<li><span>Freeze mode</span><strong>{0}</strong></li>", freezeEnabled ? "Enabled" : "Disabled");
            html.AppendFormat("<li><span>High-risk schedules awaiting second approval</span><strong>{0}</strong></li>", highRiskPendingApprovals);
            html.Append("</ul>");
            html.Append("<div class=\"small text-muted mt-2\">Recommended model: keep EnterpriseServer as control plane (policy, tenancy, queue), and execute heavy per-server checks through server modules/agents that report back.</div>");

            if (totalSchedules > 0 && serverBoundSchedules == 0)
            {
                html.Append("<div class=\"small text-warning mt-2\">No server-bound schedules detected. For larger fleets, move high-frequency host probes closer to server modules to reduce central fan-out.</div>");
            }

            if (freezeEnabled)
            {
                html.Append("<div class=\"small text-warning mt-2\">Scheduler freeze mode is enabled. Automatic runs are paused until freeze is cleared.</div>");
            }

            return html.ToString();
        }

        private static bool IsHighRiskPendingApproval(ScheduleInfo schedule)
        {
            if (schedule == null)
                return false;

            string riskLevel = GetScheduleParameterValue(schedule.Parameters, SchedulerRiskLevelParameterIds);
            if (!String.Equals((riskLevel ?? String.Empty).Trim(), "HIGH", StringComparison.OrdinalIgnoreCase))
                return false;

            string approved = GetScheduleParameterValue(schedule.Parameters, SchedulerApprovedParameterIds);
            if (String.Equals((approved ?? String.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase))
                return false;

            string approvalState = GetScheduleParameterValue(schedule.Parameters, SchedulerApprovalStateParameterIds);
            return String.Equals((approvalState ?? String.Empty).Trim(), "PENDING_SECOND_APPROVAL", StringComparison.OrdinalIgnoreCase)
                || String.IsNullOrWhiteSpace(approvalState);
        }

        private static bool GetAppSettingBool(string key, bool defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            if (Boolean.TryParse(value, out bool parsed))
                return parsed;

            return defaultValue;
        }

        private static int GetAppSettingInt(string key, int defaultValue, int min, int max)
        {
            string value = ConfigurationManager.AppSettings[key];
            int parsed = Utils.ParseInt(value, defaultValue);
            if (parsed < min)
                return min;
            if (parsed > max)
                return max;
            return parsed;
        }

        private static int GetScheduleWeight(ScheduleTaskParameterInfo[] parameters, int defaultWeight)
        {
            string value = GetScheduleParameterValue(parameters, SchedulerWeightParameterIds);
            int parsed = Utils.ParseInt(value, defaultWeight);
            return parsed > 0 ? parsed : defaultWeight;
        }

        private static string GetScheduleParameterValue(ScheduleTaskParameterInfo[] parameters, IEnumerable<string> parameterIds)
        {
            if (parameters == null)
                return String.Empty;

            foreach (string parameterId in parameterIds)
            {
                foreach (ScheduleTaskParameterInfo parameter in parameters)
                {
                    if (parameter == null || String.IsNullOrEmpty(parameter.ParameterId))
                        continue;

                    if (String.Equals(parameter.ParameterId, parameterId, StringComparison.OrdinalIgnoreCase))
                        return parameter.ParameterValue ?? String.Empty;
                }
            }

            return String.Empty;
        }

        public string GetExecutionModeBadge(object scheduleIdValue)
        {
            int scheduleId = Utils.ParseInt(scheduleIdValue, 0);
            if (scheduleId <= 0)
                return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\">Auto</span>";

            ScheduleInfo schedule = TryGetAuthorizedSchedule(scheduleId);
            if (schedule == null)
                return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\">Auto</span>";

            string mode = NormalizeExecutionMode(GetScheduleParameterValue(schedule.Parameters, SchedulerExecutionModeParameterIds));
            if (String.Equals(mode, ExecutionModeServerPreferred, StringComparison.OrdinalIgnoreCase))
            {
                return "<span class=\"fcp-execution-badge fcp-execution-badge-server\" title=\"Prefers server module/agent execution and reports back\">Server preferred</span>";
            }

            if (String.Equals(mode, ExecutionModeEnterpriseOnly, StringComparison.OrdinalIgnoreCase))
            {
                return "<span class=\"fcp-execution-badge fcp-execution-badge-enterprise\" title=\"Runs on EnterpriseServer runtime\">Enterprise</span>";
            }

            return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\" title=\"Safe default with automatic placement\">Auto</span>";
        }

        public string GetParallelismBadge(object scheduleIdValue)
        {
            int scheduleId = Utils.ParseInt(scheduleIdValue, 0);
            if (scheduleId <= 0)
                return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\" title=\"Task default parallelism\">Parallelism: default</span>";

            ScheduleInfo schedule = TryGetAuthorizedSchedule(scheduleId);
            if (schedule == null)
                return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\" title=\"Task default parallelism\">Parallelism: default</span>";

            string explicitTaskParallelism = GetScheduleParameterValue(schedule.Parameters, SchedulerTaskParallelismParameterIds);
            if (!String.IsNullOrWhiteSpace(explicitTaskParallelism))
            {
                string encoded = PortalAntiXSS.Encode(explicitTaskParallelism.Trim());
                return String.Format("<span class=\"fcp-execution-badge fcp-execution-badge-enterprise\" title=\"Task-specific parameter override\">Parallelism: explicit ({0})</span>", encoded);
            }

            string configuredMode = NormalizeParallelismMode(GetScheduleParameterValue(schedule.Parameters, SchedulerParallelismModeParameterIds));
            string configuredMax = GetScheduleParameterValue(schedule.Parameters, SchedulerParallelismMaxParameterIds);

            if (String.Equals(configuredMode, ParallelismModeManual, StringComparison.OrdinalIgnoreCase))
            {
                int manualMax = Utils.ParseInt(configuredMax, 0);
                string value = manualMax > 0 ? manualMax.ToString() : "n/a";
                return String.Format("<span class=\"fcp-execution-badge fcp-execution-badge-server\" title=\"Manual scheduler-level override\">Parallelism: manual ({0})</span>", value);
            }

            if (IsAdvisorEligibleTask(schedule))
            {
                return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\" title=\"Scheduler advisor applies in AUTO mode\">Parallelism: auto-advised</span>";
            }

            return "<span class=\"fcp-execution-badge fcp-execution-badge-auto\" title=\"Task default parallelism\">Parallelism: default</span>";
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

        private static string NormalizeParallelismMode(string mode)
        {
            string candidate = (mode ?? String.Empty).Trim();
            if (String.Equals(candidate, ParallelismModeManual, StringComparison.OrdinalIgnoreCase))
                return ParallelismModeManual;

            return ParallelismModeAuto;
        }

        private static bool IsAdvisorEligibleTask(ScheduleInfo schedule)
        {
            if (schedule == null || String.IsNullOrWhiteSpace(schedule.TaskId))
                return false;

            string taskId = schedule.TaskId.ToUpperInvariant();
            return taskId.Contains("CALCULATEPACKAGESDISKSPACE")
                || taskId.Contains("CALCULATEPACKAGESBANDWIDTH")
                || taskId.Contains("CALCULATEEXCHANGEDISKSPACE");
        }

        protected string GetNextRunDisplay(object scheduleIdValue, object nextRunValue)
        {
            DateTime nextRun = DateTime.MinValue;
            if (nextRunValue != null && nextRunValue != DBNull.Value)
                DateTime.TryParse(nextRunValue.ToString(), out nextRun);

            if (nextRun > DateTime.Now)
                return nextRun.ToString();

            int scheduleId = Utils.ParseInt(scheduleIdValue, 0);
            if (scheduleId > 0)
            {
                ScheduleInfo schedule = TryGetAuthorizedSchedule(scheduleId);
                if (schedule != null && schedule.Enabled)
                {
                    DateTime projectedNextRun = CalculateUpcomingRun(schedule, DateTime.Now);
                    if (projectedNextRun > DateTime.MinValue)
                        return projectedNextRun.ToString();
                }
            }

            if (nextRun > DateTime.MinValue)
                return nextRun.ToString();

            return "No upcoming run";
        }

        private static DateTime CalculateUpcomingRun(ScheduleInfo schedule, DateTime now)
        {
            if (schedule == null)
                return DateTime.MinValue;

            if (schedule.ScheduleType == ScheduleType.OneTime)
                return schedule.StartTime > now ? schedule.StartTime : DateTime.MinValue;

            if (schedule.ScheduleType == ScheduleType.Interval)
            {
                DateTime nextStart = schedule.LastRun > DateTime.MinValue
                    ? schedule.LastRun.AddSeconds(Math.Max(1, schedule.Interval))
                    : now;

                if (nextStart < now)
                    nextStart = now;

                DateTime fromTime = new DateTime(now.Year, now.Month, now.Day,
                    schedule.FromTime.Hour, schedule.FromTime.Minute, schedule.FromTime.Second);
                DateTime toTime = new DateTime(now.Year, now.Month, now.Day,
                    schedule.ToTime.Hour, schedule.ToTime.Minute, schedule.ToTime.Second);

                if (!(nextStart >= fromTime && nextStart <= toTime))
                    nextStart = fromTime.AddDays(1);

                return nextStart;
            }

            if (schedule.ScheduleType == ScheduleType.Daily)
            {
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                if (nextStart < now)
                    nextStart = nextStart.AddDays(1);
                return nextStart;
            }

            if (schedule.ScheduleType == ScheduleType.Weekly)
            {
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                int todayWeekDay = (int)now.DayOfWeek;
                nextStart = nextStart.AddDays(schedule.WeekMonthDay - todayWeekDay);
                if (nextStart < now)
                    nextStart = nextStart.AddDays(7);
                return nextStart;
            }

            if (schedule.ScheduleType == ScheduleType.Monthly)
            {
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                int todayDay = now.Day;
                nextStart = nextStart.AddDays(schedule.WeekMonthDay - todayDay);
                if (nextStart < now)
                    nextStart = nextStart.AddMonths(1);
                return nextStart;
            }

            return DateTime.MinValue;
        }

        private static string ResolveServerName(int packageId)
        {
            int serverId = PackagesHelper.GetCachedPackageContext(packageId)?.Package?.ServerId ?? 0;
            if (serverId <= 0)
                return String.Empty;

            ServerInfo server = ES.Services.Servers.GetServerById(serverId);
            return server?.ServerName ?? String.Empty;
        }

        private static bool IsScheduleRunning(ScheduleInfo schedule)
        {
            if (schedule == null || String.IsNullOrWhiteSpace(schedule.StatusId))
                return false;

            int numericStatus;
            if (Int32.TryParse(schedule.StatusId, out numericStatus))
                return numericStatus == (int)ScheduleStatus.Running;

            return String.Equals(schedule.StatusId, ScheduleStatus.Running.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private ScheduleInfo TryGetAuthorizedSchedule(int scheduleId)
        {
            if (scheduleId <= 0)
                return null;

            ScheduleInfo schedule = ES.Services.Scheduler.GetSchedule(scheduleId);
            if (schedule == null)
                return null;

            return IsScheduleInCurrentScope(schedule) ? schedule : null;
        }

        private bool IsScheduleInCurrentScope(ScheduleInfo schedule)
        {
            if (schedule == null)
                return false;

            if (PanelSecurity.SelectedUser.Role == UserRole.Administrator)
                return true;

            return schedule.PackageId == PanelSecurity.PackageId;
        }

        private sealed class HealthTotals
        {
            public int ActiveTasks { get; set; }
            public int ActiveUnits { get; set; }
            public int QueuedTasks { get; set; }
            public int QueuedUnits { get; set; }
            public int PeakWeight { get; set; }
        }

        private sealed class ServerScheduleTotals
        {
            public int Total { get; set; }
            public int Running { get; set; }
            public int Queued { get; set; }
            public int Warning { get; set; }
            public int Failed { get; set; }
        }

        protected void odsSchedules_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                ProcessException(e.Exception);
                e.ExceptionHandled = true;
            }
        }

        /*
        private void BindServerTime()
        {
            try
            {
                litServerTime.Text = ES.Scheduler.GetSchedulerTime().ToString();
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                // skip
            }
        }
         * */

        public string GetScheduleStatus(int statusId)
        {
			return GetSharedLocalizedString(Utils.ModuleName, "ScheduleStatus." + ((ScheduleStatus)statusId));
        }

        public bool IsScheduleActive(int statusId)
        {
            ScheduleStatus status = (ScheduleStatus)statusId;
            return (status == ScheduleStatus.Running || status == ScheduleStatus.Queued);
        }

        public bool IsScheduleQueued(int statusId)
        {
            ScheduleStatus status = (ScheduleStatus)statusId;
            return (status == ScheduleStatus.Queued);
        }

        public string GetUserHomePageUrl(int userId)
        {
            return PortalUtils.GetUserHomePageUrl(userId);
        }

        public string GetSpaceHomePageUrl(int spaceId)
        {
            return NavigateURL(PortalUtils.SPACE_ID_PARAM, spaceId.ToString());
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl(PortalUtils.SPACE_ID_PARAM, PanelSecurity.PackageId.ToString(), "edit"));
        }
        protected void gvSchedules_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int scheduleId = Utils.ParseInt(e.CommandArgument.ToString(), 0);
            ScheduleInfo schedule = TryGetAuthorizedSchedule(scheduleId);
            if (schedule == null)
            {
                ShowErrorMessage("ACCESS_DENIED");
                gvSchedules.DataBind();
                BindOverview();
                ApplyOverviewLiterals();
                return;
            }

            if (e.CommandName == "start")
            {
                try
                {
                    int result = ES.Services.Scheduler.StartSchedule(scheduleId);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_START_TASK", ex);
                    return;
                }
            }
            else if (e.CommandName == "runnow")
            {
                try
                {
                    int result = ES.Services.Scheduler.StartScheduleNow(scheduleId);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_START_TASK", ex);
                    return;
                }
            }
            else if (e.CommandName == "stop")
            {
                try
                {
                    int result = ES.Services.Scheduler.StopSchedule(scheduleId);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("SCHEDULE_STOP_TASK", ex);
                    return;
                }
            }

            // rebind grid
            gvSchedules.DataBind();
            BindOverview();
            ApplyOverviewLiterals();
        }

        public string GetSearchBoxAjaxData()
        {
            StringBuilder res = new StringBuilder();
            res.Append("PagedStored: 'Schedules'");
            res.Append(", RedirectUrl: '" + EditUrl("ScheduleID", "{0}", "edit", "SpaceID=" + PanelSecurity.PackageId).Substring(2) + "'");
            res.Append(", PackageID: " + PanelSecurity.PackageId);
            res.Append(", Recursive: ($('#" + chkRecursive.ClientID + "').val() == 'on')");
            return res.ToString();
        }

        protected void tasksTimer_Tick(object sender, EventArgs e)
        {
            if (!chkAutoRefresh.Checked)
                return;

            gvSchedules.DataBind();
            BindOverview();
            ApplyOverviewLiterals();
        }

        protected void btnApplySchedulerOverrides_Click(object sender, EventArgs e)
        {
            if (PanelSecurity.SelectedUser.Role != UserRole.Administrator)
            {
                schedulerOverrideResultText = "<span class=\"text-danger small\">Access denied.</span>";
                return;
            }

            int perAffinity = Utils.ParseInt(txtPerAffinityConcurrency.Text, 0);
            int global = Utils.ParseInt(txtGlobalConcurrency.Text, 0);
            if (perAffinity <= 0 || global <= 0)
            {
                schedulerOverrideResultText = "<span class=\"text-danger small\">Please enter valid positive integers.</span>";
                return;
            }

            try
            {
                int result = ES.Services.Scheduler.ApplySchedulerRuntimeConcurrency(perAffinity, global);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }

                schedulerOverrideResultText = "<span class=\"text-success small\">Runtime overrides applied.</span>";
                BindRuntimeOverrideInputs();
                BindOverview();
                ApplyOverviewLiterals();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("SCHEDULE_UPDATE_TASK", ex);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            gvSchedules.DataBind();
            BindOverview();
            ApplyOverviewLiterals();
        }

        protected void chkAutoRefresh_CheckedChanged(object sender, EventArgs e)
        {
            tasksTimer.Enabled = chkAutoRefresh.Checked;
        }
    }
}
