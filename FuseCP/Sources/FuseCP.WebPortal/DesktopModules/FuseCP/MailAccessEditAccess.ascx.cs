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

using FuseCP.Providers.Mail;

namespace FuseCP.Portal
{
    public partial class MailAccessEditAccess : FuseCPModuleBase
    {
        MailDomain item = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            // bind item
            BindItem();
        }

        private void BindItem()
        {
            try
            {
                if (!IsPostBack && PanelRequest.ItemID > 0)
                {
                    {
                }

                // load provider control
                LoadProviderControl((int)ViewState["PackageId"], "Mail", providerControl, "EditAccess.ascx");

                if (!IsPostBack && item != null)
                {
                    {
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("MAIL_INIT_DOMAIN_FORM", ex);
                return;
            }
        }

        private void SaveItem()
        {
            if (!Page.IsValid)
                return;

            // get form data
            MailDomain local_item = new MailDomain();
            local_item.Id = PanelRequest.ItemID;
            local_item.PackageId = PanelSecurity.PackageId;

            // get other props
            if (providerControl.Controls.Count == 0)
            {
                ShowWarningMessage("MAIL_INIT_DOMAIN_FORM");
                return;
            }
            IMailEditDomainControl ctrl = (IMailEditDomainControl)providerControl.Controls[0];
            ctrl.SaveItem(local_item);

            // existing local_item
            try
            {
                int result = ES.Services.MailServers.UpdateMailDomain(local_item);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("MAIL_UPDATE_DOMAIN", ex);
                return;
            }

            // return
            RedirectSpaceHomePage();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SaveItem();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // return
            RedirectSpaceHomePage();
        }

    }
}
