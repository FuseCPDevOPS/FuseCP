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

namespace FuseCP.Portal
{
    public partial class ServersEditReboot : FuseCPModuleBase
    {
        private const string ViewStateServerIdKey = "ServersEditReboot_ServerId";
        private const string RebootHandledItemKey = "ServersEditReboot_RebootHandled";

        private int CurrentServerId
        {
            get
            {
                if (ViewState[ViewStateServerIdKey] is int serverId && serverId > 0)
                    return serverId;

                serverId = PanelRequest.ServerId;
                if (serverId > 0)
                    ViewState[ViewStateServerIdKey] = serverId;

                return serverId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack && !IsRebootHandled && !IsCancelSubmit())
            {
                ExecuteReboot();
                return;
            }

            if (!IsPostBack && CurrentServerId <= 0)
            {
                btnReboot.Enabled = false;
                ShowErrorMessage("SERVER_REBOOT", new InvalidOperationException("Missing ServerID in request."));
            }
        }

        protected void btnReboot_Click(object sender, EventArgs e)
        {
            ExecuteReboot();
        }

        private bool IsRebootHandled
        {
            get
            {
                return HttpContext.Current?.Items[RebootHandledItemKey] is bool handled && handled;
            }
            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items[RebootHandledItemKey] = value;
            }
        }

        private bool IsCancelSubmit()
        {
            var form = Request?.Form;
            if (form == null || form.Count == 0)
                return false;

            string uniqueId = btnCancel?.UniqueID;
            string id = btnCancel?.ID;
            string clientId = btnCancel?.ClientID;
            string eventTarget = form["__EVENTTARGET"];

            if (!String.IsNullOrEmpty(uniqueId) && form[uniqueId] != null)
                return true;

            if (!String.IsNullOrEmpty(id) && form[id] != null)
                return true;

            if (!String.IsNullOrEmpty(clientId) && form[clientId] != null)
                return true;

            if (!String.IsNullOrEmpty(eventTarget))
            {
                if (!String.IsNullOrEmpty(uniqueId) && String.Equals(eventTarget, uniqueId, StringComparison.Ordinal))
                    return true;

                if (!String.IsNullOrEmpty(id) &&
                    (String.Equals(eventTarget, id, StringComparison.Ordinal) || eventTarget.EndsWith("$" + id, StringComparison.Ordinal)))
                    return true;

                if (!String.IsNullOrEmpty(clientId) &&
                    (String.Equals(eventTarget, clientId, StringComparison.Ordinal) || eventTarget.EndsWith("_" + clientId, StringComparison.Ordinal)))
                    return true;
            }

            return false;
        }

        private void ExecuteReboot()
        {
            try
            {
                if (IsRebootHandled)
                    return;

                IsRebootHandled = true;
                int serverId = CurrentServerId;

                if (serverId <= 0)
                {
                    ShowErrorMessage("SERVER_REBOOT", new InvalidOperationException("Missing ServerID in request."));
                    return;
                }

                int result = ES.Services.Servers.RebootSystem(serverId);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
                else
                {
                    ShowSuccessMessage("SERVER_REBOOT");
                    btnReboot.Enabled = false;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("SERVER_REBOOT", ex);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("ServerID", CurrentServerId.ToString(), "edit_server"));
        }
    }
}
