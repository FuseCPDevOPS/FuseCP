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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    public partial class TasksTaskDetails : FuseCPModuleBase
    {
        private const int MaxTaskLogHtmlLength = 65536;
        private const string TruncatedLogPrefix = "[older log entries trimmed]<br>";

        protected void Page_Load(object sender, EventArgs e)
        {
            BindTask();
        }

        private void BindTask()
        {
            DateTime lastLogDate = DateTime.MinValue;
            if (ViewState["lastLogDate"] != null)
                lastLogDate = (DateTime)ViewState["lastLogDate"];

            string cachedLogHtml = ViewState["taskLogHtml"] as string ?? String.Empty;
            string cachedStepText = ViewState["taskStepText"] as string ?? String.Empty;

            BackgroundTask task = ES.Services.Tasks.GetTaskWithLogRecords(PanelRequest.TaskID, lastLogDate);
            if (task == null)
            {
                RedirectToBrowsePage();
                return;
            }

            var logs = task.GetLogs();

            // bind task details
            litTitle.Text = String.Format("{0} &quot;{1}&quot;",
                GetAuditLogTaskName(task.Source, task.TaskName),
                task.ItemName);

            // Restore previous step text first. Timer postbacks may return no new log rows.
            litStep.Text = cachedStepText;
            if (logs.Count > 0)
            {
                litStep.Text = LocalizeActivityText(logs[logs.Count - 1].Text);
                ViewState["taskStepText"] = litStep.Text;
            }
            litStartTime.Text = task.StartDate.ToString();

            // progress
            int percent = 0;
            if (task.IndicatorMaximum > 0)
                percent = task.IndicatorCurrent * 100 / task.IndicatorMaximum;
            pnlProgressBar.Width = Unit.Percentage(percent);

            // duration
            litDuration.Text = GetDurationText(task.StartDate, DateTime.Now);

            // execution log
            StringBuilder log = new StringBuilder();
            if (logs.Count > 0)
                ViewState["lastLogDate"] = logs[logs.Count - 1].Date.AddTicks(1);

            foreach (BackgroundTaskLogRecord logRecord in logs)
            {
                log.Append("[").Append(GetDurationText(task.StartDate, logRecord.Date)).Append("] ");
                log.Append(GetLogLineIdent(logRecord.TextIdent));
                log.Append(LocalizeActivityText(logRecord.Text));
                log.Append("<br>");
            }
            if (log.Length > 0)
                cachedLogHtml = AppendAndTrimTaskLogHtml(cachedLogHtml, log.ToString());

            ViewState["taskLogHtml"] = cachedLogHtml;
            litLog.Text = cachedLogHtml;

            if(task.Completed)
                btnStop.Visible = false;
        }

        private static string AppendAndTrimTaskLogHtml(string existingHtml, string appendedHtml)
        {
            string current = existingHtml ?? String.Empty;
            bool hadTruncatedPrefix = current.StartsWith(TruncatedLogPrefix, StringComparison.Ordinal);
            if (hadTruncatedPrefix)
                current = current.Substring(TruncatedLogPrefix.Length);

            string combined = current + (appendedHtml ?? String.Empty);
            if (combined.Length <= MaxTaskLogHtmlLength)
                return hadTruncatedPrefix ? TruncatedLogPrefix + combined : combined;

            int startIndex = combined.Length - MaxTaskLogHtmlLength;
            string trimmed = combined.Substring(startIndex);

            int firstBreak = trimmed.IndexOf("<br>", StringComparison.Ordinal);
            if (firstBreak >= 0 && firstBreak + 4 < trimmed.Length)
                trimmed = trimmed.Substring(firstBreak + 4);

            return TruncatedLogPrefix + trimmed;
        }

        private string LocalizeActivityText(string text)
        {
            // localize text
            string locText = GetSharedLocalizedString("TaskActivity." + text);
            if (locText != null)
                return locText;

            return text;
        }

        private string GetLogLineIdent(int ident)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ident; i++)
                sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            return sb.ToString();
        }

        private string GetDurationText(DateTime startDate, DateTime endDate)
        {
            TimeSpan duration = (endDate - startDate);
            return String.Format("{0}:{1}:{2}",
                duration.Hours.ToString().PadLeft(2, '0'),
                duration.Minutes.ToString().PadLeft(2, '0'),
                duration.Seconds.ToString().PadLeft(2, '0'));
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            RedirectToBrowsePage();
        }

        protected void btnStop_Click(object sender, EventArgs e)
        {
            // stop task
            ES.Services.Tasks.StopTask(PanelRequest.TaskID);

            // hide stop button
            btnStop.Visible = false;
        }
    }
}
