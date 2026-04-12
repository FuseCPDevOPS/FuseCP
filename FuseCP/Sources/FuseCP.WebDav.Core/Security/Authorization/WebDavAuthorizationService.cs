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
using System.Threading;
using Cobalt;
using FuseCP.EnterpriseServer.Base.HostedSolution;
using FuseCP.Providers.HostedSolution;
using FuseCP.WebDav.Core.Config;
using FuseCP.WebDav.Core.Extensions;
using FuseCP.WebDav.Core.Interfaces.Security;
using FuseCP.WebDav.Core.Security.Authentication.Principals;
using FuseCP.WebDav.Core.Security.Authorization.Enums;
using FuseCP.WebDav.Core.Scp.Framework;

namespace FuseCP.WebDav.Core.Security.Authorization
{
    public class WebDavAuthorizationService : IWebDavAuthorizationService
    {
        private static readonly AsyncLocal<Dictionary<string, object>> SessionItems = new AsyncLocal<Dictionary<string, object>>();

        private static Dictionary<string, object> CurrentSession
        {
            get
            {
                if (SessionItems.Value == null)
                {
                    SessionItems.Value = new Dictionary<string, object>();
                }

                return SessionItems.Value;
            }
        }

        public bool HasAccess(ScpPrincipal principal, string path)
        {
            path = path.RemoveLeadingFromPath(principal.OrganizationId);

            var permissions = GetPermissions(principal, path);

            return permissions.HasFlag(WebDavPermissions.Read) || permissions.HasFlag(WebDavPermissions.Write);
        }

        public WebDavPermissions GetPermissions(ScpPrincipal principal, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return WebDavPermissions.Read;
            }

            var resultPermissions = WebDavPermissions.Empty;

            var rootFolder = GetRootFolder(path);

            var userGroups = GetUserSecurityGroups(principal);

            var permissions = GetFolderEsPermissions(principal, rootFolder);

            bool MatchesPrincipal(ESPermission perm)
            {
                bool isUserMatch = !perm.IsGroup && (perm.DisplayName == principal.UserName || perm.DisplayName == principal.DisplayName);
                bool isGroupMatch = perm.IsGroup && userGroups.Any(x => x.DisplayName == perm.DisplayName);
                return isUserMatch || isGroupMatch;
            }
            foreach (var permission in permissions.Where(MatchesPrincipal))
            {
                if (permission.Access.ToLowerInvariant().Contains("read"))
                {
                    resultPermissions |= WebDavPermissions.Read;
                }

                if (permission.Access.ToLowerInvariant().Contains("write"))
                {
                    resultPermissions |= WebDavPermissions.Write;
                }
            }

            var owaEditFolders = GetOwaFoldersWithEditPermission(principal);

            if (owaEditFolders.Contains(rootFolder))
            {
                resultPermissions |= WebDavPermissions.OwaEdit;
            }
            else
            {
                resultPermissions |= WebDavPermissions.OwaRead;
            }

            return resultPermissions;
        }

        private string GetRootFolder(string path)
        {
            return path.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private IEnumerable<ESPermission> GetFolderEsPermissions(ScpPrincipal principal, string rootFolderName)
        {
            CurrentSession.TryGetValue(WebDavAppConfigManager.Instance.SessionKeys.WebDavRootFoldersPermissions, out var permissionsObj);
            var dictionary = permissionsObj as Dictionary<string, IEnumerable<ESPermission>>;

            if (dictionary == null)
            {
                dictionary = new Dictionary<string, IEnumerable<ESPermission>>();

                var rootFolders = FCP.Services.EnterpriseStorage.GetEnterpriseFoldersPaged(principal.ItemId, false,false, false,"","",0, int.MaxValue).PageItems;

                foreach (var rootFolder in rootFolders)
                {
                    var permissions = FCP.Services.EnterpriseStorage.GetEnterpriseFolderPermissions(principal.ItemId, rootFolder.Name);

                    dictionary.Add(rootFolder.Name, permissions);
                }

                CurrentSession[WebDavAppConfigManager.Instance.SessionKeys.WebDavRootFoldersPermissions] = dictionary;
            }

            return dictionary.TryGetValue(rootFolderName, out var _ckv) ? _ckv : new ESPermission[0];
        }

        public IEnumerable<ExchangeAccount> GetUserSecurityGroups(ScpPrincipal principal)
        {
            CurrentSession.TryGetValue(WebDavAppConfigManager.Instance.SessionKeys.UserGroupsKey, out var groupsObj);
            var groups = groupsObj as IEnumerable<ExchangeAccount>;

            if (groups == null)
            {
                 groups = FCP.Services.Organizations.GetSecurityGroupsByMember(principal.ItemId, principal.AccountId);

                CurrentSession[WebDavAppConfigManager.Instance.SessionKeys.UserGroupsKey] = groups;
            }

            return groups ?? new ExchangeAccount[0];
        }

        private IEnumerable<string> GetOwaFoldersWithEditPermission(ScpPrincipal principal)
        {
            CurrentSession.TryGetValue(WebDavAppConfigManager.Instance.SessionKeys.OwaEditFoldersSessionKey, out var foldersObj);
            var folders = foldersObj as IEnumerable<string>;

            if (folders != null)
            {
                return folders;
            }

            var accountsIds = new List<int>();

            accountsIds.Add(principal.AccountId);

            var groups = GetUserSecurityGroups(principal);

            accountsIds.AddRange(groups.Select(x=>x.AccountId));

            try
            {
                folders = ScpContext.Services.EnterpriseStorage.GetUserEnterpriseFolderWithOwaEditPermission(principal.ItemId, accountsIds.ToArray());
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                //TODO remove try catch when es &portal will be updated
                return new List<string>();
            }


            CurrentSession[WebDavAppConfigManager.Instance.SessionKeys.OwaEditFoldersSessionKey] = folders;

            return folders;
        }
    }
}
