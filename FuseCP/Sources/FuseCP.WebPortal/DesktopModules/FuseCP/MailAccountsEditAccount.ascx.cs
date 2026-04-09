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
using System.Collections.Specialized;
using FuseCP.EnterpriseServer;
using FuseCP.Providers.Mail;
using System.Linq;

namespace FuseCP.Portal
{
    public partial class MailAccountsEditAccount : FuseCPModuleBase
    {
        MailAccount item = null;
        
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
                            item = ES.Services.MailServers.GetMailAccount(PanelRequest.ItemID);
						}
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            ShowErrorMessage("MAIL_GET_ACCOUNT", ex);
                            return;
                        }

                        if (item != null)
                        {
                            // save package info
                            ViewState["PackageId"] = item.PackageId;
                            mailEditAddress.PackageId = item.PackageId;
                            passwordControl.SetPackagePolicy(item.PackageId, UserSettings.MAIL_POLICY, "AccountPasswordPolicy");
                        }
                        else
                            RedirectToBrowsePage();
                    }
                    else
                    {
                        // new item
                        ViewState["PackageId"] = PanelSecurity.PackageId;
                        mailEditAddress.PackageId = PanelSecurity.PackageId;
                        passwordControl.SetPackagePolicy(PanelSecurity.PackageId, UserSettings.MAIL_POLICY, "AccountPasswordPolicy");
                    }
                }

                // load provider control
                LoadProviderControl((int)ViewState["PackageId"], "Mail", providerControl, "EditAccount.ascx");
                // load package context
                PackageContext cntx = PackagesHelper.GetCachedPackageContext((int)ViewState["PackageId"]);

                if (!IsPostBack)
                {
					// set messagebox size textbox visibility
					HandleMaxMailboxSizeLimitDisplay(cntx);
					// bind item to controls
					if (item != null)
					{
						// bind item to controls
						mailEditAddress.Email = item.Name;
						mailEditAddress.EditMode = true;
						passwordControl.EditMode = true;
						// Display currently set max mailbox size limit
						SetMaxMailboxSizeLimit(item.MaxMailboxSize);
						// other controls
                        if (providerControl.Controls.Count > 0)
                        {
                            IMailEditAccountControl ctrl = (IMailEditAccountControl)providerControl.Controls[0];
                            ctrl.BindItem(item);
                        }
					}
					else
					{
                        if (providerControl.Controls.Count == 0)
                        {
                            return;
                        }

                        IMailEditAccountControl ctrl = (IMailEditAccountControl)providerControl.Controls[0];

                        string[] settings = ES.Services.Servers.GetMailServiceSettingsByPackage((int)ViewState["PackageId"]) ?? Array.Empty<string>();
						StringDictionary settingsDictionary = ConvertArrayToDictionary(settings);



						MailAccount local_item = new MailAccount();
                        string isDomainAdminEnabled = settingsDictionary["isDomainAdminEnabled"];
                        if (isDomainAdminEnabled != null)
						{
                            local_item.IsDomainAdminEnabled = Convert.ToBoolean(isDomainAdminEnabled);
						}
						ctrl.BindItem(local_item);
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

		private void HandleMaxMailboxSizeLimitDisplay(PackageContext cntx)
		{
            if (cntx.Quotas.TryGetValue(Quotas.MAIL_DISABLESIZEEDIT, out var _ckv))
			{
				// Obtain quotas from the plan assigned
				bool maxMailboxSizeChangeable = (_ckv.QuotaAllocatedValue == 0);
				int maxMailboxSizeLimit = cntx.Quotas[Quotas.MAIL_MAXBOXSIZE].QuotaAllocatedValue;
                bool showReadonlyLimit = !maxMailboxSizeChangeable;
                lblMaxMailboxSizeLimit.Visible = showReadonlyLimit;
                txtMailBoxSizeLimit.Visible = !showReadonlyLimit;
				// Set the value being displayed for both controls in either case, as the logic above addresses all rendering concerns.
				SetMaxMailboxSizeLimit(maxMailboxSizeLimit);
				// Configure required field & range validators appropriately
				RequiredFieldValidator1.Enabled = txtMailBoxSizeLimit.Visible;
				MaxMailboxSizeLimitValidator.Enabled = txtMailBoxSizeLimit.Visible;
				CompareValidator1.Enabled = txtMailBoxSizeLimit.Visible;
				// Ensure the validator has its mimimun & maximum values adjusted correspondingly
				if (maxMailboxSizeLimit == -1 || maxMailboxSizeLimit == 0)
				{
					MaxMailboxSizeLimitValidator.Enabled = false;
				    CompareValidator1.Enabled = false;
				}
				else
				{
					MaxMailboxSizeLimitValidator.MinimumValue = "1";
					MaxMailboxSizeLimitValidator.MaximumValue = maxMailboxSizeLimit.ToString();
				}
				// Format the validator's error message
				MaxMailboxSizeLimitValidator.ErrorMessage = String.Format(MaxMailboxSizeLimitValidator.ErrorMessage,
					MaxMailboxSizeLimitValidator.MinimumValue, MaxMailboxSizeLimitValidator.MaximumValue);
			}
		}

		private void SetMaxMailboxSizeLimit(int sizeLimit)
		{
			txtMailBoxSizeLimit.Text = sizeLimit.ToString();
			// Ensure we use correct wording when the mailbox size limit is disabled for editing
			lblMaxMailboxSizeLimit.Text = sizeLimit == -1 || sizeLimit == 0 ? GetSharedLocalizedString("Text.Unlimited") : sizeLimit.ToString();



		}

        private void SaveItem()
        {
            if (!Page.IsValid)
                return;

            // get form data
            MailAccount account = new MailAccount();
            account.Id = PanelRequest.ItemID;
            account.PackageId = PanelSecurity.PackageId;
            account.Name = mailEditAddress.Email;
            account.Password = passwordControl.Password;
            account.MaxMailboxSize = Utils.ParseInt(txtMailBoxSizeLimit.Text);

            // Only check for conflicting names if creating new item
            if (PanelRequest.ItemID == 0)
            {
                //checking if account name is different from existing e-mail lists
                MailList[] lists = ES.Services.MailServers.GetMailLists(PanelSecurity.PackageId, true) ?? Array.Empty<MailList>();
                if (lists.Any(list => account.Name == list.Name))
                {
                    ShowWarningMessage("MAIL_ACCOUNT_NAME");
                    return;
                }

                //checking if account name is different from existing e-mail groups
                MailGroup[] mailgroups = ES.Services.MailServers.GetMailGroups(PanelSecurity.PackageId, true) ?? Array.Empty<MailGroup>();
                if (mailgroups.Any(group => account.Name == group.Name))
                {
                    ShowWarningMessage("MAIL_ACCOUNT_NAME");
                    return;
                }

                //checking if account name is different from existing forwardings
                MailAlias[] forwardings = ES.Services.MailServers.GetMailForwardings(PanelSecurity.PackageId, true) ?? Array.Empty<MailAlias>();
                if (forwardings.Any(forwarding => account.Name == forwarding.Name))
                {
                    ShowWarningMessage("MAIL_ACCOUNT_NAME");
                    return;
                }
            }

            // get other props
            if (providerControl.Controls.Count == 0)
            {
                ShowWarningMessage("INIT_SERVICE_ITEM_FORM");
                return;
            }
            IMailEditAccountControl ctrl = (IMailEditAccountControl)providerControl.Controls[0];
            ctrl.SaveItem(account);

            if (PanelRequest.ItemID == 0)
            {
                // new item
                try
                {
                    int result = ES.Services.MailServers.AddMailAccount(account);
                    if (result == BusinessErrorCodes.ERROR_MAIL_ACCOUNT_PASSWORD_NOT_COMPLEXITY)
                    {
                        ShowErrorMessage("MAIL_ACCOUNT_PASSWORD_NOT_COMPLEXITY");
                        return;
                    }
                    if (result == BusinessErrorCodes.ERROR_MAIL_LICENSE_DOMAIN_QUOTA)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                    if (result == BusinessErrorCodes.ERROR_MAIL_LICENSE_USERS_QUOTA)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_ADD_ACCOUNT", ex);
                    return;
                }
            }
            else
            {
                // existing item
                try
                {
                    int result = ES.Services.MailServers.UpdateMailAccount(account);
                    if (result == BusinessErrorCodes.ERROR_MAIL_ACCOUNT_PASSWORD_NOT_COMPLEXITY)
                    {
                        ShowErrorMessage("MAIL_ACCOUNT_PASSWORD_NOT_COMPLEXITY");
                        return;
                    }
                    if (result < 0)
                    {
                        ShowResultMessage(result);
                        return;
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    ShowErrorMessage("MAIL_UPDATE_ACCOUNT", ex);
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
                int result = ES.Services.MailServers.DeleteMailAccount(PanelRequest.ItemID);
                if (result < 0)
                {
                    ShowResultMessage(result);
                    return;
                }
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                ShowErrorMessage("MAIL_DELETE_ACCOUNT", ex);
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

		private StringDictionary ConvertArrayToDictionary(string[] settings)
		{
			StringDictionary r = new StringDictionary();
			foreach (string setting in settings)
			{
				int idx = setting.IndexOf('=');
				r.Add(setting.Substring(0, idx), setting.Substring(idx + 1));
			}
			return r;
		}
	}
}
