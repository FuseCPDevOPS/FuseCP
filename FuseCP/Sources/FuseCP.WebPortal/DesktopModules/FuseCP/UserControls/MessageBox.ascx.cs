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

namespace FuseCP.Portal
{
	public partial class MessageBox : FuseCPControlBase, IMessageBoxControl, INamingContainer
	{
		private const string ErrorReportBodyTemplate = @"
<html>
	<head>
		<title>FuseCP Error User Report</title>
	</head>
	<body>

		<h1>FuseCP Error User Report</h1>

		<p>
			An application error was encountered. Technical details are available in server logs.<br/>
			Personal Comments: %Comments%<br/>
		</p>

	</body>
</html>";

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
			var safeMessage = message;
			var safeDescription = description;

			// show exception
			if (ex != null)
			{
				safeMessage = "An unexpected error occurred.";
				safeDescription = String.Empty;
				// show error
				try
				{
					System.Diagnostics.Trace.TraceWarning("MessageBox exception details were captured for diagnostics.");
					litPageUrl.Text = String.Empty;
					litLoggedUser.Text = String.Empty;
					litSelectedUser.Text = String.Empty;
					litPackageName.Text = String.Empty;
					litStackTrace.Text = PortalAntiXSS.Encode("Technical details are available in server logs.");

					// send form
					litSendFrom.Text = String.Empty;

					litSendTo.Text = String.Empty;
					litSendCC.Text = String.Empty;
					litSendSubject.Text = GetLocalizedString("Text.Subject");

				}
				catch (System.Exception catchEx) when (!(catchEx is System.OutOfMemoryException) && !(catchEx is System.StackOverflowException) && !(catchEx is System.AccessViolationException))
				{
				    _ = catchEx;
				}
			}
			else
			{
				rowTechnicalDetails.Visible = false;
			}

			litMessage.Text = HttpUtility.HtmlEncode(safeMessage);
			litDescription.Text = !String.IsNullOrEmpty(safeDescription)
				 ? String.Format("<br/><span class=\"description\">{0}</span>", HttpUtility.HtmlEncode(safeDescription)) : "";
		}

		protected void btnSend_Click(object sender, EventArgs e)
		{
			EnableViewState = true;
			ViewState["ShowNextTime"] = true;

			try
			{
				btnSend.Visible = false;
				lblSentMessage.Visible = true;

				var from = !String.IsNullOrEmpty(PortalUtils.FromEmail) ? PortalUtils.FromEmail : PortalUtils.AdminEmail;
				var to = PortalUtils.AdminEmail;
				var subject = GetLocalizedString("Text.Subject");
				var encodedComments = PortalAntiXSS.Encode(txtSendComments.Text ?? String.Empty).Replace("\n", "<br/>\n");
				var emailMessage = ErrorReportBodyTemplate.Replace("%Comments%", $"<p>{encodedComments}</p>");

				// send mail
				PortalUtils.SendMail(from, to, String.Empty, subject, emailMessage, true);

				lblSentMessage.Text = GetLocalizedString("Text.MessageSent");
			}
			catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
			{
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

	}
}
