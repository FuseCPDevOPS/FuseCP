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
using FuseCP.EnterpriseServer;

namespace FuseCP.Portal
{
    /// <summary>
    /// Summary description for SchedulesHelper
    /// </summary>
    public class SchedulesHelper
    {
        private static readonly string[] TaskIdColumnNames = { "TaskID", "TaskId" };

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

        private static string GetTaskId(DataRow row)
        {
            if (row == null)
                return String.Empty;

            foreach (string columnName in TaskIdColumnNames)
            {
                if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                    return row[columnName]?.ToString() ?? String.Empty;
            }

            return String.Empty;
        }

        private static HashSet<string> GetAllowedTaskIds()
        {
            var allowedTaskIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ScheduleTaskInfo[] tasks = ES.Services.Scheduler.GetScheduleTasks();

            if (tasks == null)
                return allowedTaskIds;

            foreach (ScheduleTaskInfo task in tasks)
            {
                if (!String.IsNullOrWhiteSpace(task?.TaskId))
                    allowedTaskIds.Add(task.TaskId);
            }

            return allowedTaskIds;
        }

        private static void FilterSchedulesByAllowedTasks(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count <= 1)
                return;

            HashSet<string> allowedTaskIds = GetAllowedTaskIds();
            if (allowedTaskIds.Count == 0)
            {
                dataSet.Tables[1].Rows.Clear();
                return;
            }

            DataTable schedulesTable = dataSet.Tables[1];
            for (int rowIndex = schedulesTable.Rows.Count - 1; rowIndex >= 0; rowIndex--)
            {
                DataRow row = schedulesTable.Rows[rowIndex];
                string taskId = GetTaskId(row);

                if (String.IsNullOrEmpty(taskId) || !allowedTaskIds.Contains(taskId))
                    schedulesTable.Rows.RemoveAt(rowIndex);
            }
        }

        private static void AddServerNames(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count <= 1)
                return;

            DataTable schedulesTable = dataSet.Tables[1];
            if (!schedulesTable.Columns.Contains("ServerName"))
                schedulesTable.Columns.Add("ServerName", typeof(string));

            var serverNames = new Dictionary<int, string>();

            foreach (DataRow row in schedulesTable.Rows)
            {
                int packageId = Utils.ParseInt(row["PackageID"], 0);
                int serverId = PackagesHelper.GetCachedPackageContext(packageId)?.Package?.ServerId ?? 0;

                if (serverId <= 0)
                {
                    row["ServerName"] = String.Empty;
                    continue;
                }

                if (!serverNames.TryGetValue(serverId, out string serverName))
                {
                    ServerInfo server = ES.Services.Servers.GetServerById(serverId);
                    serverName = server?.ServerName ?? String.Empty;
                    serverNames[serverId] = serverName;
                }

                row["ServerName"] = serverName;
            }
        }

        private static void PrepareSchedules(DataSet dataSet)
        {
            FilterSchedulesByAllowedTasks(dataSet);
            AddServerNames(dataSet);
        }

        public DataSet GetRawSchedules()
        {
            DataSet schedules = ES.Services.Scheduler.GetSchedules(PanelSecurity.SelectedUserId);
            PrepareSchedules(schedules);
            return schedules;
        }

        public DataSet GetFilteredSchedules(int packageId)
        {
            DataSet schedules = ES.Services.Scheduler.GetSchedules(packageId);
            PrepareSchedules(schedules);
            return schedules;
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

            PrepareSchedules(dsSchedulesPaged);

            return GetPagedTable(dsSchedulesPaged, 1);
        }
    }
}
