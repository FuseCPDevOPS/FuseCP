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
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.IO;
using FuseCP.Providers.OS;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
	public partial class MessageBox : FuseCPControlBase, IMessageBoxControl, INamingContainer
	{
		private const string ErrorIdContextKey = "FuseCPErrorId";
		private const string ErrorReportReferenceIdViewStateKey = "FuseCPErrorReportReferenceId";
		private const string ErrorReportMessageViewStateKey = "FuseCPErrorReportMessage";
		private const string ErrorReportDetailsViewStateKey = "FuseCPErrorReportDetails";
		private const string ErrorReportPageUrlViewStateKey = "FuseCPErrorReportPageUrl";
		private const string ErrorReportLoggedUserViewStateKey = "FuseCPErrorReportLoggedUser";
		private const string ErrorReportSelectedUserViewStateKey = "FuseCPErrorReportSelectedUser";
		private const string ErrorReportSpaceViewStateKey = "FuseCPErrorReportSpace";

		private const string ServerAdminUsername = "serveradmin";

		protected void Page_Load(object sender, EventArgs e)
		{
			//this.Visible = false;
			if (ViewState["ShowNextTime"] != null)
			{
				this.Visible = true;
				ViewState["ShowNextTime"] = null;
			}
		}

		public void RenderMessage(MessageBoxType messageType, string message, string description,
			 Exception ex, params string[] additionalParameters)
		{
			this.Visible = true; // show message

			// set icon and styles
			string boxStyle = "MessageBox Green";
			if (messageType == MessageBoxType.Warning)
				boxStyle = "MessageBox Yellow";
			else if (messageType == MessageBoxType.Error)
				boxStyle = "MessageBox Red";

			tblMessageBox.Attributes["class"] = boxStyle;

			// set texts
			string safeMessage = message;
			string safeDescription = description;
			string errorId = null;
			bool showDetailedError = false;
			bool canSendReport = false;

			// show exception
			if (ex != null || messageType == MessageBoxType.Error)
			{
				bool isServerAdmin = IsServerAdminUser();
				showDetailedError = isServerAdmin && ShouldShowDetailedErrors();
				canSendReport = !isServerAdmin;
				errorId = GetOrCreateErrorId();
				if (String.IsNullOrWhiteSpace(safeMessage))
					safeMessage = "An unexpected error occurred.";

				if (!showDetailedError)
					safeDescription = "Technical details are available in server logs. Reference ID: " + errorId;

				// show error
				try
				{
					string pageUrl = GetCurrentPageUrl();
					string loggedUser = GetUserDisplayName(PanelSecurity.LoggedUser);
					string selectedUser = GetUserDisplayName(PanelSecurity.SelectedUser);
					string activeSpace = GetActiveSpaceLabel();
					string detailsForAdmin = BuildExceptionDetails(ex, errorId, true, safeMessage, safeDescription);

					if (ex != null)
						System.Diagnostics.Trace.TraceError("FuseCP Portal error. Reference ID: {0}{1}{2}",
							errorId,
							Environment.NewLine,
							ex);

					StoreErrorReportContext(errorId, safeMessage, detailsForAdmin, pageUrl, loggedUser, selectedUser, activeSpace);

					litPageUrl.Text = PortalAntiXSS.Encode(pageUrl);
					litLoggedUser.Text = PortalAntiXSS.Encode(loggedUser);
					litSelectedUser.Text = PortalAntiXSS.Encode(selectedUser);
					litPackageName.Text = PortalAntiXSS.Encode(activeSpace);

					secTechnicalDetails.Visible = showDetailedError;
					TechnicalDetailsPanel.Visible = showDetailedError;
					tblTechnicalDetails.Visible = showDetailedError;
					if (showDetailedError)
						litStackTrace.Text = PortalAntiXSS.Encode(detailsForAdmin).Replace("\r\n", "<br/>");
					else
						litStackTrace.Text = String.Empty;

					secSendReport.Visible = canSendReport;
					SendReportPanel.Visible = canSendReport;
					btnSend.Visible = canSendReport;
					lblSentMessage.Visible = false;

					if (canSendReport)
					{
						litSendFrom.Text = PortalAntiXSS.Encode(GetReportFromAddress());
						litSendTo.Text = PortalAntiXSS.Encode(PortalUtils.AdminEmail ?? String.Empty);
						litSendCC.Text = String.Empty;
						litSendSubject.Text = PortalAntiXSS.Encode(GetReportSubject(errorId));
					}

					rowTechnicalDetails.Visible = showDetailedError || canSendReport;

				}
				catch (System.Exception catchEx) when (!(catchEx is System.OutOfMemoryException) && !(catchEx is System.StackOverflowException) && !(catchEx is System.AccessViolationException))
				{
				    _ = catchEx;
				}
			}
			else
			{
				rowTechnicalDetails.Visible = false;
				secTechnicalDetails.Visible = false;
				SendReportPanel.Visible = false;
				secSendReport.Visible = false;
				ClearErrorReportContext();
				safeDescription = String.Empty;
				if (String.IsNullOrWhiteSpace(safeMessage))
					safeMessage = messageType == MessageBoxType.Warning
						? "Operation completed with warnings."
						: "Operation completed successfully.";
			}

			litMessage.Text = HttpUtility.HtmlEncode(safeMessage);
			litDescription.Text = String.IsNullOrEmpty(safeDescription)
				 ? ""
				 : String.Format("<br/><span class=\"description\">{0}</span>", HttpUtility.HtmlEncode(safeDescription));
		}

		protected void btnSend_Click(object sender, EventArgs e)
		{
			EnableViewState = true;
			ViewState["ShowNextTime"] = true;

			try
			{
				if (IsServerAdminUser())
					throw new InvalidOperationException("Server administrator users do not send error reports from this panel.");

				if (String.IsNullOrWhiteSpace(PortalUtils.AdminEmail))
					throw new InvalidOperationException("AdminEmail is not configured.");

				btnSend.Visible = false;
				lblSentMessage.Visible = true;

				string emailMessage = BuildErrorReportEmailBody(txtSendComments.Text ?? String.Empty);
				string subject = GetReportSubject(ViewState[ErrorReportReferenceIdViewStateKey] as string);
				PortalUtils.SendMail(GetReportFromAddress(), PortalUtils.AdminEmail, null, subject, emailMessage, true);

				System.Diagnostics.Trace.TraceWarning("User error report sent to server admin. Reference ID: {0}",
					ViewState[ErrorReportReferenceIdViewStateKey] as string ?? String.Empty);

				lblSentMessage.Text = GetLocalizedString("Text.MessageSent");
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				System.Diagnostics.Trace.TraceWarning("Error report e-mail failed: {0}", ex);
				btnSend.Visible = true;
				lblSentMessage.Visible = true;
				lblSentMessage.Text = GetLocalizedString("Text.MessageSentError");
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override object SaveControlState()
		{
			return base.SaveControlState();
		}

		protected override void LoadControlState(object state)
		{
			base.LoadControlState(state);
		}

		private static bool ShouldShowDetailedErrors()
		{
			try
			{
				string explicitSetting = ConfigurationManager.AppSettings["FuseCP.WebPortal.ShowDetailedErrors"];
				if (!String.IsNullOrEmpty(explicitSetting))
					return explicitSetting.Equals("true", StringComparison.OrdinalIgnoreCase);

				string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				if (String.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
					return true;

				HttpContext context = HttpContext.Current;
				return context != null && context.Request != null && context.Request.IsLocal;
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				return false;
			}
		}

		private static bool IsServerAdminUser()
		{
			try
			{
				UserInfo loggedUser = PanelSecurity.LoggedUser;
				if (loggedUser == null)
					return false;

				if (loggedUser.Role == UserRole.Administrator)
					return true;

				return String.Equals(loggedUser.Username, ServerAdminUsername, StringComparison.OrdinalIgnoreCase);
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				_ = ex;
				return false;
			}
		}

		private static string GetOrCreateErrorId()
		{
			HttpContext context = HttpContext.Current;
			if (context == null)
				return Guid.NewGuid().ToString("N");

			if (context.Items[ErrorIdContextKey] is string cachedErrorId && !String.IsNullOrWhiteSpace(cachedErrorId))
				return cachedErrorId;

			string errorId = Guid.NewGuid().ToString("N");
			context.Items[ErrorIdContextKey] = errorId;
			return errorId;
		}

		private static string BuildExceptionDetails(Exception ex, string errorId, bool includeStackTrace, string messageText, string descriptionText)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Reference ID: " + errorId);

			if (ex == null)
			{
				sb.AppendLine("No exception object was provided by the caller.");
				if (!String.IsNullOrWhiteSpace(messageText))
					sb.AppendLine("Message: " + messageText);

				if (!String.IsNullOrWhiteSpace(descriptionText))
					sb.AppendLine("Description: " + descriptionText);

				sb.AppendLine("This typically means the operation failed with an error code rather than a thrown exception.");
				return sb.ToString();
			}

			int depth = 0;
			Exception current = ex;
			while (current != null && depth < 5)
			{
				sb.AppendLine(String.Format("{0}. {1}: {2}", depth + 1, current.GetType().FullName, current.Message));
				current = current.InnerException;
				depth++;
			}

			if (includeStackTrace)
			{
				sb.AppendLine();
				sb.AppendLine("Stack Trace:");
				sb.Append(ex.StackTrace ?? "No stack trace available.");
			}

			return sb.ToString();
		}

		private void StoreErrorReportContext(string errorId, string message, string details, string pageUrl, string loggedUser, string selectedUser, string activeSpace)
		{
			ViewState[ErrorReportReferenceIdViewStateKey] = errorId ?? String.Empty;
			ViewState[ErrorReportMessageViewStateKey] = message ?? String.Empty;
			ViewState[ErrorReportDetailsViewStateKey] = details ?? String.Empty;
			ViewState[ErrorReportPageUrlViewStateKey] = pageUrl ?? String.Empty;
			ViewState[ErrorReportLoggedUserViewStateKey] = loggedUser ?? String.Empty;
			ViewState[ErrorReportSelectedUserViewStateKey] = selectedUser ?? String.Empty;
			ViewState[ErrorReportSpaceViewStateKey] = activeSpace ?? String.Empty;
		}

		private void ClearErrorReportContext()
		{
			ViewState.Remove(ErrorReportReferenceIdViewStateKey);
			ViewState.Remove(ErrorReportMessageViewStateKey);
			ViewState.Remove(ErrorReportDetailsViewStateKey);
			ViewState.Remove(ErrorReportPageUrlViewStateKey);
			ViewState.Remove(ErrorReportLoggedUserViewStateKey);
			ViewState.Remove(ErrorReportSelectedUserViewStateKey);
			ViewState.Remove(ErrorReportSpaceViewStateKey);
		}

		private static string GetReportFromAddress()
		{
			if (!String.IsNullOrWhiteSpace(PortalUtils.FromEmail))
				return PortalUtils.FromEmail;

			if (!String.IsNullOrWhiteSpace(PortalUtils.AdminEmail))
				return PortalUtils.AdminEmail;

			return "noreply@localhost";
		}

		private string GetReportSubject(string errorId)
		{
			string subject = GetLocalizedString("Text.Subject");
			if (String.IsNullOrWhiteSpace(subject))
				subject = "FuseCP Error Report";

			if (!String.IsNullOrWhiteSpace(errorId))
				subject = subject + " [Ref: " + errorId + "]";

			return subject;
		}

		private string BuildErrorReportEmailBody(string comments)
		{
			string referenceId = ViewState[ErrorReportReferenceIdViewStateKey] as string ?? String.Empty;
			string message = ViewState[ErrorReportMessageViewStateKey] as string ?? String.Empty;
			string details = ViewState[ErrorReportDetailsViewStateKey] as string ?? String.Empty;
			string pageUrl = ViewState[ErrorReportPageUrlViewStateKey] as string ?? String.Empty;
			string loggedUser = ViewState[ErrorReportLoggedUserViewStateKey] as string ?? String.Empty;
			string selectedUser = ViewState[ErrorReportSelectedUserViewStateKey] as string ?? String.Empty;
			string activeSpace = ViewState[ErrorReportSpaceViewStateKey] as string ?? String.Empty;

			string encodedComments = HttpUtility.HtmlEncode(comments ?? String.Empty).Replace("\r\n", "<br/>").Replace("\n", "<br/>");
			string encodedDetails = HttpUtility.HtmlEncode(details).Replace("\r\n", "<br/>").Replace("\n", "<br/>");

			StringBuilder body = new StringBuilder();
			body.AppendLine("<html><head><title>FuseCP Error User Report</title></head><body>");
			body.AppendLine("<h1>FuseCP Error User Report</h1>");
			body.AppendLine("<p>A non-serveradmin user submitted an error report.</p>");
			body.AppendLine("<table cellpadding=\"4\" cellspacing=\"0\" border=\"1\">");
			body.AppendLine("<tr><td><strong>Reference ID</strong></td><td>" + HttpUtility.HtmlEncode(referenceId) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Message</strong></td><td>" + HttpUtility.HtmlEncode(message) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Page URL</strong></td><td>" + HttpUtility.HtmlEncode(pageUrl) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Logged User</strong></td><td>" + HttpUtility.HtmlEncode(loggedUser) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Work On Behalf</strong></td><td>" + HttpUtility.HtmlEncode(selectedUser) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Active Space</strong></td><td>" + HttpUtility.HtmlEncode(activeSpace) + "</td></tr>");
			body.AppendLine("<tr><td><strong>Personal Comments</strong></td><td>" + encodedComments + "</td></tr>");
			body.AppendLine("<tr><td><strong>Technical Details</strong></td><td><pre style=\"white-space:pre-wrap; margin:0;\">" + encodedDetails + "</pre></td></tr>");
			body.AppendLine("</table>");
			body.AppendLine("</body></html>");

			return body.ToString();
		}

		private static string GetCurrentPageUrl()
		{
			HttpRequest request = HttpContext.Current != null ? HttpContext.Current.Request : null;
			if (request == null)
				return String.Empty;

			if (request.Url == null)
				return request.RawUrl ?? String.Empty;

			return request.Url.ToString();
		}

		private static string GetUserDisplayName(UserInfo user)
		{
			if (user == null)
				return String.Empty;

			if (!String.IsNullOrWhiteSpace(user.Username))
				return user.Username;

			string fullName = String.Concat(user.FirstName, " ", user.LastName).Trim();
			return fullName;
		}

		private static string GetActiveSpaceLabel()
		{
			try
			{
				int packageId = PanelSecurity.PackageId;
				if (packageId <= 0)
					return String.Empty;

				PackageInfo package = ES.Services.Packages.GetPackage(packageId);
				if (package == null)
					return "#" + packageId;

				if (String.IsNullOrWhiteSpace(package.PackageName))
					return "#" + packageId;

				return package.PackageName + " (#" + packageId + ")";
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
				return String.Empty;
			}
		}

	}
}
