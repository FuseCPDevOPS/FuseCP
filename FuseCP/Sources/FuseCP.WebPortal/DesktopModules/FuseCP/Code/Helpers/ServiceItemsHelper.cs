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

namespace FuseCP.Portal
{
    /// <summary>
    /// Summary description for ServerItemsHelper
    /// </summary>
    public class ServiceItemsHelper
    {
        private static int GetPagedCount(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return 0;

            DataTable countTable = dataSet.Tables[0];
            if (countTable == null || countTable.Rows.Count == 0 || countTable.Columns.Count == 0)
                return 0;

            return Utils.ParseInt(countTable.Rows[0][0], 0);
        }

        private static DataTable GetPagedTable(DataSet dataSet, int tableIndex)
        {
            if (dataSet == null || dataSet.Tables.Count <= tableIndex)
                return new DataTable();

            return dataSet.Tables[tableIndex] ?? new DataTable();
        }

        #region Web Sites
        DataSet dsItemsPaged;

        public int GetServiceItemsPagedCount(int packageId, string groupName, string typeName,
            int serverId, bool recursive, string filterColumn, string filterValue)
        {
            return GetPagedCount(dsItemsPaged);
        }

        public DataTable GetServiceItemsPaged(int packageId, string groupName, string typeName,
            int serverId, bool recursive, string filterColumn, string filterValue,
            int maximumRows, int startRowIndex, string sortColumn)
        {
            dsItemsPaged = ES.Services.Packages.GetRawPackageItemsPaged(packageId, groupName, typeName, serverId,
                recursive, filterColumn, filterValue, sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsItemsPaged, 1);
        }
        #endregion

        #region Web Sites
        DataSet dsWebSitesPaged;

        public int GetWebSitesPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsWebSitesPaged);
        }

        public DataTable GetWebSitesPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsWebSitesPaged = ES.Services.WebServers.GetRawWebSitesPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsWebSitesPaged, 1);
        }
        #endregion

        #region Ftp Accounts
        DataSet dsFtpAccountsPaged;

        public int GetFtpAccountsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsFtpAccountsPaged);
        }

        public DataTable GetFtpAccountsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsFtpAccountsPaged = ES.Services.FtpServers.GetRawFtpAccountsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsFtpAccountsPaged, 1);
        }
        #endregion

        #region Mail Accounts
        DataSet dsMailAccountsPaged;

        public int GetMailAccountsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsMailAccountsPaged);
        }

        public DataTable GetMailAccountsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsMailAccountsPaged = ES.Services.MailServers.GetRawMailAccountsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsMailAccountsPaged, 1);
        }
        #endregion

        #region Mail Forwardings
        DataSet dsMailForwardingsPaged;

        public int GetMailForwardingsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsMailForwardingsPaged);
        }

        public DataTable GetMailForwardingsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsMailForwardingsPaged = ES.Services.MailServers.GetRawMailForwardingsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsMailForwardingsPaged, 1);
        }
        #endregion

        #region Mail Groups
        DataSet dsMailGroupsPaged;

        public int GetMailGroupsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsMailGroupsPaged);
        }

        public DataTable GetMailGroupsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsMailGroupsPaged = ES.Services.MailServers.GetRawMailGroupsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsMailGroupsPaged, 1);
        }
        #endregion

        #region Mail Lists
        DataSet dsMailListsPaged;

        public int GetMailListsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsMailListsPaged);
        }

        public DataTable GetMailListsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsMailListsPaged = ES.Services.MailServers.GetRawMailListsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsMailListsPaged, 1);
        }
        #endregion

        #region Mail Domains
        DataSet dsMailDomainsPaged;

        public int GetMailDomainsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsMailDomainsPaged);
        }

        public DataTable GetMailDomainsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsMailDomainsPaged = ES.Services.MailServers.GetRawMailDomainsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsMailDomainsPaged, 1);
        }
        #endregion

        #region Databases
        DataSet dsSqlDatabasesPaged;

        public int GetSqlDatabasesPagedCount(string groupName, string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSqlDatabasesPaged);
        }

        public DataTable GetSqlDatabasesPaged(int maximumRows, int startRowIndex, string sortColumn,
            string groupName, string filterColumn, string filterValue)
        {
            dsSqlDatabasesPaged = ES.Services.DatabaseServers.GetRawSqlDatabasesPaged(PanelSecurity.PackageId,
                groupName, filterColumn, filterValue, sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSqlDatabasesPaged, 1);
        }
        #endregion

        #region Database Users
        DataSet dsSqlUsersPaged;

        public int GetSqlUsersPagedCount(string groupName, string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSqlUsersPaged);
        }

        public DataTable GetSqlUsersPaged(int maximumRows, int startRowIndex, string sortColumn,
            string groupName, string filterColumn, string filterValue)
        {
            dsSqlUsersPaged = ES.Services.DatabaseServers.GetRawSqlUsersPaged(PanelSecurity.PackageId,
                groupName, filterColumn, filterValue, sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSqlUsersPaged, 1);
        }
        #endregion

        #region SharePoint Users
        DataSet dsSharePointUsersPaged;

        public int GetSharePointUsersPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSharePointUsersPaged);
        }

        public DataTable GetSharePointUsersPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsSharePointUsersPaged = ES.Services.SharePointServers.GetRawSharePointUsersPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSharePointUsersPaged, 1);
        }
        #endregion

        #region SharePoint Groups
        DataSet dsSharePointGroupsPaged;

        public int GetSharePointGroupsPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSharePointGroupsPaged);
        }

        public DataTable GetSharePointGroupsPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsSharePointGroupsPaged = ES.Services.SharePointServers.GetRawSharePointGroupsPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSharePointGroupsPaged, 1);
        }
        #endregion

        #region Statistics Items
        DataSet dsStatisticsItemsPaged;

        public int GetStatisticsSitesPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsStatisticsItemsPaged);
        }

        public DataTable GetStatisticsSitesPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsStatisticsItemsPaged = ES.Services.StatisticsServers.GetRawStatisticsSitesPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsStatisticsItemsPaged, 1);
        }
        #endregion

        #region SharePoint Sites
        DataSet dsSharePointSitesPaged;

        public int GetSharePointSitesPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSharePointSitesPaged);
        }

        public DataTable GetSharePointSitesPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsSharePointSitesPaged = ES.Services.SharePointServers.GetRawSharePointSitesPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSharePointSitesPaged, 1);
        }
        #endregion

        #region ODBC DSNs
        DataSet dsOdbcSourcesPaged;

        public int GetOdbcSourcesPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsOdbcSourcesPaged);
        }

        public DataTable GetOdbcSourcesPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsOdbcSourcesPaged = ES.Services.OperatingSystems.GetRawOdbcSourcesPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsOdbcSourcesPaged, 1);
        }
        #endregion

        #region Shared SSL Folders
        DataSet dsSharedSSLFoldersPaged;

        public int GetSharedSSLFoldersPagedCount(string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSharedSSLFoldersPaged);
        }

        public DataTable GetSharedSSLFoldersPaged(int maximumRows, int startRowIndex, string sortColumn,
            string filterColumn, string filterValue)
        {
            dsSharedSSLFoldersPaged = ES.Services.WebServers.GetRawSSLFoldersPaged(PanelSecurity.PackageId, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSharedSSLFoldersPaged, 1);
        }
        #endregion
    }
}
