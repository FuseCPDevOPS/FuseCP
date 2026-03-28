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
    /// Summary description for SchedulesHelper
    /// </summary>
    public class SchedulesHelper
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

        public DataSet GetRawSchedules()
        {
            return ES.Services.Scheduler.GetSchedules(PanelSecurity.SelectedUserId);
        }

        DataSet dsSchedulesPaged;

        public int GetSchedulesPagedCount(bool recursive, string filterColumn, string filterValue)
        {
            return GetPagedCount(dsSchedulesPaged);
        }

        public DataTable GetSchedulesPaged(int maximumRows, int startRowIndex, string sortColumn,
            bool recursive, string filterColumn, string filterValue)
        {
            dsSchedulesPaged = ES.Services.Scheduler.GetSchedulesPaged(PanelSecurity.PackageId, recursive, filterColumn, filterValue,
                sortColumn, startRowIndex, maximumRows);

            return GetPagedTable(dsSchedulesPaged, 1);
        }
    }
}
