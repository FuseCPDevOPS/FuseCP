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
using System.Collections.Generic;
using System.Text;

using FuseCP.Providers.HostedSolution;
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
	public class ExchangeHelper
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

		#region Exchange Organizations
		DataSet orgs;

		public int GetExchangeOrganizationsPagedCount(int packageId,
			bool recursive, string filterColumn, string filterValue)
		{
			return GetPagedCount(orgs);
		}

		public DataTable GetExchangeOrganizationsPaged(int packageId,
			bool recursive, string filterColumn, string filterValue,
			int maximumRows, int startRowIndex, string sortColumn)
		{
			if (!String.IsNullOrEmpty(filterValue))
				filterValue = filterValue + "%";

			orgs = ES.Services.ExchangeServer.GetRawExchangeOrganizationsPaged(packageId,
				recursive, filterColumn, filterValue, sortColumn, startRowIndex, maximumRows);

			return GetPagedTable(orgs, 1);
		}
		#endregion

		#region Accounts
		ExchangeAccountsPaged accounts;

		public int GetExchangeAccountsPagedCount(int itemId, string accountTypes,
            string filterColumn, string filterValue, bool archiving)
		{
			return accounts.RecordsCount;
		}

        public int GetExchangeAccountsPagedCount(int itemId, string accountTypes,
            string filterColumn, string filterValue)
        {
            return accounts.RecordsCount;
        }

        public ExchangeAccount[] GetExchangeAccountsPaged(int itemId, string accountTypes,
            string filterColumn, string filterValue,
            int maximumRows, int startRowIndex, string sortColumn)
        {
            return GetExchangeAccountsPaged(itemId, accountTypes,
            filterColumn, filterValue,
            maximumRows, startRowIndex, sortColumn, false);
        }

		public ExchangeAccount[] GetExchangeAccountsPaged(int itemId, string accountTypes,
			string filterColumn, string filterValue,
            int maximumRows, int startRowIndex, string sortColumn, bool archiving)
		{
			if (!String.IsNullOrEmpty(filterValue))
				filterValue = filterValue + "%";

            accounts = ES.Services.ExchangeServer.GetAccountsPaged(itemId,
                accountTypes, filterColumn, filterValue, sortColumn, startRowIndex, maximumRows, archiving);

			return accounts.PageItems;
		}

		#endregion
	}
}
