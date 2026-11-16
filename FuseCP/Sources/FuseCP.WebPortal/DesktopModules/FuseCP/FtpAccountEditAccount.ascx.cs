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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;
using FuseCP.Providers.FTP;

namespace FuseCP.Portal
{
    public partial class FtpAccountEditAccount : FuseCPModuleBase
    {
        FtpAccount item = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            btnDelete.Visible = (PanelRequest.ItemID > 0);

            // bind item
            BindItem();
        }

        private void BindItem()
        {
            try
            {
                if (!IsPostBack)
                {
                    // load item if required
                    if (PanelRequest.ItemID > 0)
                    {
                        // existing item
                        try
                        {
                            item = ES.Services.FtpServers.GetFtpAccount(PanelRequest.ItemID);
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            ShowErrorMessage("FTP_GET_ACCOUNT", ex);
                            return;
                        }

                        if (item != null)
                        {
                            // save package info
                            ViewState["PackageId"] = item.PackageId;
                            fileLookup.PackageId = item.PackageId;
                            passwordControl.SetPackagePolicy(item.PackageId, UserSettings.FTP_POLICY, "UserPasswordPolicy");
                        }
                        else
                            RedirectToBrowsePage();
                    }
                    else
                    {
                        // new item
                        ViewState["PackageId"] = PanelSecurity.PackageId;
                        fileLookup.PackageId = PanelSecurity.PackageId;
                        fileLookup.SelectedFile = "\\";
                        usernameControl.SetPackagePolicy(PanelSecurity.PackageId, UserSettings.FTP_POLICY, "UserNamePolicy");
                        passwordControl.SetPackagePolicy(PanelSecurity.PackageId, UserSettings.FTP_POLICY, "UserPasswordPolicy");
                    }
                }

                // load provider control
                LoadProviderControl((int)ViewState["PackageId"], "Ftp", providerControl, "EditAccount.ascx");

                if (!IsPostBack && item != null)
                {
                    // bind item to controls
                    // bind item to controls
                    usernameControl.Text = item.Name;
                    usernameControl.EditMode = true;
                    passwordControl.EditMode = true;
                    fileLookup.SelectedFile = item.Folder;

                    // other controls
                    if (providerControl.Controls.Count > 0)
                    {
                        IFtpAccountEditControl ctrl = (IFtpAccountEditControl)providerControl.Controls[0];
                        ctrl.BindItem(item);
                    }
                }

                // WebFormsForCore does not reliably restore the UsernameControl's EditMode
                // ViewState across postbacks. Without re-applying it here (during the parent
                // Page_Load, before validation runs), the username field reverts to a visible
                // empty textbox after the first postback and the required-username validator
                // blocks the save. The account name is not user-editable when editing an
                // existing account.
                if (IsPostBack && PanelRequest.ItemID > 0)
                {
                    usernameControl.EditMode = true;
                    passwordControl.EditMode = true;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                DisableFormControls(this, btnCancel);
            }
        }

        private void SaveItem()
        {
            if (!Page.IsValid)
                return;

            // get form data
            FtpAccount local_item = new FtpAccount();
            local_item.Id = PanelRequest.ItemID;
            local_item.PackageId = PanelSecurity.PackageId;
            local_item.Name = usernameControl.Text;
            // In EditMode the username is shown read-only (a hidden textbox), so
            // usernameControl.Text comes back empty on the save postback (WebFormsForCore does
            // not reliably restore the value). The username is not user-editable when editing
            // an existing account, so restore the authoritative name from the record.
            if (PanelRequest.ItemID > 0 && String.IsNullOrEmpty(local_item.Name))
            {
                try
                {
                    FtpAccount existing = ES.Services.FtpServers.GetFtpAccount(PanelRequest.ItemID);
                    if (existing != null)
                        local_item.Name = existing.Name;
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    _ = ex;
                }
            }
            local_item.Password = passwordControl.Password;
            local_item.Folder = fileLookup.SelectedFile;

            // get other props
            if (providerControl.Controls.Count == 0)
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                return;
            }
            IFtpAccountEditControl ctrl = (IFtpAccountEditControl)providerControl.Controls[0];
            ctrl.SaveItem(local_item);

            if (PanelRequest.ItemID == 0)
            {
                try
                {
                    // new local_item
                    int result = ES.Services.FtpServers.AddFtpAccount(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    // F-018: the host's Windows local-account policy can be stricter than the
                    // panel's advertised password rules. When the OS rejects the password,
                    // surface the host's reason instead of a generic "Error creating FTP account".
                    if (IsPasswordPolicyRejection(ex))
                    {
                        ShowErrorMessage("FTP_PASSWORD_POLICY_REJECTED", ex);
                        return;
                    }
                    ShowErrorMessage("FTP_ADD_ACCOUNT", ex);
                    return;
                }
            }
            else
            {
                try
                {
                    // existing local_item
                    int result = ES.Services.FtpServers.UpdateFtpAccount(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    // F-018: surface the host's password-policy reason instead of a generic error.
                    if (IsPasswordPolicyRejection(ex))
                    {
                        ShowErrorMessage("FTP_PASSWORD_POLICY_REJECTED", ex);
                        return;
                    }
                    ShowErrorMessage("FTP_UPDATE_ACCOUNT", ex);
                    return;
                }
            }

            // return
            RedirectSpaceHomePage();
        }

        // F-018: detect a host-side password-policy rejection anywhere in the exception chain.
        // The OS (WinNT/AD SetPassword) throws messages such as
        // "The password does not meet the password policy requirements." which SecurityUtils
        // wraps in a generic exception. We match on the presence of both "password" and
        // "policy" in any message in the chain so the host's reason can be surfaced to the user.
        private static bool IsPasswordPolicyRejection(System.Exception ex)
        {
            System.Exception current = ex;
            int depth = 0;
            while (current != null && depth < 5)
            {
                string message = current.Message ?? String.Empty;
                string lower = message.ToLowerInvariant();
                if (lower.Contains("password") && lower.Contains("policy"))
                    return true;
                current = current.InnerException;
                depth++;
            }
            return false;
        }

        private void DeleteItem()
        {
            // delete
            try
            {
                int result = ES.Services.FtpServers.DeleteFtpAccount(PanelRequest.ItemID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("FTP_DELETE_ACCOUNT", ex);
                return;
            }

            // return
            RedirectSpaceHomePage();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // return
            RedirectSpaceHomePage();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteItem();
        }
    }
}