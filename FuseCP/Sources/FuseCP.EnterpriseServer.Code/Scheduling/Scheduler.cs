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
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
#if !EF64
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif

namespace FuseCP.EnterpriseServer
{
    public delegate void ScheduleFinished(SchedulerJob schedule);

    public sealed class Scheduler: ControllerBase
    {
        private const int StartingGraceSeconds = 180;
        private const string MisfirePolicyRunOnce = "RUN_ONCE";
        private const string MisfirePolicySkip = "SKIP";

        public Scheduler(ControllerBase provider) : base(provider) { }
        public Scheduler() : this(null) { }

        public SchedulerJob nextSchedule = null;

        public void Start()
        {
            ScheduleTasks();
        }

        public bool IsScheduleActive(int scheduleId)
        {
            return GetScheduleStatus(scheduleId) != ScheduleStatus.Idle;
        }

        public ScheduleStatus GetScheduleStatus(int scheduleId)
        {
            Dictionary<int, BackgroundTask> scheduledTasks = TaskManager.GetScheduledTasks();

            if (scheduledTasks.ContainsKey(scheduleId))
                return ScheduleStatus.Running;

            if (SchedulerExecutionQueue.IsQueued(scheduleId))
                return ScheduleStatus.Queued;

            return ScheduleStatus.Idle;
        }

        public void ScheduleTasks()
        {
            RunManualTasks();

            nextSchedule = SchedulerController.GetNextSchedule();

            if (nextSchedule != null && nextSchedule.ScheduleInfo.NextRun <= DateTime.Now)
            {
                RunNextSchedule(null);
            }
        }

        private void RunManualTasks()
        {
            RecoverStaleProcessTasks();

            var tasks = TaskController.GetProcessTasks(BackgroundTaskStatus.Stopping);

            foreach (var task in tasks)
            {
                SchedulerExecutionQueue.TryCancel(task.Id);
                TaskManager.StopTask(task.TaskId);
            }

            tasks = TaskController.GetProcessTasks(BackgroundTaskStatus.Starting);

            foreach (var task in tasks)
            {
                var hydratedTask = TaskController.GetTask(task.TaskId) ?? task;

                bool enqueued = SchedulerExecutionQueue.TryEnqueue(
                    hydratedTask.Id,
                    ResolveRuntimeAffinityKey(hydratedTask),
                    ResolveRuntimeTenantKey(hydratedTask),
                    ResolveRuntimeProviderThrottleKey(hydratedTask),
                    () => RunBackgroundTask(hydratedTask),
                    ResolveRuntimeWeight(hydratedTask));

                if (!enqueued)
                    continue;
            }
        }

        private string ResolveRuntimeAffinityKey(BackgroundTask task)
        {
            string explicitAffinity = GetTaskParameterValue(task, "SCHEDULER_TARGET_AFFINITY", "SCHEDULER_AFFINITY", "SERVER_ID", "SCHEDULER_TARGET_SERVER_ID", "SERVER_NAME");
            if (!String.IsNullOrWhiteSpace(explicitAffinity))
            {
                string trimmed = explicitAffinity.Trim();
                if (Int32.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numericServerId) && numericServerId > 0)
                    return "server:" + numericServerId.ToString(CultureInfo.InvariantCulture);

                return trimmed;
            }

            if (task != null && task.PackageId > 0)
            {
                PackageInfo package = PackageController.GetPackage(task.PackageId);
                if (package != null && package.ServerId > 0)
                    return "server:" + package.ServerId.ToString(CultureInfo.InvariantCulture);
            }

            return "global";
        }

        private static int ResolveRuntimeWeight(BackgroundTask task)
        {
            string weightValue = GetTaskParameterValue(task, "SCHEDULER_WEIGHT", "TASK_WEIGHT", "WEIGHT");
            if (Int32.TryParse(weightValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int configuredWeight) && configuredWeight > 0)
                return configuredWeight;

            if (task != null)
            {
                if (task.MaximumExecutionTime >= 1800)
                    return FuseCP.Web.Services.Configuration.SchedulerHeavyTaskWeight;

                if (task.MaximumExecutionTime >= 600)
                    return FuseCP.Web.Services.Configuration.SchedulerMediumTaskWeight;
            }

            return FuseCP.Web.Services.Configuration.SchedulerDefaultTaskWeight;
        }

        private string ResolveRuntimeTenantKey(BackgroundTask task)
        {
            if (task == null)
                return "tenant:global";

            int tenantId = task.EffectiveUserId > 0 ? task.EffectiveUserId : task.UserId;
            if (tenantId <= 0 && task.PackageId > 0)
            {
                PackageInfo package = PackageController.GetPackage(task.PackageId);
                if (package != null)
                    tenantId = package.UserId;
            }

            return tenantId > 0
                ? "tenant:" + tenantId.ToString(CultureInfo.InvariantCulture)
                : "tenant:global";
        }

        private static string ResolveRuntimeProviderThrottleKey(BackgroundTask task)
        {
            string explicitProvider = GetTaskParameterValue(task, "SCHEDULER_PROVIDER_THROTTLE_KEY", "PROVIDER_THROTTLE_KEY");
            if (!String.IsNullOrWhiteSpace(explicitProvider))
                return explicitProvider.Trim();

            if (!String.IsNullOrWhiteSpace(task?.TaskName))
                return "provider:" + task.TaskName.Trim();

            if (!String.IsNullOrWhiteSpace(task?.Source))
                return "provider:" + task.Source.Trim();

            return "provider:global";
        }

        private static string GetTaskParameterValue(BackgroundTask task, params string[] names)
        {
            if (task?.Params == null || names == null || names.Length == 0)
                return String.Empty;

            foreach (string name in names)
            {
                if (String.IsNullOrWhiteSpace(name))
                    continue;

                BackgroundTaskParameter parameter = task.Params.FirstOrDefault(p =>
                    p != null && !String.IsNullOrWhiteSpace(p.Name)
                    && String.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

                if (parameter?.Value == null)
                    continue;

                string value = Convert.ToString(parameter.Value, CultureInfo.InvariantCulture);
                if (!String.IsNullOrWhiteSpace(value))
                    return value;
            }

            return String.Empty;
        }

        private void RecoverStaleProcessTasks()
        {
            DateTime now = DateTime.Now;

            foreach (var task in TaskController.GetProcessTasks(BackgroundTaskStatus.Starting))
            {
                double ageSeconds = (now - task.StartDate).TotalSeconds;
                if (ageSeconds <= StartingGraceSeconds)
                    continue;

                TaskManager.StopTask(task.TaskId);
                SchedulerController.ReleaseExpiredScheduleLease(task.ScheduleId);
                TaskManager.WriteWarning(task.Guid, "Recovered stale starting scheduler task '{0}' after {1} seconds", task.ItemName, ((int)ageSeconds).ToString());
            }

            foreach (var task in TaskController.GetProcessTasks(BackgroundTaskStatus.Run))
            {
                if (task.MaximumExecutionTime <= 0 || task.MaximumExecutionTime == -1)
                    continue;

                double ageSeconds = (now - task.StartDate).TotalSeconds;
                if (ageSeconds <= task.MaximumExecutionTime)
                    continue;

                TaskManager.StopTask(task.TaskId);
                SchedulerController.ReleaseExpiredScheduleLease(task.ScheduleId);
                TaskManager.WriteWarning(task.Guid, "Recovered stale running scheduler task '{0}' after {1} seconds", task.ItemName, ((int)ageSeconds).ToString());
            }
        }

        private void RunBackgroundTask(BackgroundTask backgroundTask)
        {
            UserInfo user = PackageController.GetPackageOwner(backgroundTask.PackageId);
            if (user != null)
            {
                SecurityContext.SetThreadPrincipal(user.UserId);
            }
            else
            {
                SecurityContext.SetThreadSupervisorPrincipal();
            }
            
            var schedule = SchedulerController.GetScheduleComplete(backgroundTask.ScheduleId);

            backgroundTask.Guid = TaskManager.Guid;
            backgroundTask.Status = BackgroundTaskStatus.Run;


            TaskController.UpdateTask(backgroundTask);
            TaskManager.Write("Scheduler task started: {0}", backgroundTask.ItemName);
            if (user == null)
            {
                TaskManager.WriteWarning("Package owner not found for package '{0}', running schedule as supervisor", backgroundTask.PackageId.ToString());
            }
            
            try
            {
                if (schedule == null || schedule.Task == null || string.IsNullOrWhiteSpace(schedule.Task.TaskType))
                {
                    throw new InvalidOperationException($"Scheduler metadata not found for ScheduleID={backgroundTask.ScheduleId}");
                }

                var objTask = (ISchedulerTask)Activator.CreateInstance(Type.GetType(schedule.Task.TaskType));
                if (objTask == null)
                {
                    throw new InvalidOperationException($"Could not create scheduled task type '{schedule.Task.TaskType}'");
                }

                objTask.DoWork();
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                TaskManager.WriteError(ex, "Error executing scheduled task");
            }
            finally
            {
                TaskManager.Write("Scheduler task finished: {0}", backgroundTask.ItemName);
                try
                {
                    TaskManager.CompleteTask();
                }
                catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
                {
                    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
                }
            }
        }

        // call back for the timer function
        void RunNextSchedule(object obj) // obj ignored
        {            
            if (nextSchedule == null)
                return;

            RunSchedule(nextSchedule, true);
        }

        void RunSchedule(SchedulerJob schedule, bool changeNextRun)
        {
            string leaseOwner = null;
            string leaseToken = null;
            bool leaseTransferred = false;

            if (Web.Services.Configuration.SchedulerFreezeEnabled)
            {
                if (changeNextRun && schedule?.ScheduleInfo != null)
                {
                    try
                    {
                        SchedulerController.CalculateNextStartTime(schedule.ScheduleInfo);
                        SchedulerController.UpdateSchedule(schedule.ScheduleInfo);
                    }
                    catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
                    {
                        System.Diagnostics.Trace.TraceWarning("Scheduler freeze reschedule warning: " + swallowedEx.Message);
                    }
                }

                System.Diagnostics.Trace.TraceWarning("Scheduler execution skipped because freeze mode is enabled.");
                return;
            }

            try
            {
                leaseOwner = SchedulerRuntime.GetLeaseOwner();
                leaseToken = Guid.NewGuid().ToString("N");
                var leaseDuration = SchedulerRuntime.GetLeaseDuration(schedule.ScheduleInfo.MaxExecutionTime);

                if (!SchedulerController.TryAcquireScheduleLease(schedule.ScheduleInfo.ScheduleId, leaseOwner, leaseToken, leaseDuration, out _))
                    return;

                schedule.LeaseOwner = leaseOwner;
                schedule.LeaseToken = leaseToken;

                bool skipExecution = changeNextRun && ShouldSkipMisfireExecution(schedule.ScheduleInfo, DateTime.Now);

                // update next run (if required)
                if (changeNextRun)
                {
                    SchedulerController.CalculateNextStartTime(schedule.ScheduleInfo);
                }

                // disable run once task
                if (schedule.ScheduleInfo.ScheduleType == ScheduleType.OneTime)
                    schedule.ScheduleInfo.Enabled = false;

                Dictionary<int, BackgroundTask> scheduledTasks = TaskManager.GetScheduledTasks();
                if (!scheduledTasks.ContainsKey(schedule.ScheduleInfo.ScheduleId))
                    // this task should be run, so
                    // update its last run
                    schedule.ScheduleInfo.LastRun = DateTime.Now;

                // update schedule
                int MAX_RETRY_COUNT = 10;
                int counter = 0;
                while (counter < MAX_RETRY_COUNT)
                {
                    try
                    {
                        SchedulerController.UpdateSchedule(schedule.ScheduleInfo);
                        break;
                    }
                    catch (System.Data.Common.DbException)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                    counter++;
                }
                if (counter == MAX_RETRY_COUNT)
                    return;

                if (skipExecution)
                {
                    TaskManager.WriteWarning("Skipping misfired schedule execution for '{0}' according to policy '{1}'",
                        schedule.ScheduleInfo.ScheduleName,
                        MisfirePolicySkip);
                    return;
                }

                // skip execution if the current task is still running
                scheduledTasks = TaskManager.GetScheduledTasks();
                if (!scheduledTasks.ContainsKey(schedule.ScheduleInfo.ScheduleId) && !SchedulerExecutionQueue.IsQueued(schedule.ScheduleInfo.ScheduleId))
                {
                    // run the schedule in the separate thread
                    if (!schedule.Run())
                        return;

                    leaseTransferred = true;
                }
            }
            catch (System.Exception Ex) when (!(Ex is System.OutOfMemoryException) && !(Ex is System.StackOverflowException) && !(Ex is System.AccessViolationException))
            {
                try
                {
                    TaskManager.WriteError(string.Format("RunSchedule Error : {0}", Ex.Message));
                }
                catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
                {
                    System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
                }
            }
            finally
            {
                if (!leaseTransferred && !string.IsNullOrWhiteSpace(leaseOwner) && !string.IsNullOrWhiteSpace(leaseToken))
                {
                    SchedulerController.ReleaseScheduleLease(schedule.ScheduleInfo.ScheduleId, leaseOwner, leaseToken);
                }
            }
        }

        private static bool ShouldSkipMisfireExecution(ScheduleInfo scheduleInfo, DateTime now)
        {
            if (scheduleInfo == null)
                return false;

            if (!string.Equals(ResolveMisfirePolicy(scheduleInfo), MisfirePolicySkip, StringComparison.OrdinalIgnoreCase))
                return false;

            if (scheduleInfo.NextRun == DateTime.MinValue)
                return false;

            return scheduleInfo.NextRun < now.AddSeconds(-ResolveMisfireGraceSeconds(scheduleInfo));
        }

        private static int ResolveMisfireGraceSeconds(ScheduleInfo scheduleInfo)
        {
            int configuredGrace = ResolveIntScheduleParameter(scheduleInfo, 60, 60, 3600,
                "SCHEDULER_MISFIRE_GRACE_SECONDS",
                "MISFIRE_GRACE_SECONDS");

            if (scheduleInfo.ScheduleType == ScheduleType.Interval && scheduleInfo.Interval > 0)
                return Math.Max(configuredGrace, Math.Min(scheduleInfo.Interval, 3600));

            return configuredGrace;
        }

        private static string ResolveMisfirePolicy(ScheduleInfo scheduleInfo)
        {
            string configured = ResolveStringScheduleParameter(scheduleInfo,
                "SCHEDULER_MISFIRE_POLICY",
                "MISFIRE_POLICY");

            if (string.Equals(configured, MisfirePolicySkip, StringComparison.OrdinalIgnoreCase))
                return MisfirePolicySkip;

            return MisfirePolicyRunOnce;
        }

        private static int ResolveIntScheduleParameter(ScheduleInfo scheduleInfo, int defaultValue, int minValue, int maxValue, params string[] ids)
        {
            string value = ResolveStringScheduleParameter(scheduleInfo, ids);
            if (!Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed))
                return defaultValue;

            if (parsed < minValue)
                return minValue;

            if (parsed > maxValue)
                return maxValue;

            return parsed;
        }

        private static string ResolveStringScheduleParameter(ScheduleInfo scheduleInfo, params string[] ids)
        {
            if (scheduleInfo?.Parameters == null || scheduleInfo.Parameters.Length == 0 || ids == null)
                return String.Empty;

            foreach (string id in ids)
            {
                if (String.IsNullOrWhiteSpace(id))
                    continue;

                ScheduleTaskParameterInfo parameter = scheduleInfo.Parameters.FirstOrDefault(p =>
                    p != null
                    && !String.IsNullOrWhiteSpace(p.ParameterId)
                    && String.Equals(p.ParameterId, id, StringComparison.OrdinalIgnoreCase)
                    && !String.IsNullOrWhiteSpace(p.ParameterValue));

                if (parameter != null)
                    return parameter.ParameterValue.Trim();
            }

            return String.Empty;
        }
    }
}



