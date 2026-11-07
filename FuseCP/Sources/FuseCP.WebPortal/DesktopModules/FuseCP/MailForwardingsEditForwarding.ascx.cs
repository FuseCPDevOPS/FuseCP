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
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.Providers.Mail;

namespace FuseCP.Portal
{
    public partial class MailForwardingsEditForwarding : FuseCPModuleBase
    {
        MailAlias item = null;

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
                            item = ES.Services.MailServers.GetMailForwarding(PanelRequest.ItemID);
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            ShowErrorMessage("MAIL_GET_FORWARDING", ex);
                            return;
                        }

                        if (item != null)
                        {
                            // save package info
                            ViewState["PackageId"] = item.PackageId;
                            emailAddress.PackageId = item.PackageId;
                        }
                        else
                            RedirectToBrowsePage();
                    }
                    else
                    {
                        // new item
                        ViewState["PackageId"] = PanelSecurity.PackageId;
                        emailAddress.PackageId = PanelSecurity.PackageId;
                    }
                }

                // load provider control
                LoadProviderControl((int)ViewState["PackageId"], "Mail", providerControl, "EditForwarding.ascx");

                if (!IsPostBack && item != null)
                {
                    // bind item to controls
                    emailAddress.Email = item.Name;
                    emailAddress.EditMode = true;

                    txtForwardTo.Text = item.ForwardTo;

                    // other controls
                    if (providerControl.Controls.Count > 0)
                    {
                        IMailEditForwardingControl ctrl = (IMailEditForwardingControl)providerControl.Controls[0];
                        ctrl.BindItem(item);
                    }
                }

                // WebFormsForCore does not reliably restore the EditMode ViewState for
                // MailEditAddress across postbacks (it is written in Page_Load, after
                // TrackViewState has already run). Without re-applying it here (during the
                // parent Page_Load, before validation runs), the address field reverts to an
                // editable create-mode textbox after the save postback, the create-mode
                // validators fire against the empty hidden fields, and the update is silently
                // dropped (same class as F-027). The forwarding name is not user-editable when
                // editing an existing forwarding.
                if (IsPostBack && PanelRequest.ItemID > 0)
                    emailAddress.EditMode = true;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                DisableFormControls(this, btnCancel);
                return;
            }
        }

        private void SaveItem()
        {
            if (!Page.IsValid)
                return;

            // get form data
            MailAlias local_item = new MailAlias();
            local_item.Id = PanelRequest.ItemID;
            local_item.PackageId = PanelSecurity.PackageId;
            local_item.Name = emailAddress.Email;
            // In EditMode the address is shown read-only (a label), so emailAddress.Email
            // comes back empty on the save postback (WebFormsForCore does not reliably restore
            // the value). The forwarding name is not user-editable when editing an existing
            // forwarding, so restore the authoritative name from the record.
            if (PanelRequest.ItemID > 0 && String.IsNullOrEmpty(local_item.Name))
            {
                try
                {
                    MailAlias existing = ES.Services.MailServers.GetMailForwarding(PanelRequest.ItemID);
                    if (existing != null)
                        local_item.Name = existing.Name;
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    _ = ex;
                }
            }
            local_item.ForwardTo = txtForwardTo.Text.Trim();

                //checking if forwarding name is different from existing e-mail accounts
                MailAccount[] accounts = ES.Services.MailServers.GetMailAccounts(PanelSecurity.PackageId, true) ?? Array.Empty<MailAccount>();
            if (accounts.Any(account => local_item.Name == account.Name))
            {
                ShowWarningMessage("MAIL_FORW_NAME");
                return;
            }

            //checking if forwarding name is different from existing e-mail lists
                MailList[] lists = ES.Services.MailServers.GetMailLists(PanelSecurity.PackageId, true) ?? Array.Empty<MailList>();
            if (lists.Any(list => local_item.Name == list.Name))
            {
                ShowWarningMessage("MAIL_FORW_NAME");
                return;
            }

            //checking if forwarding name is different from existing e-mail groups
                MailGroup[] mailgroups = ES.Services.MailServers.GetMailGroups(PanelSecurity.PackageId, true) ?? Array.Empty<MailGroup>();
            if (mailgroups.Any(group => local_item.Name == group.Name))
            {
                ShowWarningMessage("MAIL_FORW_NAME");
                return;
            }

            // get other props
            if (providerControl.Controls.Count == 0)
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                return;
            }
            IMailEditForwardingControl ctrl = (IMailEditForwardingControl)providerControl.Controls[0];
            ctrl.SaveItem(local_item);

            if (PanelRequest.ItemID == 0)
            {
                // new local_item
                try
                {
                    int result = ES.Services.MailServers.AddMailForwarding(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }

                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_ADD_FORWARDING", ex);
                    return;
                }
            }
            else
            {
                // existing local_item
                try
                {
                    int result = ES.Services.MailServers.UpdateMailForwarding(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_UPDATE_FORWARDING", ex);
                    return;
                }
            }

            // return
            RedirectSpaceHomePage();
        }

        private void DeleteItem()
        {
            // delete
            try
            {
                int result = ES.Services.MailServers.DeleteMailForwarding(PanelRequest.ItemID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("MAIL_DELETE_FORWARDING", ex);
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