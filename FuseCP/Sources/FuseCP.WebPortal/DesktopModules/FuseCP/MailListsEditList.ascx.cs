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
    public partial class MailListsEditList : FuseCPModuleBase
    {
        MailList item = null;
    	private string listName = null;

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
                            item = ES.Services.MailServers.GetMailList(PanelRequest.ItemID);
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            ShowErrorMessage("MAIL_GET_LIST", ex);
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
                LoadProviderControl((int)ViewState["PackageId"], "Mail", providerControl, "EditList.ascx");

                if (!IsPostBack)
                {
                    // bind item to controls
                    if (item != null)
                    {
                        // bind item to controls
                        emailAddress.Email = item.Name;
                        emailAddress.EditMode = true;
                    	

                        // other controls
                        if (providerControl.Controls.Count > 0)
                        {
                            IMailEditListControl ctrl = (IMailEditListControl)providerControl.Controls[0];
                            ctrl.BindItem(item);
                        }
                    }
                }
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

			//MailList tempitem = ES.Services.MailServers.GetMailList(PanelRequest.ItemID);

            // get form data
            MailList local_item = new MailList();
            local_item.Id = PanelRequest.ItemID;
            local_item.PackageId = PanelSecurity.PackageId;
			if (listName != null)
			{
				local_item.Name = listName;
			}
			else
			{
				local_item.Name = emailAddress.Email;
			}
			//checking if list name is different from existing e-mail accounts
        MailAccount[] accounts = ES.Services.MailServers.GetMailAccounts(PanelSecurity.PackageId, true) ?? Array.Empty<MailAccount>();
            foreach (MailAccount account in accounts.Where(account => local_item.Name == account.Name))
            {
                    ShowWarningMessage("MAIL_LIST_NAME");
                    return;
            }
            //checking if list name is different from existing e-mail groups
        MailGroup[] mailgroups = ES.Services.MailServers.GetMailGroups(PanelSecurity.PackageId, true) ?? Array.Empty<MailGroup>();
            foreach (MailGroup group in mailgroups.Where(group => local_item.Name == group.Name))
            {
                    ShowWarningMessage("MAIL_LIST_NAME");
                    return;
            }

            //checking if list name is different from existing forwardings
        MailAlias[] forwardings = ES.Services.MailServers.GetMailForwardings(PanelSecurity.PackageId, true) ?? Array.Empty<MailAlias>();
            foreach (MailAlias forwarding in forwardings.Where(forwarding => local_item.Name == forwarding.Name))
            {
                    ShowWarningMessage("MAIL_LIST_NAME");
                    return;
            }
            
			// get other props
            if (providerControl.Controls.Count == 0)
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                return;
            }
            IMailEditListControl ctrl = (IMailEditListControl)providerControl.Controls[0];
			ctrl.SaveItem(local_item);

            if (PanelRequest.ItemID == 0)
            {
                // new local_item
                try
                {
                    int result = ES.Services.MailServers.AddMailList(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }

                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_ADD_LIST", ex);
                    return;
                }
            }
            else
            {
                // existing local_item
                try
                {
                    int result = ES.Services.MailServers.UpdateMailList(local_item);
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_UPDATE_LIST", ex);
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
                int result = ES.Services.MailServers.DeleteMailList(PanelRequest.ItemID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("MAIL_DELETE_LIST", ex);
                return;
            }

            // return
            RedirectSpaceHomePage();
        }

        private string GetDomainName(string email)
        {
            return email.Substring(email.IndexOf("@") + 1);
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
