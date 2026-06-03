// Copyright (C) 2026 FuseCP
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

using Microsoft.Win32;
using FuseCP.Providers.OS;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using FuseCP.Providers.FTP.IIs100;
using FuseCP.Providers.FTP.IIs100.Config;
using FuseCP.Providers.Utils;
using FuseCP.Providers.Utils.LogParser;
using FuseCP.Server.Utils;

namespace FuseCP.Providers.FTP
{
    [SupportedOSPlatform("windows")]
    public class MsFTP100 : HostingServiceProviderBase, IFtpServer
    {

        private static readonly string DefaultFtpSiteFolder = @"%SystemDrive%\inetpub\ftproot";
        private static readonly string DefaultFtpSiteLogsFolder = @"%SystemDrive%\inetpub\logs\LogFiles";
        private const int PowerShellTimeoutMs = 120000;
        private const int PowerShellDrainTimeoutMs = 5000;
        public const string DEFAULT_LOG_EXT_FILE_FIELDS = @"Date,Time,ClientIP,UserName,SiteName,ComputerName,
			ServerIP,Method,UriStem,FtpStatus,Win32Status,BytesSent,BytesRecv,TimeTaken,ServerPort,Host,FtpSubStatus,
			Session,FullPath,Info,ClientPort";

        private static readonly HashSet<string> Ftp100ServiceCommands = new(StringComparer.Ordinal) {
            "DataChannelClosed",
            "DataChannelOpened",
            "ControlChannelOpened"
        };

        public const string EMPTY_LOG_FIELD = "-";

        /// <summary>
        /// Initializes a new instance of the <see cref="MsFTP"/> class.
        /// </summary>
        public MsFTP100()
        {
            // Intentionally lazy to avoid loading IIS management assemblies unless needed.
        }

        #region Properties
        protected string SiteId
        {
            get { return ProviderSettings["SiteId"]; }
        }

        protected string SharedIP
        {
            get { return ProviderSettings["SharedIP"]; }
        }

        protected string FtpGroupName
        {
            get { return ProviderSettings["FtpGroupName"]; }
        }

        protected string UsersOU
        {
            get { return ProviderSettings["ADUsersOU"]; }
        }

        protected string GroupsOU
        {
            get { return ProviderSettings["ADGroupsOU"]; }
        }

        protected string AdFtpRoot
        {
            get { return ProviderSettings["AdFtpRoot"]; }
        }

        protected IIs100.Config.Mode UserIsolationMode
        {
            get
            {
                var site = GetSite(ProviderSettings["SiteId"]);
                return (Mode)Enum.Parse(typeof(Mode), site["UserIsolationMode"]);
            }
        }
        #endregion

        #region IFtpServer Members

        /// <summary>
        /// Changes site's state.
        /// </summary>
        /// <param name="siteId">Site's id to change state for.</param>
        /// <param name="state">State to be set.</param>
        /// <exception cref="ArgumentException">Is thrown in case site name is null or empty.</exception>
        public void ChangeSiteState(string siteId, ServerState state)
        {
            if (String.IsNullOrEmpty(siteId))
            {
                throw new ArgumentException("Site name is null or empty.");
            }

            switch (state)
            {
                case ServerState.Continuing:
                case ServerState.Started:
                    StartFtpSite(siteId);
                    break;
                case ServerState.Stopped:
                case ServerState.Paused:
                    StopFtpSite(siteId);
                    break;
            }
        }

        /// <summary>
        /// Gets state for ftp site with supplied id.
        /// </summary>
        /// <param name="siteId">Site's id to get state for.</param>
        /// <returns>Ftp site's state.</returns>
        /// <exception cref="ArgumentException">Is thrown in case site name is null or empty.</exception>
        public ServerState GetSiteState(string siteId)
        {
            if (String.IsNullOrEmpty(siteId))
            {
                throw new ArgumentException("Site name is null or empty.");
            }

            return GetFtpSiteState(siteId);
        }

        /// <summary>
        /// Checks whether site with given name exists.
        /// </summary>
        /// <param name="siteId">Site's name to check.</param>
        /// <returns>true - if it exists; false - otherwise.</returns>
        /// <exception cref="ArgumentException">Is thrown in case site name is null or empty.</exception>
        public bool SiteExists(string siteId)
        {
            if (String.IsNullOrEmpty(siteId))
            {
                throw new ArgumentException("Site name is null or empty.");
            }
            // In case site id doesn't contain default ftp site name we consider it as not existent.
            return SiteExistsByPowerShell(siteId);
        }

        /// <summary>
        /// Gets list of available ftp sites.
        /// </summary>
        /// <returns>List of available ftp sites.</returns>
        public FtpSite[] GetSites()
        {
            List<FtpSite> ftpSites = new List<FtpSite>();

            foreach (string ftpSiteName in GetFtpSitesNames())
            {
                ftpSites.Add(this.GetSite(ftpSiteName));
            }

            return ftpSites.ToArray();
        }

        /// <summary>
        /// Gets ftp site with given name.
        /// </summary>
        /// <param name="siteId">Ftp site's name to get.</param>
        /// <returns>Ftp site.</returns>
        /// <exception cref="ArgumentException"> Is thrown in case site name is null or empty. </exception>
        public FtpSite GetSite(string siteId)
        {
            if (String.IsNullOrEmpty(siteId))
            {
                throw new ArgumentException("Site name is null or empty.");
            }

            FtpSite ftpSite = new FtpSite();
            ftpSite.SiteId = siteId;
            ftpSite.Name = siteId;
            this.FillFtpSiteFromIis(ftpSite);

            return ftpSite;
        }

        /// <summary>
        /// Creates ftp site.
        /// </summary>
        /// <param name="site">Ftp site to be created.</param>
        /// <returns>Created site id.</returns>
        /// <exception cref="ArgumentNullException">Is thrown in case supplied argument is null.</exception>
        /// <exception cref="ArgumentException">
        /// Is thrown in case site id or its name is null or empty or if site id is not equal to default ftp site name.
        /// </exception>
        public string CreateSite(FtpSite site)
        {
            if (site == null)
            {
                throw new ArgumentNullException("site");
            }

            if (String.IsNullOrEmpty(site.SiteId) || String.IsNullOrEmpty(site.Name))
            {
                throw new ArgumentException("Site id or name is null or empty.");
            }

            this.CheckFtpServerBindings(site);
            string error;
            if (!EnsureFtpSiteConfiguredWithPowerShell(site, out error))
            {
                throw new InvalidOperationException(error);
            }

            // Do not start the site because it is started during creation.
            try
            {
                this.ChangeSiteState(site.Name, ServerState.Started);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                // Ignore the error if happened.
            }
            return site.Name;
        }

        /// <summary>
        /// Updates site with given information.
        /// </summary>
        /// <param name="site">Ftp site.</param>
        public void UpdateSite(FtpSite site)
        {
            // Check server bindings.
            CheckFtpServerBindings(site);

            string error;
            if (!EnsureFtpSiteConfiguredWithPowerShell(site, out error))
            {
                throw new InvalidOperationException(error);
            }
        }

        /// <summary>
        /// Deletes site with specified name.
        /// </summary>
        /// <param name="siteId">Site's name to be deleted.</param>
        public void DeleteSite(string siteId)
        {
            DeleteFtpSite(siteId);
        }

        /// <summary>
        /// Checks whether account with given name exists.
        /// </summary>
        /// <param name="accountName">Account name to check.</param>
        /// <returns>true - if it exists; false - otherwise.</returns>
        public bool AccountExists(string accountName)
        {
            if (String.IsNullOrEmpty(accountName))
            {
                return false;
            }

            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    return SecurityUtils.UserExists(accountName, ServerSettings, UsersOU);

                default:
                    // check acocunt on FTP server
                    bool ftpExists = AppVirtualDirectoryExistsByPowerShell(this.SiteId, accountName);

                    // check account in the system
                    bool systemExists = SecurityUtils.UserExists(accountName, ServerSettings, UsersOU);
                    return (ftpExists || systemExists);
            }
        }

        /// <summary>
		/// Gets available ftp accounts.
		/// </summary>
		/// <returns>List of avaialble accounts.</returns>
        public FtpAccount[] GetAccounts()
        {
            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    return SecurityUtils.GetUsers(ServerSettings, UsersOU).Select(GetAccount).ToArray();
                default:
                    List<FtpAccount> accounts = new List<FtpAccount>();

                    foreach (string directory in this.GetAppVirtualDirectoriesNamesByPowerShell(this.SiteId)
                        .Where(directory => !String.Equals(directory, "/")))
                    {
                        //
                        accounts.Add(this.GetAccount(directory.Substring(1)));
                    }

                    return accounts.ToArray();
            }
        }

        /// <summary>
        /// Gets account with given name.
        /// </summary>
        /// <param name="accountName">Account's name to get.</param>
        /// <returns>Ftp account.</returns>
        public FtpAccount GetAccount(string accountName)
        {
            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    var user = SecurityUtils.GetUser(accountName, ServerSettings, UsersOU);

                    var path = Path.Join(user.MsIIS_FTPRoot, user.MsIIS_FTPDir.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                    var permission = GetUserPermission(accountName, path);
                    var account = new FtpAccount()
                    {
                        CanRead = permission.Read,
                        CanWrite = permission.Write,
                        Enabled = !user.AccountDisabled,
                        Folder = path,
                        Name = accountName
                    };

                    return account;
                default:
                    FtpAccount acc = new FtpAccount();
                    acc.Name = accountName;
                    this.FillFtpAccountFromIis(acc);
                    return acc;
            }
        }

        protected UserPermission GetUserPermission(string accountName, string folder)
        {
            var userPermission = new UserPermission { AccountName = accountName };
            return SecurityUtils.GetGroupNtfsPermissions(folder, new[] { userPermission }, ServerSettings, UsersOU, GroupsOU)[0];
        }


        /// <summary>
		/// Creates ftp account under root ftp site.
		/// </summary>
		/// <param name="account">Ftp account to create.</param>
        public void CreateAccount(FtpAccount account)
        {
            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    SecurityUtils.EnsureOrganizationalUnitsExist(ServerSettings, UsersOU, GroupsOU);

                    var systemUser = SecurityUtils.GetUser(account.Name, ServerSettings, UsersOU);

                    if (systemUser == null)
                    {
                        systemUser = new SystemUser
                        {
                            Name = account.Name,
                            FullName = account.Name,
                            Password = account.Password,
                            PasswordCantChange = true,
                            PasswordNeverExpires = true,
                            System = true
                        };

                        SecurityUtils.CreateUser(systemUser, ServerSettings, UsersOU, GroupsOU);
                    }

                    UpdateAccount(account);

                    break;

                default:
                    // Create user account.
                    SystemUser user = new SystemUser();
                    user.Name = account.Name;
                    user.FullName = account.Name;
                    user.Description = "FuseCP System Account";
                    user.MemberOf = new string[] { FtpGroupName };
                    user.Password = account.Password;
                    user.PasswordCantChange = true;
                    user.PasswordNeverExpires = true;
                    user.AccountDisabled = !account.Enabled;
                    user.System = true;

                    // Create in the operating system.
                    if (SecurityUtils.UserExists(user.Name, ServerSettings, UsersOU))
                    {
                        SecurityUtils.DeleteUser(user.Name, ServerSettings, UsersOU);
                    }
                    SecurityUtils.CreateUser(user, ServerSettings, UsersOU, GroupsOU);

                    // Prepare account's home folder.
                    this.EnsureUserHomeFolderExists(account.Folder, account.Name, account.CanRead, account.CanWrite);

                    // Future account will be given virtual directory under default ftp web site.
                    EnsureFtpAccountVirtualDirectory(this.SiteId, account.VirtualPath, account.Folder);
                    SetFtpAuthorization(this.SiteId, account.Name, account.CanRead, account.CanWrite);
                    break;
            }
        }

        /// <summary>
        /// Updates ftp account.
        /// </summary>
        /// <param name="account">Accoun to update.</param>
        public void UpdateAccount(FtpAccount account)
        {
            var user = SecurityUtils.GetUser(account.Name, ServerSettings, UsersOU);

            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    var ftpRoot = AdFtpRoot.ToLower();
                    var ftpDir = account.Folder.ToLower().Replace(ftpRoot, "");

                    var oldDir = user.MsIIS_FTPDir;

                    user.Password = account.Password;
                    user.PasswordCantChange = true;
                    user.PasswordNeverExpires = true;
                    user.Description = "FuseCP FTP Account with AD User Isolation";
                    user.MemberOf = new[] { FtpGroupName };
                    user.AccountDisabled = !account.Enabled;
                    user.MsIIS_FTPRoot = ftpRoot;
                    user.MsIIS_FTPDir = ftpDir;
                    user.System = true;

                    SecurityUtils.UpdateUser(user, ServerSettings, UsersOU, GroupsOU);

                    // Set NTFS permissions
                    var userPermission = GetUserPermission(account.Name, account.Folder);

                    // Do we need to change the NTFS permissions? i.e. is users home dir changed or are permissions changed?
                    if (oldDir != ftpDir || account.CanRead != userPermission.Read || account.CanWrite != userPermission.Write)
                    {
                        // First get sid of user account
                        var sid = SecurityUtils.GetAccountSid(account.Name, ServerSettings, UsersOU, GroupsOU);

                        // Remove the permissions set for this account on previous folder
                        SecurityUtils.RemoveNtfsPermissionsBySid(Path.Join(ftpRoot, oldDir.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), sid);

                        // If no permissions is to be set, exit
                        if (!account.CanRead && !account.CanWrite)
                        {
                            return;
                        }

                        // Add the new permissions
                        var ntfsPermissions = account.CanRead ? NTFSPermission.Read : NTFSPermission.Write;
                        if (account.CanRead && account.CanWrite)
                        {
                            ntfsPermissions = NTFSPermission.Modify;
                        }

                        SecurityUtils.GrantNtfsPermissionsBySid(account.Folder, sid, ntfsPermissions, true, true);
                    }
                    break;

                default:

                    // Change user account state and password (if required).
                    user.Password = account.Password;
                    user.AccountDisabled = !account.Enabled;
                    SecurityUtils.UpdateUser(user, ServerSettings, UsersOU, GroupsOU);
                    // Update iis configuration.
                    this.FillIisFromFtpAccount(account);
                    break;
            }
        }

        /// <summary>
        /// Deletes account with given name.
        /// </summary>
        /// <param name="accountName">Account's name to be deleted.</param>
        public void DeleteAccount(string accountName)
        {
            switch (UserIsolationMode)
            {
                case Mode.ActiveDirectory:
                    var account = GetAccount(accountName);

                    // Remove the NTFS permissions first
                    SecurityUtils.RemoveNtfsPermissions(account.Folder, account.Name, ServerSettings, UsersOU, GroupsOU);

                    if (SecurityUtils.UserExists(accountName, ServerSettings, UsersOU))
                    {
                        SecurityUtils.DeleteUser(accountName, ServerSettings, UsersOU);
                    }
                    break;

                default:
                    string appVirtualDirectory = String.Format("/{0}", accountName);
                    string currentPhysicalPath = GetSitePhysicalPathByPowerShell(this.SiteId, appVirtualDirectory);

                    // Delete virtual directory
                    DeleteFtpAccountVirtualDirectory(this.SiteId, appVirtualDirectory);

                    // Remove permissions
                    RemoveFtpFolderPermissions(currentPhysicalPath, accountName);

                    // Delete system user account
                    if (SecurityUtils.UserExists(accountName, ServerSettings, UsersOU))
                    {
                        SecurityUtils.DeleteUser(accountName, ServerSettings, UsersOU);
                    }
                    break;
            }
        }

        /// <summary>
		/// Fills iis configuration  from ftp account.
		/// </summary>
		/// <param name="ftpAccount">Ftp account to fill from.</param>
		private void FillIisFromFtpAccount(FtpAccount ftpAccount)
        {
            // Remove permissions if required.
            string currentPhysicalPath = GetSitePhysicalPathByPowerShell(this.SiteId, String.Format("/{0}", ftpAccount.Name));
            if (String.Compare(currentPhysicalPath, ftpAccount.Folder, true) != 0)
            {
                RemoveFtpFolderPermissions(currentPhysicalPath, ftpAccount.Name);
            }

            // Set new permissions
            EnsureUserHomeFolderExists(ftpAccount.Folder, ftpAccount.Name, ftpAccount.CanRead, ftpAccount.CanWrite);
            // Update physical path.
            EnsureFtpAccountVirtualDirectory(this.SiteId, ftpAccount.VirtualPath, ftpAccount.Folder);
            SetFtpAuthorization(this.SiteId, ftpAccount.Name, ftpAccount.CanRead, ftpAccount.CanWrite);
        }

        /// <summary>
        /// Fills ftp account from iis configuration.
        /// </summary>
        /// <param name="ftpAccount">Ftp account to fill.</param>
        private void FillFtpAccountFromIis(FtpAccount ftpAccount)
        {
            //
            ftpAccount.Folder = GetSitePhysicalPathByPowerShell(this.SiteId, String.Format("/{0}", ftpAccount.Name));

            ftpAccount.CanRead = false;
            ftpAccount.CanWrite = false;
            if (!String.IsNullOrEmpty(ftpAccount.Folder) && FileUtils.DirectoryExists(ftpAccount.Folder))
            {
                var permission = GetUserPermission(ftpAccount.Name, ftpAccount.Folder);
                ftpAccount.CanRead = permission.Read;
                ftpAccount.CanWrite = permission.Write;
            }

            // Load user account.
            SystemUser user = SecurityUtils.GetUser(ftpAccount.Name, ServerSettings, UsersOU);
            if (user != null)
            {
                ftpAccount.Enabled = !user.AccountDisabled;
            }
        }

        /// <summary>
        /// Fills ftp site with data from iis ftp site.
        /// </summary>
        /// <param name="ftpSite">Ftp site to fill.</param>
        private void FillFtpSiteFromIis(FtpSite ftpSite)
        {
            ftpSite.AllowAnonymous = GetFtpSiteAnonymousEnabled(ftpSite.SiteId);
            ftpSite.AnonymousUsername = String.Empty;
            ftpSite.AnonymousUserPassword = String.Empty;
            ftpSite["UserIsolationMode"] = GetFtpSiteUserIsolationMode(ftpSite.SiteId);
            ftpSite[FtpSite.MSFTP7_SITE_ID] = ftpSite.SiteId;
            ftpSite.LogFileDirectory = GetFtpSiteLogDirectory(ftpSite.SiteId);
            ftpSite[FtpSite.MSFTP7_LOG_EXT_FILE_FIELDS] = DEFAULT_LOG_EXT_FILE_FIELDS;
            ftpSite.Bindings = GetSiteBindingsByPowerShell(ftpSite.SiteId);
            ftpSite.ContentPath = GetSitePhysicalPathByPowerShell(ftpSite.SiteId, "/");
        }

        /// <summary>
        /// Fills iis configuration with information from ftp site.
        /// </summary>
        /// <param name="ftpSite">Ftp site that holds information.</param>
        private void FillIisFromFtpSite(FtpSite ftpSite)
        {
            string error;
            if (!EnsureFtpSiteConfiguredWithPowerShell(ftpSite, out error))
            {
                throw new InvalidOperationException(error);
            }
        }

        /// <summary>
        /// Ensures that home folder for ftp account exists.
        /// </summary>
        /// <param name="folder">Path to home folder.</param>
        /// <param name="accountName">Account name.</param>
        /// <param name="allowRead">A value which specifies whether read operation is allowed or not.</param>
        /// <param name="allowWrite">A value which specifies whether write operation is allowed or not.</param>
        private void EnsureUserHomeFolderExists(string folder, string accountName, bool allowRead, bool allowWrite)
        {
            // create folder
            if (!FileUtils.DirectoryExists(folder))
            {
                FileUtils.CreateDirectory(folder);
            }

            if (!allowRead && !allowWrite)
            {
                return;
            }

            NTFSPermission permissions = allowRead ? NTFSPermission.Read : NTFSPermission.Write;

            if (allowRead && allowWrite)
            {
                permissions = NTFSPermission.Modify;
            }

            // Set ntfs permissions
            SecurityUtils.GrantNtfsPermissions(folder, accountName, permissions, true, true,
                ServerSettings, UsersOU, GroupsOU);
        }

        /// <summary>
        /// Removes user specific permissions from folder.
        /// </summary>
        /// <param name="path">Folder to operate on.</param>
        /// <param name="accountName">User's name.</param>
        private void RemoveFtpFolderPermissions(string path, string accountName)
        {
            if (!FileUtils.DirectoryExists(path))
            {
                return;
            }

            // Anonymous account
            SecurityUtils.RemoveNtfsPermissions(path, accountName, ServerSettings, UsersOU, GroupsOU);
        }

        /// <summary>
        /// Checks if bindings listed in given site already in use.
        /// </summary>
        /// <param name="site">Site to check.</param>
        /// <exception cref="InvalidOperationException">Is thrown in case supplied site contains bindings that are already in use.</exception>
        private void CheckFtpServerBindings(FtpSite site)
        {
            if (this.IsFtpServerBindingsInUse(site))
            {
                throw new InvalidOperationException("Some of ftp site's bindings are already in use.");
            }
        }

        /// <summary>
        /// Gets a value which shows whether supplied site contains bindings that are already in use.
        /// </summary>
        /// <param name="site">Site to check.</param>
        /// <returns>true - if any of supplied bindinds is in use; false -otherwise.</returns>
        private bool IsFtpServerBindingsInUse(FtpSite site)
        {
            if (site == null)
            {
                throw new ArgumentNullException("site");
            }

            // check for server bindings
            return this.GetSites()
                .Where(existentSite => existentSite.Name != site.Name)
                .SelectMany(existentSite => existentSite.Bindings)
                .Any(usedBinding => site.Bindings.Any(requestedBinding => usedBinding.IP == requestedBinding.IP && usedBinding.Port == requestedBinding.Port));
        }

        /// <summary>
        /// Gets fully qualified name with respect to enabled active directory.
        /// </summary>
        /// <param name="accountName">Account name.</param>
        /// <returns>Fully qualified acount/domain name.</returns>
        private string GetQualifiedAccountName(string accountName)
        {
            if (!ServerSettings.ADEnabled)
            {
                return accountName;
            }

            if (accountName.IndexOf("\\") != -1)
            {
                return accountName; // already has domain information
            }

            // DO IT FOR ACTIVE DIRECTORY MODE ONLY
            string domainName = null;
            try
            {
                DirectoryContext objContext = new DirectoryContext(DirectoryContextType.Domain, ServerSettings.ADRootDomain);
                Domain objDomain = Domain.GetDomain(objContext);
                domainName = objDomain.Name;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                Log.WriteError("Get domain name error", ex);
            }

            return domainName != null ? domainName + "\\" + accountName : accountName;
        }
        #endregion

        #region IHostingServiceProvier methods
        /// <summary>
        /// Installs Ftp7 provider.
        /// </summary>
        /// <returns>Error messages.</returns>
        public override string[] Install()
        {
            List<string> messages = new List<string>();

            FtpSite site = null;
            string folder = FileUtils.EvaluateSystemVariables(DefaultFtpSiteFolder);
            string logsDirectory = FileUtils.EvaluateSystemVariables(DefaultFtpSiteLogsFolder);
            Log.WriteInfo("MSFTP100 Install: preparing folders. SiteId='{0}', SiteFolder='{1}', LogsFolder='{2}'", SiteId, folder, logsDirectory);
            // Create site folder.
            if (!FileUtils.DirectoryExists(folder))
            {
                FileUtils.CreateDirectory(folder);
            }
            // Create logs folder.
            if (!FileUtils.DirectoryExists(logsDirectory))
            {
                FileUtils.CreateDirectory(logsDirectory);
            }

            site = new FtpSite();

            site.Name = this.SiteId;
            site.SiteId = this.SiteId;
            site.ContentPath = DefaultFtpSiteFolder;
            site.Bindings = new ServerBinding[1];
            // set default log directory
            site.LogFileDirectory = DefaultFtpSiteLogsFolder;
            // set default logging fields
            site[FtpSite.MSFTP7_LOG_EXT_FILE_FIELDS] = DEFAULT_LOG_EXT_FILE_FIELDS;

            site.Bindings[0] = !String.IsNullOrEmpty(this.SharedIP)
                ? new ServerBinding(this.SharedIP, "21", String.Empty)
                : new ServerBinding("*", "21", "*");
                //// Get information on local server.
                //IPHostEntry localServerHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                //foreach (IPAddress address in localServerHostEntry.AddressList)
                //{
                //    if (address.AddressFamily == AddressFamily.InterNetwork)
                //    {
                //        site.Bindings[0] = new ServerBinding(address.ToString(), "21", String.Empty);
                //    }
                //}


            bool siteExists = SiteExistsByPowerShell(site.SiteId);
            if (!siteExists && !IsFtpPortAvailable(site.Bindings[0]?.Port ?? "21"))
            {
                messages.Add("Cannot create ftp site because requested bindings are already in use.");
                return messages.ToArray();
            }
            if (siteExists)
            {
                Log.WriteInfo("MSFTP100 Install: site '{0}' already exists, applying hardening/update flow.", site.SiteId);
            }

            try
            {
                Log.WriteInfo("MSFTP100 Install: ensuring AD organizational units. UsersOU='{0}', GroupsOU='{1}'", UsersOU, GroupsOU);
                SecurityUtils.EnsureOrganizationalUnitsExist(ServerSettings, UsersOU, GroupsOU);
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                messages.Add(String.Format("Could not check/create Organizational Units: {0}", ex.Message));
                return messages.ToArray();
            }

            // create folder if it not exists
            if (String.IsNullOrEmpty(SiteId))
            {
                messages.Add("Please, select FTP site to create accounts on");
            }
            else
            {
                // create FTP group name
                if (String.IsNullOrEmpty(FtpGroupName))
                {
                    messages.Add("FTP Group can not be blank");
                }
                else
                {
                    try
                    {
                        // create group
                        Log.WriteInfo("MSFTP100 Install: ensuring FTP group exists. Group='{0}'", FtpGroupName);
                        if (!SecurityUtils.GroupExists(FtpGroupName, ServerSettings, GroupsOU))
                        {
                            SystemGroup group = new SystemGroup();
                            group.Name = FtpGroupName;
                            group.Members = new string[] { };
                            group.Description = "FuseCP System Group";

                            SecurityUtils.CreateGroup(group, ServerSettings, UsersOU, GroupsOU);
                        }
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        messages.Add(String.Format("There was an error while adding '{0}' group: {1}",
                            FtpGroupName, ex.Message));
                        return messages.ToArray();
                    }
                }

                Log.WriteInfo("MSFTP100 Install: configuring IIS FTP site via PowerShell. SiteId='{0}'", site.SiteId);
                if (!EnsureFtpSiteConfigured(site, messages))
                {
                    return messages.ToArray();
                }

                Log.WriteInfo("MSFTP100 Install: skipping broad root NTFS grant to preserve per-user isolation. SitePath='{0}'", site.ContentPath);
            }
            return messages.ToArray();
        }

        public override void ChangeServiceItemsState(ServiceProviderItem[] items, bool enabled)
        {
            foreach (FtpAccount item in items.OfType<FtpAccount>())
            {
                try
                {
                    // make FTP account read-only
                    FtpAccount account = GetAccount(item.Name);
                    account.Enabled = enabled;
                    UpdateAccount(account);
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    Log.WriteError(String.Format("Error switching '{0}' {1}", item.Name, item.GetType().Name), ex);
                }
            }
        }

        public override void DeleteServiceItems(ServiceProviderItem[] items)
        {
            foreach (FtpAccount item in items.OfType<FtpAccount>())
            {
                try
                {
                    // delete FTP account from default FTP site
                    DeleteAccount(item.Name);
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    Log.WriteError(String.Format("Error deleting '{0}' {1}", item.Name, item.GetType().Name), ex);
                }
            }
        }

        public override ServiceProviderItemBandwidth[] GetServiceItemsBandwidth(ServiceProviderItem[] items, DateTime since)
        {
            ServiceProviderItemBandwidth[] itemsBandwidth = new ServiceProviderItemBandwidth[items.Length];

            // calculate bandwidth for Default FTP Site
            FtpSite ftpSite = GetSite(SiteId);
            string siteId = String.Concat("FTPSVC", ftpSite[FtpSite.MSFTP7_SITE_ID]);
            string logsPath = Path.Join(ftpSite.LogFileDirectory, siteId.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

            // create parser object
            // and update statistics
            LogParser parser = new LogParser("Ftp", siteId, logsPath, "s-sitename", "cs-username");
            // Subscribe to the events because FTP 7.0 has several differences that should be taken into account
            // and processed in a specific way
            parser.ProcessKeyFields += new ProcessKeyFieldsEventHandler(LogParser_ProcessKeyFields);
            parser.CalculateStatisticsLine += new CalculateStatsLineEventHandler(LogParser_CalculateStatisticsLine);
            // 
            parser.ParseLogs();

            // update items with diskspace
            for (int i = 0; i < items.Length; i++)
            {
                ServiceProviderItem item = items[i];

                // create new bandwidth object
                itemsBandwidth[i] = new ServiceProviderItemBandwidth();
                itemsBandwidth[i].ItemId = item.Id;
                itemsBandwidth[i].Days = new DailyStatistics[0];

                if (item is FtpAccount)
                {
                    try
                    {
                        // get daily statistics
                        itemsBandwidth[i].Days = parser.GetDailyStatistics(since, new string[] { siteId, item.Name });
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        Log.WriteError(ex);
                    }
                }
            }
            return itemsBandwidth;
        }

        #endregion

        #region LogParser event handlers and helper routines

        private bool IsFtpServiceCommand(string command)
        {
            return Ftp100ServiceCommands.Contains(command);
        }

        private void LogParser_ProcessKeyFields(string[] key_fields, string[] key_values, string[] log_fields,
            string[] log_values)
        {
            int cs_method = Array.IndexOf(log_fields, "cs-method");
            int cs_uri_stem = Array.IndexOf(log_fields, "cs-uri-stem");
            int cs_username = Array.IndexOf(key_fields, "cs-username");
            //
            if (cs_username > -1)
            {
                string valueStr = EMPTY_LOG_FIELD;
                // this trick allows to calculate USER command bytes as well
                // in spite that "cs-username" field is empty for the command
                if (key_values[cs_username] != EMPTY_LOG_FIELD)
                    valueStr = key_values[cs_username];
                else if (cs_method > -1 && cs_uri_stem > -1 && log_values[cs_method] == "USER")
                    valueStr = log_values[cs_uri_stem];
                //
                key_values[cs_username] = valueStr.Substring(valueStr.IndexOf(@"\") + 1);
            }
        }

        private void LogParser_CalculateStatisticsLine(StatsLine line, string[] fields, string[] values)
        {
            int cs_method = Array.IndexOf(fields, "cs-method");
            // bandwidth calculation ignores FTP 7.0 serviced commands
            if (cs_method > -1 && !IsFtpServiceCommand(values[cs_method]))
            {
                int cs_bytes = Array.IndexOf(fields, "cs-bytes");
                int sc_bytes = Array.IndexOf(fields, "sc-bytes");
                // skip empty cs-bytes value processing
                if (cs_bytes > -1 && values[cs_bytes] != "0")
                    line.BytesReceived += Int64.Parse(values[cs_bytes]);
                // skip empty sc-bytes value processing
                if (sc_bytes > -1 && values[sc_bytes] != "0")
                    line.BytesSent += Int64.Parse(values[sc_bytes]);
            }
        }

        #endregion

        private static bool IsMsFtpServiceInstalled()
        {
            if (!OperatingSystem.IsWindows())
            {
                return false;
            }

            using RegistryKey inetStp = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\InetStp", writable: false);
            int majorVersion = inetStp?.GetValue("MajorVersion") as int? ?? 0;

            using RegistryKey ftpService = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\ftpsvc", writable: false);
            return majorVersion == 10 && ftpService != null;
        }

        private bool EnsureFtpSiteConfigured(FtpSite site, List<string> messages)
        {
            string error;
            if (!EnsureFtpSiteConfiguredWithPowerShell(site, out error))
            {
                Log.WriteWarning("MSFTP100 Install: IIS FTP site configuration failed. {0}", error);
                messages.Add(error);
                return false;
            }

            return true;
        }

        private static bool IsFtpPortAvailable(string port)
        {
            string script = "$port = [int]'" + EscapePowerShellLiteral(port) + "';"
                + "$listeners = Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue | Where-Object { $_.LocalPort -eq $port };"
                + "if ($listeners) { exit 1 } else { exit 0 }";

            string stdOut;
            string stdErr;
            int exitCode;
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr))
            {
                // If we cannot determine port state, do not block installation here.
                return true;
            }

            return exitCode == 0;
        }

        private bool EnsureFtpSiteConfiguredWithPowerShell(FtpSite site, out string error)
        {
            error = null;

            if (String.IsNullOrWhiteSpace(FtpGroupName))
            {
                error = "Could not configure Microsoft FTP 10.0 site because FTP Group is empty.";
                return false;
            }

            string siteName = EscapePowerShellLiteral(site.SiteId);
            string physicalPath = EscapePowerShellLiteral(FileUtils.EvaluateSystemVariables(site.ContentPath));
            string ip = EscapePowerShellLiteral(site.Bindings?[0]?.IP ?? "*");
            string port = EscapePowerShellLiteral(site.Bindings?[0]?.Port ?? "21");
            string hostHeader = EscapePowerShellLiteral(site.Bindings?[0]?.Host ?? String.Empty);
            string logDir = EscapePowerShellLiteral(FileUtils.EvaluateSystemVariables(site.LogFileDirectory));
            string ftpGroup = EscapePowerShellLiteral(FtpGroupName);

            string script = $@"
Import-Module WebAdministration -ErrorAction Stop
$siteName = '{siteName}'
$physicalPath = '{physicalPath}'
$ip = '{ip}'
$port = '{port}'
$hostHeader = '{hostHeader}'
$logDir = '{logDir}'
$ftpGroup = '{ftpGroup}'

if ($hostHeader -eq '*') {{
    $hostHeader = ''
}}

if (-not (Test-Path -LiteralPath $physicalPath)) {{
    New-Item -Path $physicalPath -ItemType Directory -Force | Out-Null
}}

if (-not (Test-Path -LiteralPath $logDir)) {{
    New-Item -Path $logDir -ItemType Directory -Force | Out-Null
}}

if (-not (Test-Path ('IIS:\Sites\' + $siteName))) {{
    New-WebFtpSite -Name $siteName -Port ([int]$port) -IPAddress $ip -PhysicalPath $physicalPath -Force | Out-Null
}} else {{
    Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name physicalPath -Value $physicalPath
}}

$desiredBinding = ($ip + ':' + $port + ':' + $hostHeader)
$ftpBindings = @(Get-WebBinding -Name $siteName -Protocol ftp -ErrorAction SilentlyContinue)
$hasDesiredBinding = $false
foreach ($binding in $ftpBindings) {{
    if ($binding.bindingInformation -eq $desiredBinding) {{
        $hasDesiredBinding = $true
    }} else {{
        Remove-WebBinding -Name $siteName -Protocol ftp -BindingInformation $binding.bindingInformation -ErrorAction SilentlyContinue
    }}
}}

if (-not $hasDesiredBinding) {{
    New-WebBinding -Name $siteName -Protocol ftp -IPAddress $ip -Port ([int]$port) -HostHeader $hostHeader | Out-Null
}}

Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name ftpServer.security.authentication.anonymousAuthentication.enabled -Value $false
Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name ftpServer.security.authentication.basicAuthentication.enabled -Value $true
Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name ftpServer.userIsolation.mode -Value 'StartInUsersDirectory'
Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name ftpServer.security.ssl.controlChannelPolicy -Value 'SslAllow'
Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name ftpServer.security.ssl.dataChannelPolicy -Value 'SslAllow'
Set-ItemProperty -Path ('IIS:\Sites\' + $siteName) -Name logFile.directory -Value $logDir

Clear-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Location $siteName -Filter 'system.ftpServer/security/authorization'
Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Location $siteName -Filter 'system.ftpServer/security/authorization' -Value @{{ accessType = 'Allow'; users = ''; roles = $ftpGroup; permissions = 'Read' }}
";

            string stdOut;
            string stdErr;
            int exitCode;
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr))
            {
                error = "Could not configure Microsoft FTP 10.0 site because PowerShell could not be started.";
                return false;
            }

            if (exitCode != 0)
            {
                error = String.Format("Could not configure Microsoft FTP 10.0 site using PowerShell (exit code {0}). {1}", exitCode,
                    !String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim());
                return false;
            }

            return true;
        }

        private static bool RunPowerShell(string script, out int exitCode, out string stdOut, out string stdErr)
        {
            exitCode = -1;
            stdOut = String.Empty;
            stdErr = String.Empty;

            string executable = ResolvePowerShellExecutable();
            if (String.IsNullOrEmpty(executable))
            {
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + Convert.ToBase64String(Encoding.Unicode.GetBytes(script)),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    return false;
                }

                Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
                Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

                bool exited = process.WaitForExit(PowerShellTimeoutMs);
                if (!exited)
                {
                    try
                    {
                        process.Kill(true);
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        // Best effort kill; continue and report timeout.
                    }

                    process.WaitForExit(PowerShellDrainTimeoutMs);
                    exitCode = -2;
                }
                else
                {
                    exitCode = process.ExitCode;
                }

                Task.WaitAll(new Task[] { stdOutTask, stdErrTask }, PowerShellDrainTimeoutMs);
                stdOut = stdOutTask.IsCompleted ? stdOutTask.Result : String.Empty;
                stdErr = stdErrTask.IsCompleted ? stdErrTask.Result : String.Empty;

                if (!exited)
                {
                    string timeoutDetails = "PowerShell operation timed out after " + (PowerShellTimeoutMs / 1000) + " seconds.";
                    stdErr = String.IsNullOrEmpty(stdErr) ? timeoutDetails : (stdErr.TrimEnd() + Environment.NewLine + timeoutDetails);
                }

                return true;
            }
        }

        private static string ResolvePowerShellExecutable()
        {
            // WebAdministration + IIS provider commands are most reliable in Windows PowerShell.
            string[] candidates = new[] { "powershell.exe", "pwsh.exe" };
            foreach (string candidate in candidates)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = candidate,
                        Arguments = "-NoProfile -NonInteractive -Command \"Import-Module WebAdministration -ErrorAction Stop; if (Test-Path 'IIS:\\Sites') { exit 0 } else { exit 1 }\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (var process = Process.Start(startInfo))
                    {
                        if (process == null)
                        {
                            continue;
                        }

                        process.WaitForExit(5000);
                        if (process.ExitCode == 0)
                        {
                            return candidate;
                        }
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    // Try next candidate.
                }
            }

            return null;
        }

        private static string EscapePowerShellLiteral(string value)
        {
            if (value == null)
            {
                return String.Empty;
            }

            return value.Replace("'", "''");
        }

        protected virtual bool IsMsFTPInstalled()
        {
            return IsMsFtpServiceInstalled();
        }

        private void StartFtpSite(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; Start-WebSite -Name '" + EscapePowerShellLiteral(siteId) + "'";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                throw new InvalidOperationException("Could not start FTP site '" + siteId + "'. " + (!String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim()));
        }

        private void StopFtpSite(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; Stop-WebSite -Name '" + EscapePowerShellLiteral(siteId) + "'";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                throw new InvalidOperationException("Could not stop FTP site '" + siteId + "'. " + (!String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim()));
        }

        private void DeleteFtpSite(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; if (Test-Path ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "')) { Remove-Website -Name '" + EscapePowerShellLiteral(siteId) + "' }";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                throw new InvalidOperationException("Could not delete FTP site '" + siteId + "'. " + (!String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim()));
        }

        private bool SiteExistsByPowerShell(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; if (Test-Path ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "')) { exit 0 } else { exit 1 }";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr))
                return false;
            return exitCode == 0;
        }

        private ServerState GetFtpSiteState(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; $s = Get-Item ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "') -ErrorAction Stop; $s.State.ToString()";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return ServerState.Unknown;

            string state = (stdOut ?? String.Empty).Trim();
            switch (state)
            {
                case "Started": return ServerState.Started;
                case "Stopped": return ServerState.Stopped;
                case "Starting": return ServerState.Starting;
                case "Stopping": return ServerState.Stopping;
                default: return ServerState.Unknown;
            }
        }

        private string[] GetFtpSitesNames()
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; Get-ChildItem IIS:\\Sites | Where-Object { $_.Bindings.Collection | Where-Object { $_.protocol -eq 'ftp' } } | Select-Object -ExpandProperty Name";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return Array.Empty<string>();

            return (stdOut ?? String.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !String.IsNullOrEmpty(s))
                .ToArray();
        }

        private ServerBinding[] GetSiteBindingsByPowerShell(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; Get-WebBinding -Name '" + EscapePowerShellLiteral(siteId) + "' -Protocol ftp | Select-Object -ExpandProperty bindingInformation";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return Array.Empty<ServerBinding>();

            return (stdOut ?? String.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseBindingInformation)
                .Where(b => b != null)
                .ToArray();
        }

        private static ServerBinding ParseBindingInformation(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            string[] parts = value.Split(':');
            if (parts.Length < 2)
                return null;

            string ip = parts[0];
            string port = parts[1];
            string host = parts.Length > 2 ? parts[2] : String.Empty;
            return new ServerBinding("ftp", ip, port, host);
        }

        private string GetSitePhysicalPathByPowerShell(string siteId, string virtualPath)
        {
            string normalizedVirtualPath = String.IsNullOrEmpty(virtualPath) ? "/" : virtualPath;
            string stdOut;
            string stdErr;
            int exitCode;

            string script;
            if (String.Equals(normalizedVirtualPath, "/", StringComparison.Ordinal))
            {
                script = "Import-Module WebAdministration -ErrorAction Stop; (Get-Item ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "')).physicalPath";
            }
            else
            {
                string name = normalizedVirtualPath.Trim('/');
                script = "Import-Module WebAdministration -ErrorAction Stop; $v = Get-WebVirtualDirectory -Site '" + EscapePowerShellLiteral(siteId) + "' -Name '" + EscapePowerShellLiteral(name) + "' -ErrorAction SilentlyContinue; if ($v) { $v.PhysicalPath }";
            }

            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return String.Empty;

            return (stdOut ?? String.Empty).Trim();
        }

        private bool AppVirtualDirectoryExistsByPowerShell(string siteId, string accountName)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; $v = Get-WebVirtualDirectory -Site '" + EscapePowerShellLiteral(siteId) + "' -Name '" + EscapePowerShellLiteral(accountName) + "' -ErrorAction SilentlyContinue; if ($v) { exit 0 } else { exit 1 }";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr))
                return false;
            return exitCode == 0;
        }

        private IEnumerable<string> GetAppVirtualDirectoriesNamesByPowerShell(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; Get-WebVirtualDirectory -Site '" + EscapePowerShellLiteral(siteId) + "' | Select-Object -ExpandProperty Path";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return Array.Empty<string>();

            return (stdOut ?? String.Empty)
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !String.IsNullOrEmpty(s));
        }

        private void EnsureFtpAccountVirtualDirectory(string siteId, string virtualPath, string physicalPath)
        {
            string name = (virtualPath ?? String.Empty).Trim('/');
            string script = "Import-Module WebAdministration -ErrorAction Stop; "
                + "$site='" + EscapePowerShellLiteral(siteId) + "';"
                + "$name='" + EscapePowerShellLiteral(name) + "';"
                + "$path='" + EscapePowerShellLiteral(physicalPath) + "';"
                + "if (-not (Test-Path -LiteralPath $path)) { New-Item -Path $path -ItemType Directory -Force | Out-Null };"
                + "$existing = Get-WebVirtualDirectory -Site $site -Name $name -ErrorAction SilentlyContinue;"
                + "if ($existing) { Set-ItemProperty -Path ('IIS:\\Sites\\' + $site + '\\' + $name) -Name physicalPath -Value $path } else { New-WebVirtualDirectory -Site $site -Name $name -PhysicalPath $path | Out-Null }";

            string stdOut;
            string stdErr;
            int exitCode;
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                throw new InvalidOperationException("Could not create/update FTP account virtual directory. " + (!String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim()));
        }

        private void DeleteFtpAccountVirtualDirectory(string siteId, string virtualPath)
        {
            string name = (virtualPath ?? String.Empty).Trim('/');
            string script = "Import-Module WebAdministration -ErrorAction Stop; $target = 'IIS:\\Sites\\" + EscapePowerShellLiteral(siteId) + "\\" + EscapePowerShellLiteral(name) + "'; if (Test-Path $target) { Remove-Item -Path $target -Recurse -Force }";
            string stdOut;
            string stdErr;
            int exitCode;
            RunPowerShell(script, out exitCode, out stdOut, out stdErr);
        }

        private void SetFtpAuthorization(string siteId, string accountName, bool canRead, bool canWrite)
        {
            string permissions = canRead && canWrite ? "Read, Write" : (canWrite ? "Write" : "Read");
            string location = EscapePowerShellLiteral(siteId + "/" + accountName);
            string script = "Import-Module WebAdministration -ErrorAction Stop; "
                + "Clear-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Location '" + location + "' -Filter 'system.ftpServer/security/authorization';"
                + "Add-WebConfiguration -PSPath 'MACHINE/WEBROOT/APPHOST' -Location '" + location + "' -Filter 'system.ftpServer/security/authorization' -Value @{ accessType='Allow'; users='" + EscapePowerShellLiteral(accountName) + "'; roles=''; permissions='" + permissions + "' }";

            string stdOut;
            string stdErr;
            int exitCode;
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                throw new InvalidOperationException("Could not set FTP authorization. " + (!String.IsNullOrEmpty(stdErr) ? stdErr.Trim() : stdOut.Trim()));
        }

        private bool GetFtpSiteAnonymousEnabled(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; $site = Get-Item ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "') -ErrorAction Stop; $site.ftpServer.security.authentication.anonymousAuthentication.enabled.ToString()";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return false;
            return String.Equals((stdOut ?? String.Empty).Trim(), "True", StringComparison.OrdinalIgnoreCase);
        }

        private string GetFtpSiteUserIsolationMode(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; $site = Get-Item ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "') -ErrorAction Stop; $site.ftpServer.userIsolation.mode.ToString()";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return Mode.StartInUsersDirectory.ToString();

            string value = (stdOut ?? String.Empty).Trim();
            if (String.IsNullOrEmpty(value))
                return Mode.None.ToString();

            if (value.IndexOf("StartInUsersDirectory", StringComparison.OrdinalIgnoreCase) >= 0 || value == "0")
                return Mode.StartInUsersDirectory.ToString();
            if (value.IndexOf("IsolateRootDirectoryOnly", StringComparison.OrdinalIgnoreCase) >= 0 || value == "1")
                return Mode.IsolateRootDirectoryOnly.ToString();
            if (value.IndexOf("IsolateAllDirectories", StringComparison.OrdinalIgnoreCase) >= 0 || value == "2")
                return Mode.IsolateAllDirectories.ToString();
            if (value.IndexOf("ActiveDirectory", StringComparison.OrdinalIgnoreCase) >= 0 || value == "3")
                return Mode.ActiveDirectory.ToString();
            if (value.IndexOf("None", StringComparison.OrdinalIgnoreCase) >= 0 || value == "4")
                return Mode.None.ToString();

            return Mode.None.ToString();
        }

        private string GetFtpSiteLogDirectory(string siteId)
        {
            string stdOut;
            string stdErr;
            int exitCode;
            string script = "Import-Module WebAdministration -ErrorAction Stop; (Get-Item ('IIS:\\Sites\\' + '" + EscapePowerShellLiteral(siteId) + "')).logFile.directory";
            if (!RunPowerShell(script, out exitCode, out stdOut, out stdErr) || exitCode != 0)
                return DefaultFtpSiteLogsFolder;
            return (stdOut ?? String.Empty).Trim();
        }

        public override bool IsInstalled()
        {
            return IsMsFTPInstalled();
        }
    }
}


