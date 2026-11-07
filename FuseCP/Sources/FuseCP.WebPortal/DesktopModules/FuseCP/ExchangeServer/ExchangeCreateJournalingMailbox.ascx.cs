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
using System.Text;
using System.Security.Cryptography;
using System.Web.Security;
using System.Web.UI.WebControls;
using FuseCP.EnterpriseServer;
using FuseCP.Providers.HostedSolution;
using FuseCP.Providers.ResultObjects;

namespace FuseCP.Portal.ExchangeServer
{
    public partial class ExchangeCreateJournalingMailbox : FuseCPModuleBase
    {
        private bool IsNewUser
        {
            get
            {
                return !rbtnUserExistingUser.Checked;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PackageContext cntx = PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId);

            SetBranchVisibility(cntx);

            if (!IsPostBack)
            {
                BindPasswordSettings();

                ExchangeMailboxPlan[] plans = ES.Services.ExchangeServer.GetExchangeMailboxPlans(PanelRequest.ItemID, false);

                if (plans.Length == 0)
                    btnCreate.Enabled = false;

if (cntx.Quotas.TryGetValue(Quotas.EXCHANGE2007_ISCONSUMER, out var _ckv) && _ckv.QuotaAllocatedValue != 1)
{
                        locSubscriberNumber.Visible = txtSubscriberNumber.Visible = valRequireSubscriberNumber.Enabled = false;
}

                BindJournalingRecipients();
            }
        }

        private void BindJournalingRecipients()
        {
            Organization org = ES.Services.Organizations.GetOrganization(PanelRequest.ItemID);

            string path = org.SecurityGroup;
            string[] parts = path.Substring(path.ToUpper().IndexOf("DC=")).Split(',');
            StringBuilder domainBuilder = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                domainBuilder.Append(parts[i].Substring(3));
                if (i < parts.Length - 1)
                {
                    domainBuilder.Append('.');
                }
            }
            string domain = domainBuilder.ToString();

            ddlRecipient.Items.Clear();
            if (!String.IsNullOrEmpty(domain)) {
                string mail = org.OrganizationId + "@" + domain;
                ddlRecipient.Items.Add(new ListItem(GetLocalizedString("OrganizationGroup"), mail, true));
            }

            ExchangeAccount[] accounts = ES.Services.ExchangeServer.GetAccounts(PanelRequest.ItemID, ExchangeAccountType.DistributionList);
            foreach (ExchangeAccount account in accounts)
            {
                ddlRecipient.Items.Add(new ListItem(account.DisplayName + " - " + account.PrimaryEmailAddress, account.PrimaryEmailAddress));
            }
            accounts = ES.Services.ExchangeServer.GetAccounts(PanelRequest.ItemID, ExchangeAccountType.Mailbox);
            foreach (ExchangeAccount account in accounts)
            {
                ddlRecipient.Items.Add(new ListItem(account.DisplayName + " - " + account.PrimaryEmailAddress, account.PrimaryEmailAddress));
            }
            accounts = ES.Services.ExchangeServer.GetAccounts(PanelRequest.ItemID, ExchangeAccountType.SharedMailbox);
            foreach (ExchangeAccount account in accounts)
            {
                ddlRecipient.Items.Add(new ListItem(account.DisplayName + " - " + account.PrimaryEmailAddress, account.PrimaryEmailAddress));
            }
        }

        private void BindPasswordSettings()
        {
            var grainedPasswordSettigns = ES.Services.Organizations.GetOrganizationPasswordSettings(PanelRequest.ItemID);

            if (grainedPasswordSettigns != null)
            {
                password.SetUserPolicy(grainedPasswordSettigns);
            }
            else
            {
                messageBox.ShowErrorMessage("UNABLETOLOADPASSWORDSETTINGS");
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            CreateMailbox();
        }

        private void CreateMailbox()
        {
            if (!Page.IsValid)
                return;

            try
            {
                string name = IsNewUser ? email.AccountName : userSelector.GetPrimaryEmailAddress().Split('@')[0];
                string primryEmail = IsNewUser ? email.Email : userSelector.GetPrimaryEmailAddress();
                string displayName = IsNewUser ? txtDisplayName.Text.Trim() : userSelector.GetDisplayName();
                string accountName = IsNewUser ? string.Empty : userSelector.GetAccount();

                string domain = IsNewUser ? email.DomainName : userSelector.GetPrimaryEmailAddress().Split('@')[1];

                int accountId = IsNewUser ? 0 : userSelector.GetAccountId();

                string subscriberNumber = IsNewUser ? txtSubscriberNumber.Text.Trim() : userSelector.GetSubscriberNumber();

                var passwordString = password.Password;

                if (sendToControl.IsRequestSend && IsNewUser)
                {
                    passwordString = GenerateSecurePassword(16);
                }

                accountId = ES.Services.ExchangeServer.CreateMailbox(PanelRequest.ItemID, accountId, ExchangeAccountType.JournalingMailbox, accountName,
                    displayName, name, domain, passwordString, false, "", Convert.ToInt32(mailboxPlanSelector.MailboxPlanId), 0, subscriberNumber, false);


                if (accountId < 0)
                {
                    messageBox.ShowResultMessage(accountId);
                    return;
                }
                else
                {
                    if ((!string.IsNullOrEmpty(txtFirstName.Text)) || (!string.IsNullOrEmpty(txtLastName.Text)) || (!string.IsNullOrEmpty(txtInitials.Text)))
                    {
                        SetUserAttributes(accountId);
                    }

                    ES.Services.ExchangeServer.CreateJournalRule(PanelRequest.ItemID, primryEmail, ddlScope.SelectedValue, ddlRecipient.SelectedValue, cbEnabled.Checked);
                }

                if (sendToControl.SendEmail && IsNewUser)
                {
                    ES.Services.Organizations.SendUserPasswordRequestEmail(PanelRequest.ItemID, accountId, "User creation", sendToControl.Email, true);
                }
                else if (sendToControl.SendMobile && IsNewUser)
                {
                    ES.Services.Organizations.SendUserPasswordRequestSms(PanelRequest.ItemID, accountId, "User creation", sendToControl.Mobile);
                }

                Response.Redirect(EditUrl("AccountID", accountId.ToString(), "journaling_mailbox_settings",
                    "SpaceID=" + PanelSecurity.PackageId,
                    "ItemID=" + PanelRequest.ItemID,
                    "Context=JournalingMailbox"));
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messageBox.ShowErrorMessage("EXCHANGE_CREATE_JOURNALING_MAILBOX", ex);
            }
        }

        private void SetUserAttributes(int accountId)
        {
            OrganizationUser user = ES.Services.Organizations.GetUserGeneralSettings(PanelRequest.ItemID, accountId);

            ES.Services.Organizations.SetUserGeneralSettings(PanelRequest.ItemID, accountId, txtDisplayName.Text, null, false, user.Disabled, user.Locked,
                txtFirstName.Text, txtInitials.Text, txtLastName.Text, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, user.ExternalEmail, txtSubscriberNumber.Text, 0, false, false);
        }

        private static string GenerateSecurePassword(int length)
        {
            const string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*";

            if (length <= 0)
                return string.Empty;

            var data = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[data[i] % allowedChars.Length];
            }

            return new string(chars);
        }



        protected void rbtnUserExistingUser_CheckedChanged(object sender, EventArgs e)
        {
            SetBranchVisibility(PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId));
        }

        protected void rbtnCreateNewMailbox_CheckedChanged(object sender, EventArgs e)
        {
            SetBranchVisibility(PackagesHelper.GetCachedPackageContext(PanelSecurity.PackageId));
        }

        private void SetBranchVisibility(PackageContext cntx)
        {
            bool existingUser = rbtnUserExistingUser.Checked;

            // The user selector must always render so its event-validation tuple is registered on the initial GET;
            // branch visibility is handled with CSS instead of server-side Visible (F-040).
            NewUserDiv.Style["display"] = existingUser ? "none" : string.Empty;
            ExistingUserDiv.Style["display"] = existingUser ? string.Empty : "none";

            // WebFormsForCore validators do not inherit container visibility, so the inactive branch must be disabled explicitly.
            valRequireDisplayName.Enabled = !existingUser;

            bool requireSubscriberNumber = !(cntx.Quotas.TryGetValue(Quotas.EXCHANGE2007_ISCONSUMER, out var consumerQuota) && consumerQuota.QuotaAllocatedValue != 1);
            valRequireSubscriberNumber.Enabled = !existingUser && requireSubscriberNumber;

            email.ValidationEnabled = !existingUser;
            sendToControl.ValidationEnabled = !existingUser && sendToControl.IsRequestSend;
            password.ValidationEnabled = !existingUser;
        }

        protected void mailboxPlanSelector_Change(object sender, EventArgs e)
        {
        }

    }
}
