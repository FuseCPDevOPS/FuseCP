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

        public Scheduler(ControllerBase provider) : base(provider) { }
        public Scheduler() : this(null) { }

        public SchedulerJob nextSchedule = null;

        public void Start()
        {
            ScheduleTasks();
        }

        public bool IsScheduleActive(int scheduleId)
        {
            Dictionary<int, BackgroundTask> scheduledTasks = TaskManager.GetScheduledTasks();

            return scheduledTasks.ContainsKey(scheduleId) || SchedulerExecutionQueue.IsQueued(scheduleId);
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
                TaskManager.StopTask(task.TaskId);
            }

            tasks = TaskController.GetProcessTasks(BackgroundTaskStatus.Starting);

            foreach (var task in tasks)
            {
                var taskThread = new Thread(() => RunBackgroundTask(task)) { Priority = ThreadPriority.Highest };
                taskThread.Start();
                TaskManager.AddTaskThread(task.Id, taskThread);
            }
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

            try
            {
                leaseOwner = SchedulerRuntime.GetLeaseOwner();
                leaseToken = Guid.NewGuid().ToString("N");
                var leaseDuration = SchedulerRuntime.GetLeaseDuration(schedule.ScheduleInfo.MaxExecutionTime);

                if (!SchedulerController.TryAcquireScheduleLease(schedule.ScheduleInfo.ScheduleId, leaseOwner, leaseToken, leaseDuration, out _))
                    return;

                schedule.LeaseOwner = leaseOwner;
                schedule.LeaseToken = leaseToken;

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
    }
}



