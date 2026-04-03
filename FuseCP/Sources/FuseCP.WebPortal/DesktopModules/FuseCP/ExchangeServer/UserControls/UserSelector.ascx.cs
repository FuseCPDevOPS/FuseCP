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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using FuseCP.Providers.HostedSolution;

namespace FuseCP.Portal.ExchangeServer.UserControls
{
    public partial class UserSelector : FuseCPControlBase
    {
        public const string DirectionString = "DirectionString";

        public bool IncludeMailboxes
        {
            get
            {
                object ret = ViewState["IncludeMailboxes"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["IncludeMailboxes"] = value;
            }
        }

        public bool IncludeMailboxesOnly
        {
            get
            {
                object ret = ViewState["IncludeMailboxesOnly"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["IncludeMailboxesOnly"] = value;
            }
        }


        public bool ExcludeOCSUsers
        {
            get
            {
                object ret = ViewState["ExcludeOCSUsers"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["ExcludeOCSUsers"] = value;
            }
        }

        public bool ExcludeLyncUsers
        {
            get
            {
                object ret = ViewState["ExcludeLyncUsers"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["ExcludeLyncUsers"] = value;
            }
        }

        public bool ExcludeSfBUsers
        {
            get
            {
                object ret = ViewState["ExcludeSfBUsers"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["ExcludeSfBUsers"] = value;
            }
        }


        public bool ExcludeBESUsers
        {
            get
            {
                object ret = ViewState["ExcludeBESUsers"];
                return (ret != null) && (bool)ret;
            }
            set
            {
                ViewState["ExcludeBESUsers"] = value;
            }
        }


        public int ExcludeAccountId
        {
            get { return PanelRequest.AccountID; }
        }

        public void SetAccount(OrganizationUser account)
        {
            BindSelectedAccount(account);
        }

        public string GetAccount()
        {
            return (string)ViewState["AccountName"];
        }

        public string GetSAMAccountName()
        {
            return (string)ViewState["SAMAccountName"];
        }


        public string GetDisplayName()
        {
            return (string)ViewState["DisplayName"];
        }

        public string GetPrimaryEmailAddress()
        {
            return (string)ViewState["PrimaryEmailAddress"];
        }

        public string GetSubscriberNumber()
        {
            return (string)ViewState["SubscriberNumber"];
        }


        public int GetAccountId()
        {
            return Utils.ParseInt(ViewState["AccountId"], 0);
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            // increase timeout
            ScriptManager scriptMngr = ScriptManager.GetCurrent(this.Page);
            scriptMngr.AsyncPostBackTimeout = 300;
        }

        private void BindSelectedAccount(OrganizationUser account)
        {
            if (account != null)
            {
                txtDisplayName.Text = account.DisplayName;
                ViewState["AccountName"] = account.AccountName;
                ViewState["DisplayName"] = account.DisplayName;
                ViewState["PrimaryEmailAddress"] = account.PrimaryEmailAddress;
                ViewState["AccountId"] = account.AccountId;
                ViewState["SAMAccountName"] = account.SamAccountName;
                ViewState["SubscriberNumber"] = account.SubscriberNumber;
            }
            else
            {
                txtDisplayName.Text = "";
                ViewState["AccountName"] = null;
                ViewState["DisplayName"] = null;
                ViewState["PrimaryEmailAddress"] = null;
                ViewState["AccountId"] = null;
                ViewState["SAMAccountName"] = null;
                ViewState["SubscriberNumber"] = null;
            }
        }

        public string GetAccountImage()
        {
            return GetThemedImage("Exchange/admin_16.png");
        }

        private void BindPopupAccounts()
        {
            OrganizationUser[] accounts = ES.Services.Organizations.SearchAccounts(PanelRequest.ItemID,
                ddlSearchColumn.SelectedValue, txtSearchValue.Text + "%", "", IncludeMailboxes);

            if (ExcludeAccountId > 0)
            {
                accounts = accounts.Where(account => account.AccountId != ExcludeAccountId).ToArray();
            }

            if (IncludeMailboxesOnly)
            {
                accounts = accounts.Where(account =>
                    account.ExternalEmail != string.Empty
                    && (!account.IsBlackBerryUser || !ExcludeBESUsers)
                    && (!account.IsLyncUser || !ExcludeLyncUsers)
                    && (!account.IsSfBUser || !ExcludeSfBUsers)).ToArray();
            }
            else
                if ((ExcludeOCSUsers) | (ExcludeBESUsers) | (ExcludeLyncUsers) | (ExcludeSfBUsers))
                {
                    accounts = accounts.Where(account =>
                        (!account.IsOCSUser || !ExcludeOCSUsers)
                        && (!account.IsLyncUser || !ExcludeLyncUsers)
                        && (!account.IsSfBUser || !ExcludeSfBUsers)
                        && (!account.IsBlackBerryUser || !ExcludeBESUsers)).ToArray();
                }


            Array.Sort(accounts, CompareAccount);
            if (Direction == SortDirection.Ascending)
            {
                Array.Reverse(accounts);
                Direction = SortDirection.Descending;
            }
            else
                Direction = SortDirection.Ascending;

            gvPopupAccounts.DataSource = accounts;
            gvPopupAccounts.DataBind();
        }

        private SortDirection Direction
        {
            get { return ViewState[DirectionString] == null ? SortDirection.Descending : (SortDirection)ViewState[DirectionString]; }
            set { ViewState[DirectionString] = value; }
        }

        private static int CompareAccount(OrganizationUser user1, OrganizationUser user2)
        {
            return string.Compare(user1.DisplayName, user2.DisplayName);
        }



        protected void chkIncludeMailboxes_CheckedChanged(object sender, EventArgs e)
        {
            BindPopupAccounts();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            BindPopupAccounts();
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            BindSelectedAccount(null);
        }

        protected void UserLookUp_Click(object sender, EventArgs e)
        {
            // bind all accounts
            BindPopupAccounts();

            // show modal
            SelectAccountsModal.Show();
        }

        protected void gvPopupAccounts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAccount")
            {
                int AccountId = Utils.ParseInt(e.CommandArgument.ToString());

                OrganizationUser account = ES.Services.Organizations.GetUserGeneralSettings(PanelRequest.ItemID, AccountId);

                // set account
                BindSelectedAccount(account);

                // hide popup
                SelectAccountsModal.Hide();

                // update parent panel
                MainUpdatePanel.Update();
            }
        }

        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {

            BindPopupAccounts();

        }

    }
}
