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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using FuseCP.EnterpriseServer;
using FuseCP.Portal;

namespace FuseCP.Portal.UserControls
{
    public partial class AuditLogControl : FuseCPControlBase
    {
        private string logSource;
        public string LogSource
        {
            get { return logSource; }
            set { logSource = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // set display preferences
            gvLog.PageSize = UsersHelper.GetDisplayItemsPerPage();

            // grid columns
            gvLog.Columns[4].Visible = String.IsNullOrEmpty(logSource);
            gvLog.Columns[6].Visible = PanelRequest.ItemID == 0;


            if (!IsPostBack)
            {
                try
                {
                    btnClearLog.Visible
                        = (PanelSecurity.EffectiveUser.Role == UserRole.Administrator);

                    // bind
                    BindPeriod();
                    BindSources();

                    // hide source if required
                    if (!String.IsNullOrEmpty(logSource))
                    {
                        ddlSource.SelectedValue = logSource;
                        SourceRow.Visible = false;
                    }

                    // tasks
                    BindSourceTasks();

                    // hide item name if required
                    if (PanelRequest.ItemID > 0)
                    {
                        ItemNameRow.Visible = false;
                        FilterButtonsRow.Visible = false;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    HostModule.ProcessException(ex);
                    return;
                }
            }
        }

        public string GetIconUrl(int severityID)
        {
            if (severityID == 1)
                return PortalUtils.GetThemedImage("warning_icon_small.gif");
            else if (severityID == 2)
                return PortalUtils.GetThemedImage("error_icon_small.gif");
            else
                return PortalUtils.GetThemedImage("information_icon_small.gif");
        }

        private void BindSources()
        {
            ddlSource.Items.Clear();
            ddlSource.Items.Add(new ListItem(GetLocalizedString("All.Text"), ""));
            DataSet sourceSet = ES.Services.AuditLog.GetAuditLogSources();
            DataTable dt = (sourceSet != null && sourceSet.Tables.Count > 0) ? sourceSet.Tables[0] : null;
            if (dt == null)
                return;

            foreach (DataRow dr in dt.Rows)
            {
                string sourceName = dr["SourceName"].ToString();
                ddlSource.Items.Add(new ListItem(GetAuditLogSourceName(sourceName), sourceName));
            }
        }

        private void BindSourceTasks()
        {
            string sourceName = ddlSource.SelectedValue;

            ddlTask.Items.Clear();
            ddlTask.Items.Add(new ListItem(GetLocalizedString("All.Text"), ""));
            DataSet taskSet = ES.Services.AuditLog.GetAuditLogTasks(sourceName);
            DataTable dt = (taskSet != null && taskSet.Tables.Count > 0) ? taskSet.Tables[0] : null;
            if (dt == null)
                return;

            foreach (DataRow dr in dt.Rows)
            {
                string taskName = dr["TaskName"].ToString();
                ddlTask.Items.Add(new ListItem(GetAuditLogTaskName(sourceName, taskName), taskName));
            }
        }

        protected void ddlSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSourceTasks();
        }

        private void BindPeriod()
        {
            if (calPeriod.SelectedDates.Count == 0)
                calPeriod.SelectedDate = DateTime.Now;

            DateTime startDate = calPeriod.SelectedDates[0];
            DateTime endDate = calPeriod.SelectedDates[calPeriod.SelectedDates.Count - 1];

            litPeriod.Text = startDate.ToString("MMM dd, yyyy") +
                " - " + endDate.ToString("MMM dd, yyyy");

            hidStartDate.Value = startDate.ToString();
            hidEndDate.Value = endDate.ToString();
        }

        private void ExportLog()
        {
            // build HTML
            DataSet recordsSet = ES.Services.AuditLog.GetAuditLogRecordsPaged(PanelSecurity.SelectedUserId,
                PanelSecurity.PackageId, PanelRequest.ItemID, txtItemName.Text.Trim(),
                DateTime.Parse(hidStartDate.Value),
                DateTime.Parse(hidEndDate.Value),
                Utils.ParseInt(ddlSeverity.SelectedValue, 0),
                ddlSource.SelectedValue, ddlTask.SelectedValue,
                "StartDate ASC", 0, Int32.MaxValue);
            DataTable dtRecords = (recordsSet != null && recordsSet.Tables.Count > 1) ? recordsSet.Tables[1] : null;
            if (dtRecords == null)
                return;

            StringBuilder sb = new StringBuilder();

            // header
            sb.AppendLine("Started,Finished,Severity,User-ID,Username,Source,Task,Item-Name,Execution-Log");

            foreach (DataRow dr in dtRecords.Rows)
            {
				// Started
                sb.AppendFormat("\"{0}\",", dr["StartDate"]);
				// Finished
                sb.AppendFormat("\"{0}\",", dr["FinishDate"]);
				// Severity
                sb.AppendFormat("\"{0}\",", 
					GetAuditLogRecordSeverityName((int)dr["SeverityID"]));
				// User-ID
				sb.AppendFormat("\"{0}\",", dr["UserID"]);
				// Username
                sb.AppendFormat("\"{0}\",", dr["Username"]);
                // Source
				sb.AppendFormat("\"{0}\",", 
					GetAuditLogSourceName((string)dr["SourceName"]));
                // Task
				sb.AppendFormat("\"{0}\",", 
					PortalAntiXSS.Encode(GetAuditLogTaskName((string)dr["SourceName"], (string)dr["TaskName"])));
				// Item-Name
                sb.AppendFormat("\"{0}\",", PortalAntiXSS.Encode(dr["ItemName"].ToString()));
				// Execution-Log
				string executionLog = FormatPlainTextExecutionLog(
					dr["ExecutionLog"].ToString(), DateTime.Parse(dr["StartDate"].ToString()));
				//
				executionLog = executionLog.Replace("\"", "\"\"");
				//
				sb.AppendFormat("\"{0}\"", executionLog);
				sb.AppendLine();
            }

            string cleanedPeriod = litPeriod.Text.Replace(" ", "").Replace("/", "-").Replace(",", "-");
            string fileName = "FCP-AuditLog-" + cleanedPeriod + ".csv";

            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/ms-excel";

            Response.Write(sb.ToString());

            Response.End();
        }

        private void ClearLog()
        {
            try
            {
                int result = ES.Services.AuditLog.DeleteAuditLogRecords(PanelSecurity.SelectedUserId,
                    0, txtItemName.Text.Trim(),
                    DateTime.Parse(hidStartDate.Value),
                    DateTime.Parse(hidEndDate.Value),
                    Utils.ParseInt(ddlSeverity.SelectedValue, 0),
                    ddlSource.SelectedValue, ddlTask.SelectedValue);

                if (result < 0)
                {
                    HostModule.ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                HostModule.ShowErrorMessage("AUDIT_CLEAR", ex);
                return;
            }
        }

        private void BindRecordDetails(string recordId)
        {
            // load task
            LogRecord record = ES.Services.AuditLog.GetAuditLogRecord(recordId);

            // guard against missing or deleted records
            if (record == null)
                return;

            litUsername.Text = record.Username;
            litTaskName.Text = GetAuditLogTaskName(record.SourceName, record.TaskName);
            litSourceName.Text = GetAuditLogSourceName(record.SourceName);
            litItemName.Text = record.ItemName;
            litStarted.Text = record.StartDate.ToString();
            litFinished.Text = record.FinishDate.ToString();

            litDuration.Text = GetDurationText(record.StartDate, record.FinishDate);

            litSeverity.Text = GetAuditLogRecordSeverityName(record.SeverityID);
            litLog.Text = !String.IsNullOrEmpty(record.ExecutionLog)
                ? FormatExecutionLog(record.ExecutionLog, record.StartDate)
                : String.Empty;
        }

		private string FormatPlainTextExecutionLog(string xmlLog, DateTime startDate)
		{
			StringBuilder sb = new StringBuilder();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlLog);

			XmlNodeList nodeRecords = doc.SelectNodes("/log/records/record");

			foreach (XmlNode nodeRecord in nodeRecords)
			{
				// read attributes
				DateTime date = DateTime.MinValue;
				int severity = 0;
				int ident = 0;

				if (nodeRecord.Attributes["date"] != null)
					date = DateTime.Parse(nodeRecord.Attributes["date"].Value,
						System.Globalization.CultureInfo.InvariantCulture);

				if (nodeRecord.Attributes["severity"] != null)
					severity = Int32.Parse(nodeRecord.Attributes["severity"].Value);

				if (nodeRecord.Attributes["ident"] != null)
					ident = Int32.Parse(nodeRecord.Attributes["ident"].Value);

				// Begin audit record
				sb.Append('\t', ident);
				sb.Append("......................");
				sb.AppendLine();
				// Timestamp
				sb.Append('\t', ident);
				sb.AppendFormat("Timestamp: {0}", GetDurationText(startDate, date));
				sb.AppendLine();

				// text
				XmlNode nodeText = nodeRecord.SelectSingleNode("text");

				// text parameters
				string[] prms = new string[0];
				XmlNodeList nodePrms = nodeRecord.SelectNodes("textParameters/value");
				if (nodePrms != null)
				{
					prms = new string[nodePrms.Count];
					for (int i = 0; i < nodePrms.Count; i++)
						prms[i] = nodePrms[i].InnerText;
				}

				// write text
				string recordClass = "Information";
				if (severity == 1)
					recordClass = "Warning";
				else if (severity == 2)
					recordClass = "Error";

				string text = nodeText != null ? nodeText.InnerText : String.Empty;

				// localize text
				string locText = GetSharedLocalizedString("TaskActivity." + text);
				if (locText != null)
					text = locText;

				// format parameters
				if (prms.Length > 0)
					text = String.Format(text, prms);
				// Severity
				sb.Append('\t', ident);
				sb.AppendFormat(String.Format("Severity: {0}", recordClass));
				sb.AppendLine();
				// Record text
				if (!String.IsNullOrEmpty(text))
				{
					sb.Append('\t', ident);
					sb.Append(text);
					sb.AppendLine();	
				}
				//
				XmlNode nodeStackTrace = nodeRecord.SelectSingleNode("stackTrace");
				// Record stack trace
				if (nodeStackTrace != null && !String.IsNullOrEmpty(nodeStackTrace.InnerText))
				{
					sb.Append('\t', ident);
					sb.Append(nodeStackTrace.InnerText);
					sb.AppendLine();
				}
				// End audit record
				sb.Append('\t', ident);
				sb.AppendLine();
			}
			// Replace each double-quote with 2*double-quote as per CSV specification.
			// See "http://en.wikipedia.org/wiki/Comma-separated_values#Basic_Rules" for further reference
			return sb.ToString();
		}

        private string FormatExecutionLog(string xmlLog, DateTime startDate)
        {
            StringBuilder sb = new StringBuilder();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlLog);

            XmlNodeList nodeRecords = doc.SelectNodes("/log/records/record");

            foreach (XmlNode nodeRecord in nodeRecords)
            {
                sb.Append("<div class=\"LogRecord\">");

                // read attributes
                DateTime date = DateTime.MinValue;
                int severity = 0;
                int ident = 0;

                if (nodeRecord.Attributes["date"] != null)
                    date = DateTime.Parse(nodeRecord.Attributes["date"].Value,
                        System.Globalization.CultureInfo.InvariantCulture);

                if (nodeRecord.Attributes["severity"] != null)
                    severity = Int32.Parse(nodeRecord.Attributes["severity"].Value);

                if (nodeRecord.Attributes["ident"] != null)
                    ident = Int32.Parse(nodeRecord.Attributes["ident"].Value);

                // date div
                sb.Append("<div class=\"Time\">");
                sb.Append(GetDurationText(startDate, date));
                sb.Append("</div>");

                // text
                XmlNode nodeText = nodeRecord.SelectSingleNode("text");

                // text parameters
                string[] prms = new string[0];
                XmlNodeList nodePrms = nodeRecord.SelectNodes("textParameters/value");
                if (nodePrms != null)
                {
                    prms = new string[nodePrms.Count];
                    for (int i = 0; i < nodePrms.Count; i++)
                        prms[i] = nodePrms[i].InnerText;
                }

                // write text
                int padding = 80 + ident * 20;
                string recordClass = "Information";
                if (severity == 1)
                    recordClass = "Warning";
                else if (severity == 2)
                    recordClass = "Error";

                string text = nodeText != null ? nodeText.InnerText : String.Empty;

                // localize text
                string locText = GetSharedLocalizedString("TaskActivity." + text);
                if (locText != null)
                    text = locText;

                if (!String.IsNullOrEmpty(text))
                    text = text.Replace("\n", "<br/>");

                // format parameters
                if (prms.Length > 0)
                    text = String.Format(text, prms);

                sb.Append("<div class=\"").Append(recordClass).Append("\" style=\"padding-left:");
                sb.Append(padding).Append("px;\">").Append(text);

                XmlNode nodeStackTrace = nodeRecord.SelectSingleNode("stackTrace");
                if (nodeStackTrace != null && !String.IsNullOrEmpty(nodeStackTrace.InnerText))
                {
                    sb.Append("<br/>");
                    sb.Append(nodeStackTrace.InnerText.Replace("\n", "<br>"));
                }

                sb.Append("</div></div>");
            }

            return sb.ToString();
        }

        private string GetDurationText(DateTime startDate, DateTime endDate)
        {
            TimeSpan duration = endDate - startDate;
            return String.Format("{0}:{1}:{2}",
                duration.Hours.ToString().PadLeft(2, '0'),
                duration.Minutes.ToString().PadLeft(2, '0'),
                duration.Seconds.ToString().PadLeft(2, '0'));
        }

        protected void calPeriod_SelectionChanged(object sender, EventArgs e)
        {
            BindPeriod();
        }

        protected void odsLog_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                HostModule.ProcessException(e.Exception);
                e.ExceptionHandled = true;
            }
        }

        protected void btnExportLog_Click(object sender, EventArgs e)
        {
            ExportLog();
        }

        protected void btnClearLog_Click(object sender, EventArgs e)
        {
            ClearLog();

            // rebind grid
            gvLog.DataBind();
        }

        protected void btnDisplay_Click(object sender, EventArgs e)
        {
            gvLog.DataBind();
        }

        /// <summary>
        /// Handler for the hidden detail button, triggered by client-side JavaScript
        /// when the user clicks an audit log task name link. The RecordID is passed
        /// via the hidRecordId hidden field to avoid ViewState issues with the GridView.
        /// </summary>
        protected void btnShowDetail_Click(object sender, EventArgs e)
        {
            string recordId = hidRecordId.Value;
            if (string.IsNullOrEmpty(recordId))
                return;

            try
            {
                BindRecordDetails(recordId);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                litLog.Text = "<div class='text-danger'>Error loading task details: " + Server.HtmlEncode(ex.Message) + "</div>";
            }

            // Show the Bootstrap 5 modal via client-side script after partial postback completes
            ScriptManager.RegisterStartupScript(updatePanelLog, typeof(AuditLogControl), "ShowTaskDetailsModal",
                "var el = document.getElementById('taskDetailsModal'); if (el && typeof bootstrap !== 'undefined') { bootstrap.Modal.getOrCreateInstance(el).show(); }", true);
        }
    }
}
