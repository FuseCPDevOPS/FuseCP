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
using System.Text;
using FuseCP.Server.Utils;
using FuseCP.Providers.Utils;
using Microsoft.Win32;
using System.Linq;


namespace FuseCP.Providers.Statistics
{
    public class SmarterStats : HostingServiceProviderBase, IStatisticsServer
    {
        private ISmarterStatsAutomationClient automationClient;

        #region Properties
        protected string SmarterUrl
        {
            get { return ProviderSettings["SmarterUrl"]; }
        }

        protected string Username
        {
            get { return ProviderSettings["Username"]; }
        }

        protected string Password
        {
            get { return ProviderSettings["Password"]; }
        }

        protected int ServerId
        {
            get { try { return Int32.Parse(ProviderSettings["ServerID"]); } catch { return 1; } }
        }

        protected string LogFormat
        {
            get { return ProviderSettings["LogFormat"]; }
        }

        protected string LogWildcard
        {
            get { return ProviderSettings["LogWildcard"]; }
        }

        protected int LogDeleteDays
        {
            get { try { return Int32.Parse(ProviderSettings["LogDeleteDays"]); } catch { return 0; } }
        }

        protected string SmarterLogsPath
        {
            get { return FileUtils.EvaluateSystemVariables(ProviderSettings["SmarterLogsPath"]); }
        }

        protected int SmarterLogDeleteMonths
        {
            get { try { return Int32.Parse(ProviderSettings["SmarterLogDeleteMonths"]); } catch { return 0; } }
        }

        protected int TimeZoneId
        {
            get { try { return Int32.Parse(ProviderSettings["TimeZoneId"]); } catch { return 1; } }
        }

		protected string StatisticsUrl
		{
			get { return ProviderSettings["StatisticsUrl"]; }
		}

        protected string AutomationTransport
        {
            get
            {
                string transport = ProviderSettings["AutomationTransport"];
                return String.IsNullOrWhiteSpace(transport) ? "Soap" : transport.Trim();
            }
        }

        protected string ModernAutomationEndpoint
        {
            get
            {
                string endpoint = ProviderSettings["ModernAutomationEndpoint"];
                return String.IsNullOrWhiteSpace(endpoint) ? SmarterUrl : endpoint.Trim();
            }
        }

        protected ISmarterStatsAutomationClient AutomationClient
        {
            get
            {
                if (automationClient == null)
                    automationClient = CreateAutomationClient();

                return automationClient;
            }
        }
        #endregion

        #region IStatistics methods
        public virtual StatsServer[] GetServers()
        {
            ServerInfoArrayResult result = AutomationClient.GetServers(Username, Password);
            if (result.Servers == null)
                return new StatsServer[] { };

            StatsServer[] servers = new StatsServer[result.Servers.Length];
            for (int i = 0; i < servers.Length; i++)
            {
                servers[i] = new StatsServer();
                servers[i].ServerId = result.Servers[i].ServerID;
                servers[i].Name = result.Servers[i].Name;
                servers[i].Ip = result.Servers[i].IP;
            }

            return servers;
        }

        public virtual string GetSiteId(string siteName)
        {
            SiteInfoArrayResult result = AutomationClient.GetAllSites(Username, Password, false);
            if (result.Sites == null)
                return null;

            var site = result.Sites.FirstOrDefault(item => String.Compare(siteName, item.DomainName, 0) == 0);
            return site != null ? site.SiteID.ToString() : null;
        }

        public virtual string[] GetSites()
        {
            List<string> sites = new List<string>();

            SiteInfoArrayResult result = AutomationClient.GetAllSites(Username, Password, false);
            if (result.Sites == null)
                return sites.ToArray();

            foreach (SiteInfo site in result.Sites)
                sites.Add(site.DomainName);

            return sites.ToArray();
        }

        public virtual StatsSite GetSite(string siteId)
        {
            int sid = Int32.Parse(siteId);
            SiteInfoResult result = AutomationClient.GetSite(Username, Password, sid);
            if (result.Site == null)
                return null;

            StatsSite site = new StatsSite();
            site.Name = result.Site.DomainName;
            site.ExportPath = result.Site.ExportPath;
            site.ExportPathUrl = result.Site.ExportPathURL;
            site.LogDirectory = result.Site.LogDirectory;
            site.TimeZoneId = TimeZoneId;
            site.Status = result.Site.SiteStatus;

			// process stats URL
			string url = null;
			if (!String.IsNullOrEmpty(StatisticsUrl))
			{
				url = StringUtils.ReplaceStringVariable(StatisticsUrl, "domain_name", site.Name);
				url = StringUtils.ReplaceStringVariable(url, "site_id", siteId);
			}

            // get site users
            UserInfoResultArray usrResult = AutomationClient.GetUsers(Username, Password, sid);
            if (usrResult.user != null)
            {
                site.Users = new StatsUser[usrResult.user.Length];
                for (int i = 0; i < site.Users.Length; i++)
                {
                    site.Users[i] = new StatsUser();
                    site.Users[i].Username = usrResult.user[i].UserName;
                    site.Users[i].Password = usrResult.user[i].Password;
                    site.Users[i].FirstName = usrResult.user[i].FirstName;
                    site.Users[i].LastName = usrResult.user[i].LastName;
                    site.Users[i].IsAdmin = usrResult.user[i].IsAdmin;
                    site.Users[i].IsOwner = usrResult.user[i].IsSiteOwner;
                }

				if (site.Users.Length > 0 && !String.IsNullOrEmpty(url))
				{
					url = StringUtils.ReplaceStringVariable(url, "username", site.Users[0].Username);
					url = StringUtils.ReplaceStringVariable(url, "password", site.Users[0].Password);
				}
            }

			site.StatisticsUrl = url;

            return site;
        }


        public virtual string AddSite(StatsSite site)
        {
            // generate unique SiteID
            //int iSiteId = site.Name.GetHashCode();
            //if (iSiteId < 0)
            //     iSiteId *= -1;

            //string siteId = iSiteId.ToString();

            // create logs folder if not exists
            //if (!FileUtils.DirectoryExists(site.LogDirectory))
            //    FileUtils.CreateDirectory(site.LogDirectory);

            // add site
            if (site.Users == null || site.Users.Length == 0)
                throw new Exception("At least one user (site owner) should be specified when creating new statistics site");

            string ownerUsername = site.Users[0].Username.ToLower();
            GenericResult1 result = AutomationClient.AddSite(Username, Password,
                site.Users[0].Username, site.Users[0].Password, site.Users[0].FirstName, site.Users[0].LastName,
                ServerId, 0, site.Name, site.LogDirectory, LogFormat, LogWildcard, LogDeleteDays,
                SmarterLogsPath, SmarterLogDeleteMonths, "", "", TimeZoneId);

            if (!result.Result)
                throw new Exception("Error creating statistics site: " + result.Message);

            string siteId = GetSiteId(site.Name);

            int iSiteId = Int32.Parse(siteId);

            // add other users
            foreach (StatsUser user in site.Users.Where(user => user.Username.ToLower() != ownerUsername))
            {
                    // add user
                    GenericResult2 r = AutomationClient.AddUser(Username, Password, iSiteId,
                        user.Username, user.Password, user.FirstName, user.LastName, user.IsAdmin);
                    if (!r.Result)
                        throw new Exception("Error adding site user: " + r.Message);
            }

            return siteId;
        }

        public virtual void UpdateSite(StatsSite site)
        {
            int siteId = Int32.Parse(site.SiteId);

            // get original site
            SiteInfoResult siteResult = AutomationClient.GetSite(Username, Password, siteId);
            if (siteResult.Site == null)
                return;

            SiteInfo origSite = siteResult.Site;

            // update site with only required properties
            GenericResult1 result = AutomationClient.UpdateSite(Username, Password, siteId, site.Name, origSite.LogDirectory,
                origSite.LogFormat, origSite.LogWildcard, origSite.LogDaysBeforeDelete,
                origSite.SmarterLogDirectory, origSite.SmarterLogMonthsBeforeDelete, origSite.ExportPath, origSite.ExportPathURL,
                origSite.TimeZoneID);

            if (!result.Result)
                throw new Exception("Error updating statistics site: " + result.Message);

            // update site users
            // get original users
            if (site.Users != null)
            {
                List<string> origUsers = new List<string>();
                List<string> newUsers = new List<string>();
                string ownerUsername = null;
                UserInfoResultArray usrResult = AutomationClient.GetUsers(Username, Password, siteId);
                foreach (UserInfo user in usrResult.user)
                {
                    // add to original users
                    origUsers.Add(user.UserName.ToLower());

                    // remember owner (he can't be deleted)
                    if (user.IsSiteOwner)
                        ownerUsername = user.UserName.ToLower();
                }

                // add, update users
                foreach (StatsUser user in site.Users)
                {
                    if (!origUsers.Contains(user.Username.ToLower()))
                    {
                        // add user
                        GenericResult2 r = AutomationClient.AddUser(Username, Password, siteId,
                            user.Username, user.Password, user.FirstName, user.LastName, user.IsAdmin);
                        if (!r.Result)
                            throw new Exception("Error adding site user: " + r.Message);
                    }
                    else
                    {
                        // update user
                        GenericResult2 r = AutomationClient.UpdateUser(Username, Password, siteId,
                            user.Username, user.Password, user.FirstName, user.LastName, user.IsAdmin);
                        if (!r.Result)
                            throw new Exception("Error updating site user: " + r.Message);
                    }

                    // add to new users list
                    newUsers.Add(user.Username.ToLower());
                }

                // delete users
                foreach (string username in origUsers.Where(username => !newUsers.Contains(username) && username != ownerUsername))
                {
                        // delete user
                        AutomationClient.DeleteUser(Username, Password, siteId, username);
                }
            }
        }

        public virtual void DeleteSite(string siteId)
        {
            int sid = Int32.Parse(siteId);

            GenericResult1 result = AutomationClient.DeleteSite(Username, Password, sid, true);
            if (!result.Result)
                throw new Exception("Error deleting statistics site: " + result.Message);
        }

        #endregion

        #region IHostingServiceProvider methods
        public override void DeleteServiceItems(ServiceProviderItem[] items)
        {
            foreach (StatsSite item in items.OfType<StatsSite>())
            {
                    try
                    {
                        DeleteSite(item.SiteId);
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        Log.WriteError(String.Format("Error deleting '{0}' {1}", item.Name, item.GetType().Name), ex);
                    }
            }
        }
        #endregion

        #region Helper Methods
        protected virtual ISmarterStatsAutomationClient CreateAutomationClient()
        {
            if (String.Equals(AutomationTransport, "Soap", StringComparison.OrdinalIgnoreCase))
                return new SmarterStatsSoapAutomationClient(SmarterUrl);

            if (String.Equals(AutomationTransport, "Modern", StringComparison.OrdinalIgnoreCase))
                return new SmarterStatsModernAutomationClient(ModernAutomationEndpoint);

            throw new NotSupportedException(
                String.Format(
                    "Unsupported SmarterStats automation transport '{0}'. Supported values: Soap, Modern.",
                    AutomationTransport));
        }
        #endregion

        public override bool IsInstalled()
        {
            string productName = null;
            string productVersion = null;
            String[] names = null;

            RegistryKey HKLM = Registry.LocalMachine;

            RegistryKey key = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

            if (key != null)
            {
                names = key.GetSubKeyNames();

                foreach (RegistryKey subkey in names.Select(s => HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + s)))
                {

                    if (subkey != null)
                    {
                        if (!String.IsNullOrEmpty((string) subkey.GetValue("DisplayName")))
                        {
                            productName = (string) subkey.GetValue("DisplayName");
                        }
                        if (productName != null && (productName.Equals("SmarterStats") || productName.Equals("SmarterStats Service")))
                        {
                            productVersion = (string) subkey.GetValue("DisplayVersion");
                            break;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(productVersion))
                {
                    string[] split = productVersion.Split(new char[] {'.'});
                    return split[0].Equals("3");
                }
            }
            
                //checking x64 platform
                key = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

                if (key != null)
                {
                    names = key.GetSubKeyNames();

                    foreach (string s in names)
                    {
                        RegistryKey subkey =
                            HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + s);

                        if (subkey != null)
                        {
                            if (!String.IsNullOrEmpty((string) subkey.GetValue("DisplayName")))
                            {
                                productName = (string) subkey.GetValue("DisplayName");
                            }
                            if (productName != null && (productName.Equals("SmarterStats") || productName.Equals("SmarterStats Service")))
                            {
                                productVersion = (string) subkey.GetValue("DisplayVersion");
                                break;
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(productVersion))
                    {
                        string[] split = productVersion.Split(new char[] {'.'});
                        return split[0].Equals("3");
                    }
                }
            
            return false;
        }

    }
}


