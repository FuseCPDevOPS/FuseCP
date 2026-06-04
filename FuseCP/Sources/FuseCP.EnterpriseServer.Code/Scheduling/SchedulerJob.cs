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
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace FuseCP.EnterpriseServer
{
    public class SchedulerJob: ControllerAsyncBase
    {
        private ScheduleInfo scheduleInfo;
        private ScheduleTaskInfo task;

        public ScheduleFinished ScheduleFinishedCallback;

        #region public properties
        public ScheduleInfo ScheduleInfo
        {
            get { return this.scheduleInfo; }
            set { this.scheduleInfo = value; }
        }

        public ScheduleTaskInfo Task
        {
            get { return this.task; }
            set { this.task = value; }
        }

		public string LeaseOwner { get; set; }
		public string LeaseToken { get; set; }
        #endregion

        // Sets the next time this Schedule is kicked off and kicks off events on
        // a seperate thread, freeing the Scheduler to continue
        public bool Run()
        {
            return SchedulerExecutionQueue.TryEnqueue(scheduleInfo.ScheduleId, ResolveAffinityKey(), RunSchedule, ResolveTaskWeight());
        }

        private int ResolveTaskWeight()
        {
            int configuredWeight = ResolveWeightFromParameters();
            if (configuredWeight > 0)
                return configuredWeight;

            int? recommendedWeight = SchedulerTaskWeightAdvisor.GetRecommendedWeight(
                task?.TaskType,
                FuseCP.Web.Services.Configuration.SchedulerDefaultTaskWeight,
                FuseCP.Web.Services.Configuration.SchedulerMediumTaskWeight,
                FuseCP.Web.Services.Configuration.SchedulerHeavyTaskWeight);
            if (recommendedWeight.HasValue)
                return recommendedWeight.Value;

            string taskType = task?.TaskType ?? string.Empty;
            StringComparison cmp = StringComparison.OrdinalIgnoreCase;

            if (taskType.IndexOf("CalculatePackagesDiskspaceTask", cmp) >= 0 ||
                taskType.IndexOf("CalculatePackagesBandwidthTask", cmp) >= 0 ||
                taskType.IndexOf("CalculateExchangeDiskspaceTask", cmp) >= 0 ||
                taskType.IndexOf("BackupTask", cmp) >= 0 ||
                taskType.IndexOf("BackupDatabaseTask", cmp) >= 0)
            {
                return FuseCP.Web.Services.Configuration.SchedulerHeavyTaskWeight;
            }

            if (taskType.IndexOf("HostedSolutionReport", cmp) >= 0 ||
                taskType.IndexOf("NotifyOverusedDatabasesTask", cmp) >= 0 ||
                taskType.IndexOf("SuspendOverusedPackagesTask", cmp) >= 0)
            {
                return FuseCP.Web.Services.Configuration.SchedulerMediumTaskWeight;
            }

            int maxExecutionTime = scheduleInfo?.MaxExecutionTime ?? 0;
            if (maxExecutionTime >= 1800)
                return FuseCP.Web.Services.Configuration.SchedulerHeavyTaskWeight;
            if (maxExecutionTime >= 600)
                return FuseCP.Web.Services.Configuration.SchedulerMediumTaskWeight;

            return FuseCP.Web.Services.Configuration.SchedulerDefaultTaskWeight;
        }

        private int ResolveWeightFromParameters()
        {
            if (scheduleInfo?.Parameters == null || scheduleInfo.Parameters.Length == 0)
                return 0;

            var prm = scheduleInfo.Parameters.FirstOrDefault(p =>
                p != null &&
                (string.Equals(p.ParameterId, "SCHEDULER_WEIGHT", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.ParameterId, "TASK_WEIGHT", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.ParameterId, "WEIGHT", StringComparison.OrdinalIgnoreCase)));

            if (prm == null || string.IsNullOrWhiteSpace(prm.ParameterValue))
                return 0;

            return int.TryParse(prm.ParameterValue, out int weight) ? weight : 0;
        }

        private string ResolveAffinityKey()
        {
            string affinityFromParameter = ResolveAffinityKeyFromParameters();
            if (!string.IsNullOrWhiteSpace(affinityFromParameter))
                return affinityFromParameter;

            if (scheduleInfo != null && scheduleInfo.PackageId > 0)
            {
                var package = PackageController.GetPackage(scheduleInfo.PackageId);
                if (package != null && package.ServerId > 0)
                    return $"server:{package.ServerId}";
            }

            return "global";
        }

        private string ResolveAffinityKeyFromParameters()
        {
            if (scheduleInfo?.Parameters == null || scheduleInfo.Parameters.Length == 0)
                return null;

            var affinityParameter = scheduleInfo.Parameters.FirstOrDefault(p =>
                p != null &&
                (string.Equals(p.ParameterId, "SCHEDULER_AFFINITY", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.ParameterId, "SERVER_ID", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(p.ParameterId, "SERVER_NAME", StringComparison.OrdinalIgnoreCase)));

            if (affinityParameter == null || string.IsNullOrWhiteSpace(affinityParameter.ParameterValue))
                return null;

            if (string.Equals(affinityParameter.ParameterId, "SERVER_ID", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(affinityParameter.ParameterValue, out int serverId) && serverId > 0)
                return $"server:{serverId}";

            return affinityParameter.ParameterValue.Trim();
        }

        // Implementation of ThreadStart delegate.
        // Used by Scheduler to kick off events on a seperate thread
        private void RunSchedule()
        {
            if (string.IsNullOrWhiteSpace(LeaseOwner) || string.IsNullOrWhiteSpace(LeaseToken))
                return;

            Stopwatch stopwatch = Stopwatch.StartNew();

            // impersonate thread
            UserInfo user = PackageController.GetPackageOwner(scheduleInfo.PackageId);
            if (user != null)
            {
                SecurityContext.SetThreadPrincipal(user.UserId);
            }
            else
            {
                SecurityContext.SetThreadSupervisorPrincipal();
            }

            var leaseDuration = SchedulerRuntime.GetLeaseDuration(scheduleInfo.MaxExecutionTime);

            var parameters = scheduleInfo.Parameters
                .Select(prm => new BackgroundTaskParameter(prm.ParameterId, prm.ParameterValue))
                .ToList();

            using (var lease = new SchedulerLeaseHeartbeat(SchedulerController, scheduleInfo.ScheduleId, LeaseOwner, LeaseToken, leaseDuration))
            {
                TaskManager.StartTask("SCHEDULER", "RUN_SCHEDULE", scheduleInfo.ScheduleName, scheduleInfo.ScheduleId,
                                      scheduleInfo.ScheduleId, scheduleInfo.PackageId, scheduleInfo.MaxExecutionTime,
                                      parameters);
                TaskManager.Write("Scheduler task started: {0}", scheduleInfo.ScheduleName);
                if (user == null)
                {
                    TaskManager.WriteWarning("Package owner not found for package '{0}', running schedule as supervisor", scheduleInfo.PackageId.ToString());
                }

                try
                {
                    if (task == null || string.IsNullOrWhiteSpace(task.TaskType))
                    {
                        throw new InvalidOperationException($"Scheduler task type not found for schedule '{scheduleInfo.ScheduleId}'");
                    }

                    ISchedulerTask objTask = (ISchedulerTask)Activator.CreateInstance(Type.GetType(task.TaskType));

                    if (objTask != null)
                        objTask.DoWork();
                    else
                        throw new Exception(String.Format("Could not create scheduled task of '{0}' type",
                            task.TaskType));      
                    // Thread.Sleep(40000);
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    // log error
                    TaskManager.WriteError(ex, "Error executing scheduled task");
                }
                finally
                {
                    stopwatch.Stop();
                    SchedulerTaskWeightAdvisor.RecordExecution(task?.TaskType, stopwatch.Elapsed);
                    TaskManager.Write("Scheduler task finished: {0}. Duration: {1}",
                        scheduleInfo.ScheduleName,
                        stopwatch.Elapsed.ToString());

                    try
                    {
                        TaskManager.CompleteTask();
                    }
                    catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
                    {
                        System.Diagnostics.Trace.TraceWarning("Exception swallowed:" + swallowedEx.Message);
                    }
                }
            }
        }
    }
}



