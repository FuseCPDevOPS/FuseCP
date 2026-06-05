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
using System.Xml;
using System.Linq;
using System.Globalization;
using System.Threading;
using FuseCP.EnterpriseServer.Base.Scheduling;
using FuseCP.EnterpriseServer.Data;
using FuseCP.EnterpriseServer.Data.Entities;
using ScheduleTaskViewConfiguration = FuseCP.EnterpriseServer.Base.Scheduling.ScheduleTaskViewConfiguration;


namespace FuseCP.EnterpriseServer
{
    public class SchedulerController: ControllerBase
    {
        private const string SchedulerLeaseSettingsName = "SchedulerLease";
        private static readonly string[] SchedulerExecutionModeParameterIds = { "SCHEDULER_EXECUTION_MODE", "EXECUTION_MODE", "SCHEDULER_MODE" };
        private static readonly string[] SchedulerAffinityParameterIds = { "SCHEDULER_TARGET_AFFINITY", "SCHEDULER_AFFINITY", "SERVER_ID", "SERVER_NAME" };
        private static readonly string[] SchedulerParallelismModeParameterIds = { "SCHEDULER_PARALLELISM_MODE", "PARALLELISM_MODE", "SCHEDULER_TASK_PARALLELISM_MODE" };
        private static readonly string[] SchedulerParallelismMaxParameterIds = { "SCHEDULER_PARALLELISM_MAX", "PARALLELISM_MAX", "SCHEDULER_TASK_PARALLELISM_MAX" };
        private const string SchedulerParallelismEffectiveParameterId = "SCHEDULER_PARALLELISM_EFFECTIVE";
        private const string SchedulerParallelismSourceParameterId = "SCHEDULER_PARALLELISM_SOURCE";
        private const string SchedulerTargetServerIdParameterId = "SCHEDULER_TARGET_SERVER_ID";
        private const string SchedulerDispatchNodeParameterId = "SCHEDULER_DISPATCH_NODE";
        private const string SchedulerRunTokenParameterId = "SCHEDULER_RUN_TOKEN";
        private const string SchedulerIdempotencyKeyParameterId = "SCHEDULER_IDEMPOTENCY_KEY";
        private const string SchedulerRiskLevelParameterId = "SCHEDULER_RISK_LEVEL";
        private const string SchedulerApprovedParameterId = "SCHEDULER_APPROVED";
        private const string SchedulerApprovedByParameterId = "SCHEDULER_APPROVED_BY_USERID";
        private const string SchedulerApprovedAtParameterId = "SCHEDULER_APPROVED_AT_UTC";
        private const string SchedulerApprovalStateParameterId = "SCHEDULER_APPROVAL_STATE";
        private const string SchedulerFirstApproverParameterId = "SCHEDULER_FIRST_APPROVER_USERID";
        private const string SchedulerSecondApproverParameterId = "SCHEDULER_SECOND_APPROVER_USERID";
        private const string ApprovalStatePendingSecond = "PENDING_SECOND_APPROVAL";
        private const string ApprovalStateApproved = "APPROVED";
        private static readonly string[] HighRiskTaskIdMarkers = { "SYSTEM_COMMAND", "DELETE_EXCHANGE", "SUSPEND_OVERUSED", "BACKUP_DATABASE" };

        private const string ExecutionModeAuto = "AUTO";
        private const string ExecutionModeServerPreferred = "SERVER_PREFERRED";
        private const string ExecutionModeEnterpriseOnly = "ENTERPRISE_ONLY";
        private const string ParallelismModeAuto = "AUTO";
        private const string ParallelismModeManual = "MANUAL";

        public SchedulerController(ControllerBase provider) : base(provider) { }

        public DateTime GetSchedulerTime()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return DateTime.MinValue;
            return DateTime.Now;
        }

        public int GetSchedulerRuntimePerAffinityConcurrency()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.MaxConcurrentExecutions;
        }

        public int GetSchedulerRuntimeGlobalConcurrency()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.MaxGlobalConcurrentExecutions;
        }

        public int GetSchedulerRuntimeQueueDepth()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.QueuedExecutions;
        }

        public int GetSchedulerRuntimeActiveUnits()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.ActiveExecutionUnits;
        }

        public bool GetSchedulerRuntimeFreezeEnabled()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return false;
            return Web.Services.Configuration.SchedulerFreezeEnabled;
        }

        public int GetSchedulerRuntimeTenantConcurrency()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.MaxTenantConcurrentExecutions;
        }

        public int GetSchedulerRuntimeProviderConcurrency()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.MaxProviderConcurrentExecutions;
        }

        public int GetSchedulerRuntimeActiveTenantBuckets()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.ActiveTenantBuckets;
        }

        public int GetSchedulerRuntimeActiveProviderBuckets()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.ActiveProviderBuckets;
        }

        public long GetSchedulerRuntimeTenantDeferrals()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.DeferralsTenant;
        }

        public long GetSchedulerRuntimeProviderDeferrals()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return 0;
            return SchedulerExecutionQueue.DeferralsProvider;
        }

        public int ApplySchedulerRuntimeConcurrency(int perAffinityConcurrency, int globalConcurrency)
        {
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive | DemandAccount.IsAdmin);
            if (accountCheck < 0)
                return accountCheck;

            SchedulerExecutionQueue.ConfigureMaxConcurrentExecutions(perAffinityConcurrency);
            SchedulerExecutionQueue.ConfigureGlobalMaxConcurrentExecutions(globalConcurrency);
            return 0;
        }

        public List<ScheduleTaskInfo> GetScheduleTasks()
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return new List<ScheduleTaskInfo>();
            return ObjectUtils.CreateListFromDataReader<ScheduleTaskInfo>(
                Database.GetScheduleTasks(SecurityContext.User.UserId));
        }

        public ScheduleTaskInfo GetScheduleTask(string taskId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return null;
            return ObjectUtils.FillObjectFromDataReader<ScheduleTaskInfo>(
                Database.GetScheduleTask(SecurityContext.User.UserId, taskId));
        }

        public DataSet GetSchedules(int packageId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return new DataSet();
            DataSet ds = Database.GetSchedules(SecurityContext.User.UserId, packageId);

            // set status to each returned schedule
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["StatusID"] = Scheduler.GetScheduleStatus((int)dr["ScheduleID"]);
            }
            return ds;
        }

        public DataSet GetSchedulesPaged(int packageId, bool recursive,
            string filterColumn, string filterValue, string sortColumn, int startRow, int maximumRows)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return new DataSet();
            DataSet ds = Database.GetSchedulesPaged(SecurityContext.User.UserId, packageId,
                recursive, filterColumn, filterValue, sortColumn, startRow, maximumRows);

            // set status to each returned schedule
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                dr["StatusID"] = Scheduler.GetScheduleStatus((int)dr["ScheduleID"]);
            }
            return ds;
        }

        public ScheduleInfo GetSchedule(int scheduleId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return null;
            DataSet ds = Database.GetSchedule(SecurityContext.User.UserId, scheduleId);
            ScheduleInfo si = ObjectUtils.FillObjectFromDataView<ScheduleInfo>(ds.Tables[0].DefaultView);
            return si;
        }

        /// <summary>
        /// Gets view configuration for a certain task.
        /// </summary>
        /// <param name="taskId">Task id for which view configuration is intended to be loeaded.</param>
        /// <returns>View configuration for the task with supplied id.</returns>
        public List<ScheduleTaskViewConfiguration> GetScheduleTaskViewConfigurations(string taskId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return new List<ScheduleTaskViewConfiguration>();
            List<ScheduleTaskViewConfiguration> c = ObjectUtils.CreateListFromDataReader<ScheduleTaskViewConfiguration>(Database.GetScheduleTaskViewConfigurations(taskId));
            return c;
        }

        internal SchedulerJob GetScheduleComplete(int scheduleId)
        {
            DataSet ds = Database.GetSchedule(SecurityContext.User.UserId, scheduleId);
            return CreateCompleteScheduleFromDataSet(ds);
        }

        internal SchedulerJob GetNextSchedule()
        {
            DataSet ds = Database.GetNextSchedule();
            return CreateCompleteScheduleFromDataSet(ds);
        }

        internal SchedulerJob CreateCompleteScheduleFromDataSet(DataSet ds)
        {
            if (ds.Tables[0].Rows.Count == 0)
                return null;

            SchedulerJob schedule = new SchedulerJob();

            // schedule info
            schedule.ScheduleInfo = ObjectUtils.FillObjectFromDataView<ScheduleInfo>(ds.Tables[0].DefaultView);

            // task info
            schedule.Task = ObjectUtils.FillObjectFromDataView<ScheduleTaskInfo>(ds.Tables[1].DefaultView);

            // parameters info
            List<ScheduleTaskParameterInfo> parameters = new List<ScheduleTaskParameterInfo>();
            ObjectUtils.FillCollectionFromDataView<ScheduleTaskParameterInfo>(
                parameters, ds.Tables[2].DefaultView);
            schedule.ScheduleInfo.Parameters = parameters.ToArray();

            return schedule;
        }

        public ScheduleInfo GetScheduleInternal(int scheduleId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return null;
            return ObjectUtils.FillObjectFromDataReader<ScheduleInfo>(
                Database.GetScheduleInternal(scheduleId));
        }

        public List<ScheduleTaskParameterInfo> GetScheduleParameters(string taskId, int scheduleId)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return new List<ScheduleTaskParameterInfo>();
            return ObjectUtils.CreateListFromDataReader<ScheduleTaskParameterInfo>(
                Database.GetScheduleParameters(SecurityContext.User.UserId,
                taskId, scheduleId));
        }

        public int StartSchedule(int scheduleId)
        {
            return StartScheduleInternal(scheduleId, false);
        }

        public int StartScheduleNow(int scheduleId)
        {
            return StartScheduleInternal(scheduleId, true);
        }

        private sealed class SchedulerDispatchPlan
        {
            public string EffectiveExecutionMode { get; set; }
            public string AffinityKey { get; set; }
            public string NodeHint { get; set; }
            public int? TargetServerId { get; set; }
            public string FallbackReason { get; set; }
        }

        private int StartScheduleInternal(int scheduleId, bool bypassQueue)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);

            if (accountCheck < 0)
                return accountCheck;

            if (Web.Services.Configuration.SchedulerFreezeEnabled && !SecurityContext.User.IsInRole(SecurityContext.ROLE_ADMINISTRATOR))
                return BusinessErrorCodes.ERROR_USER_ACCOUNT_NOT_ENOUGH_PERMISSIONS;

            SchedulerJob schedule = GetScheduleComplete(scheduleId);
            if (schedule == null)
                return 0;

            if (IsHighRiskSchedule(schedule.ScheduleInfo) && !IsHighRiskExecutionApproved(schedule.ScheduleInfo))
                return BusinessErrorCodes.ERROR_USER_ACCOUNT_NOT_ENOUGH_PERMISSIONS;

            bool hasExplicitExecutionMode = HasScheduleParameter(schedule.ScheduleInfo?.Parameters, SchedulerExecutionModeParameterIds);
            string configuredExecutionMode = NormalizeExecutionMode(GetScheduleParameterValue(schedule.ScheduleInfo?.Parameters, SchedulerExecutionModeParameterIds));
            string dispatchExecutionMode = ResolveDispatchExecutionMode(schedule, configuredExecutionMode, hasExplicitExecutionMode, out string placementNote);
            SchedulerDispatchPlan dispatchPlan = BuildDispatchPlan(schedule, dispatchExecutionMode);

            if (TaskController.GetScheduleTasks(scheduleId).Any(x => x.Status == BackgroundTaskStatus.Run
                                                                     || (x.Status == BackgroundTaskStatus.Starting && !SchedulerRuntime.IsStaleStartingTask(x))))
                return 0;

            if (!bypassQueue && SchedulerExecutionQueue.IsQueued(scheduleId))
                return 0;

            if (bypassQueue)
                SchedulerExecutionQueue.TryCancel(scheduleId);

            var parameters = (schedule.ScheduleInfo.Parameters ?? Array.Empty<ScheduleTaskParameterInfo>()).Select(
                prm => new BackgroundTaskParameter(prm.ParameterId, prm.ParameterValue)).ToList();
            string runToken = Guid.NewGuid().ToString("N");
            string idempotencyKey = String.Format(CultureInfo.InvariantCulture, "schedule:{0}:nextrun:{1:O}",
                scheduleId,
                schedule.ScheduleInfo.NextRun == DateTime.MinValue ? DateTime.UtcNow : schedule.ScheduleInfo.NextRun.ToUniversalTime());

            parameters.Add(new BackgroundTaskParameter("SCHEDULER_EXECUTION_MODE_CONFIGURED", configuredExecutionMode));
            parameters.Add(new BackgroundTaskParameter("SCHEDULER_EXECUTION_MODE_EFFECTIVE", dispatchPlan.EffectiveExecutionMode));
            UpsertBackgroundTaskParameter(parameters, SchedulerRunTokenParameterId, runToken);
            UpsertBackgroundTaskParameter(parameters, SchedulerIdempotencyKeyParameterId, idempotencyKey);
            UpsertBackgroundTaskParameter(parameters, SchedulerAffinityParameterIds[0], dispatchPlan.AffinityKey);
            UpsertBackgroundTaskParameter(parameters, SchedulerDispatchNodeParameterId, dispatchPlan.NodeHint);
            string parallelismNote = ApplyAdaptiveParallelismParameters(schedule, parameters);

            if (dispatchPlan.TargetServerId.HasValue)
                UpsertBackgroundTaskParameter(parameters, SchedulerTargetServerIdParameterId, dispatchPlan.TargetServerId.Value.ToString(CultureInfo.InvariantCulture));

            // update next run (if required)
            CalculateNextStartTime(schedule.ScheduleInfo);

            // disable run once task
            if (schedule.ScheduleInfo.ScheduleType == ScheduleType.OneTime)
                schedule.ScheduleInfo.Enabled = false;

            schedule.ScheduleInfo.LastRun = DateTime.Now;
            int scheduleUpdateResult = UpdateSchedule(schedule.ScheduleInfo);
            if (scheduleUpdateResult < 0)
                return scheduleUpdateResult;

            var userInfo = PackageController.GetPackageOwner(schedule.ScheduleInfo.PackageId);
            var actor = SecurityContext.User;

            int ownerUserId;
            int effectiveUserId;

            if (userInfo != null)
            {
                ownerUserId = userInfo.OwnerId == 0 ? userInfo.UserId : userInfo.OwnerId;
                effectiveUserId = userInfo.UserId;
            }
            else
            {
                ownerUserId = actor != null && actor.OwnerId > 0 ? actor.OwnerId : actor?.UserId ?? 0;
                effectiveUserId = actor?.UserId ?? ownerUserId;
            }

            var backgroundTask = new BackgroundTask(
                Guid.NewGuid(),
                Guid.NewGuid().ToString("N"),
                ownerUserId,
                effectiveUserId,
                "SCHEDULER",
                "RUN_SCHEDULE",
                schedule.ScheduleInfo.ScheduleName,
                schedule.ScheduleInfo.ScheduleId,
                schedule.ScheduleInfo.ScheduleId,
                schedule.ScheduleInfo.PackageId,
                schedule.ScheduleInfo.MaxExecutionTime, parameters)
                                     {
                                         Status = BackgroundTaskStatus.Starting
                                     };
            
            int createdTaskId = TaskController.AddTask(backgroundTask);
            TaskController.AddLog(new BackgroundTaskLogRecord(
                createdTaskId,
                0,
                false,
                String.Format("Scheduler task created; mode configured='{0}', effective='{1}', affinity='{2}', nodeHint='{3}', runToken='{4}', idempotencyKey='{5}'; waiting for worker startup.",
                    configuredExecutionMode,
                    dispatchPlan.EffectiveExecutionMode,
                    dispatchPlan.AffinityKey,
                    dispatchPlan.NodeHint,
                    runToken,
                    idempotencyKey),
                null,
                null));

            if (!String.IsNullOrWhiteSpace(placementNote))
            {
                TaskController.AddLog(new BackgroundTaskLogRecord(
                    createdTaskId,
                    0,
                    false,
                    placementNote,
                    null,
                    null));
            }

            if (!String.IsNullOrWhiteSpace(dispatchPlan.FallbackReason))
            {
                TaskController.AddLog(new BackgroundTaskLogRecord(
                    createdTaskId,
                    1,
                    false,
                    dispatchPlan.FallbackReason,
                    null,
                    null));
            }

                    if (!String.IsNullOrWhiteSpace(parallelismNote))
                    {
                    TaskController.AddLog(new BackgroundTaskLogRecord(
                        createdTaskId,
                        0,
                        false,
                        parallelismNote,
                        null,
                        null));
                    }

            if (userInfo == null)
            {
                TaskController.AddLog(new BackgroundTaskLogRecord(
                    createdTaskId,
                    1,
                    false,
                    "Package owner could not be resolved at schedule start. Falling back to current actor identity for task ownership metadata.",
                    null,
                    null));
            }

            if (bypassQueue)
            {
                var workerThread = new Thread(() => RunScheduledTaskImmediately(backgroundTask))
                {
                    Priority = ThreadPriority.Highest,
                    IsBackground = true
                };

                TaskManager.AddTaskThread(createdTaskId, workerThread);
                workerThread.Start();
            }

            return 0;
        }        

        private SchedulerDispatchPlan BuildDispatchPlan(SchedulerJob schedule, string configuredMode)
        {
            string normalizedMode = NormalizeExecutionMode(configuredMode);
            int? packageServerId = ResolvePackageServerId(schedule?.ScheduleInfo?.PackageId ?? 0);
            string requestedAffinity = ResolveRequestedAffinity(schedule?.ScheduleInfo?.Parameters, packageServerId);

            var plan = new SchedulerDispatchPlan
            {
                EffectiveExecutionMode = normalizedMode,
                AffinityKey = requestedAffinity,
                NodeHint = "enterprise:" + SchedulerRuntime.GetLeaseOwner(),
                TargetServerId = packageServerId
            };

            if (String.Equals(normalizedMode, ExecutionModeEnterpriseOnly, StringComparison.OrdinalIgnoreCase))
            {
                plan.AffinityKey = plan.NodeHint;
                plan.TargetServerId = null;
                return plan;
            }

            if (String.Equals(normalizedMode, ExecutionModeServerPreferred, StringComparison.OrdinalIgnoreCase))
            {
                if (packageServerId.HasValue)
                {
                    plan.AffinityKey = "server:" + packageServerId.Value.ToString(CultureInfo.InvariantCulture);
                    plan.NodeHint = plan.AffinityKey;
                    return plan;
                }

                plan.EffectiveExecutionMode = ExecutionModeAuto;
                plan.FallbackReason = "Execution mode 'SERVER_PREFERRED' requested, but the schedule package is not bound to a server. Falling back to AUTO dispatch.";
            }

            return plan;
        }

        private static void UpsertBackgroundTaskParameter(List<BackgroundTaskParameter> parameters, string name, string value)
        {
            if (parameters == null || String.IsNullOrWhiteSpace(name))
                return;

            for (int index = parameters.Count - 1; index >= 0; index--)
            {
                BackgroundTaskParameter existing = parameters[index];
                if (existing == null || String.IsNullOrWhiteSpace(existing.Name))
                    continue;

                if (String.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase))
                    parameters.RemoveAt(index);
            }

            parameters.Add(new BackgroundTaskParameter(name, value ?? String.Empty));
        }

        private static string ResolveRequestedAffinity(ScheduleTaskParameterInfo[] parameters, int? packageServerId)
        {
            if (parameters != null)
            {
                foreach (string parameterId in SchedulerAffinityParameterIds)
                {
                    string value = GetScheduleParameterValue(parameters, new[] { parameterId });
                    if (String.IsNullOrWhiteSpace(value))
                        continue;

                    string trimmed = value.Trim();
                    if (String.Equals(parameterId, "SERVER_ID", StringComparison.OrdinalIgnoreCase)
                        && Int32.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int explicitServerId)
                        && explicitServerId > 0)
                    {
                        return "server:" + explicitServerId.ToString(CultureInfo.InvariantCulture);
                    }

                    return trimmed;
                }
            }

            if (packageServerId.HasValue)
                return "server:" + packageServerId.Value.ToString(CultureInfo.InvariantCulture);

            return "global";
        }

        private static bool HasScheduleParameter(ScheduleTaskParameterInfo[] parameters, IEnumerable<string> parameterIds)
        {
            if (parameters == null || parameterIds == null)
                return false;

            foreach (string parameterId in parameterIds)
            {
                if (String.IsNullOrWhiteSpace(parameterId))
                    continue;

                foreach (ScheduleTaskParameterInfo parameter in parameters)
                {
                    if (parameter == null || String.IsNullOrWhiteSpace(parameter.ParameterId))
                        continue;

                    if (String.Equals(parameter.ParameterId, parameterId, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        private static string ResolveDispatchExecutionMode(SchedulerJob schedule, string configuredMode, bool hasExplicitExecutionMode, out string placementNote)
        {
            placementNote = null;

            if (hasExplicitExecutionMode && !String.Equals(configuredMode, ExecutionModeAuto, StringComparison.OrdinalIgnoreCase))
                return configuredMode;

            SchedulerPlacementMode recommendation = SchedulerTaskPlacementAdvisor.GetRecommendedMode(schedule?.Task?.TaskType, schedule?.Task?.TaskId);
            string recommendedMode = configuredMode;
            switch (recommendation)
            {
                case SchedulerPlacementMode.ServerPreferred:
                    recommendedMode = ExecutionModeServerPreferred;
                    break;
                case SchedulerPlacementMode.EnterpriseOnly:
                    recommendedMode = ExecutionModeEnterpriseOnly;
                    break;
                default:
                    recommendedMode = ExecutionModeAuto;
                    break;
            }

            if (!String.Equals(recommendedMode, configuredMode, StringComparison.OrdinalIgnoreCase))
            {
                placementNote = String.Format(
                    CultureInfo.InvariantCulture,
                    "Scheduler placement policy applied for task '{0}' ({1}): mode '{2}' -> '{3}'.",
                    schedule?.Task?.TaskId ?? "unknown",
                    schedule?.Task?.TaskType ?? "unknown",
                    configuredMode,
                    recommendedMode);
            }

            return recommendedMode;
        }

        private static string ApplyAdaptiveParallelismParameters(SchedulerJob schedule, List<BackgroundTaskParameter> runtimeParameters)
        {
            SchedulerParallelismRecommendation recommendation = SchedulerTaskParallelismAdvisor.GetRecommendation(schedule?.Task?.TaskId, schedule?.Task?.TaskType);
            if (recommendation == null)
                return null;

            string configuredMode = NormalizeParallelismMode(GetScheduleParameterValue(schedule?.ScheduleInfo?.Parameters, SchedulerParallelismModeParameterIds));
            string explicitTaskParallelism = GetScheduleParameterValue(schedule?.ScheduleInfo?.Parameters, recommendation.TargetParameterIds);
            if (!String.IsNullOrWhiteSpace(explicitTaskParallelism))
            {
                UpsertBackgroundTaskParameter(runtimeParameters, SchedulerParallelismEffectiveParameterId, explicitTaskParallelism.Trim());
                UpsertBackgroundTaskParameter(runtimeParameters, SchedulerParallelismSourceParameterId, "EXPLICIT_TASK_PARAMETER");

                return String.Format(
                    CultureInfo.InvariantCulture,
                    "Scheduler parallelism kept explicit task parameter '{0}'='{1}' for task '{2}' ({3}).",
                    recommendation.PrimaryParameterId,
                    explicitTaskParallelism.Trim(),
                    schedule?.Task?.TaskId ?? "unknown",
                    schedule?.Task?.TaskType ?? "unknown");
            }

            if (String.Equals(configuredMode, ParallelismModeAuto, StringComparison.OrdinalIgnoreCase)
                && !Web.Services.Configuration.SchedulerAutoTuneEnabled)
            {
                return null;
            }

            int effectiveParallelism;
            string source;

            if (String.Equals(configuredMode, ParallelismModeManual, StringComparison.OrdinalIgnoreCase))
            {
                string configuredMax = GetScheduleParameterValue(schedule?.ScheduleInfo?.Parameters, SchedulerParallelismMaxParameterIds);
                if (!Int32.TryParse(configuredMax, NumberStyles.Integer, CultureInfo.InvariantCulture, out effectiveParallelism) || effectiveParallelism <= 0)
                {
                    effectiveParallelism = recommendation.RecommendedValue;
                    source = "AUTO_FALLBACK_INVALID_MANUAL";
                }
                else
                {
                    effectiveParallelism = Math.Min(100, effectiveParallelism);
                    source = "MANUAL";
                }
            }
            else
            {
                effectiveParallelism = recommendation.RecommendedValue;
                source = "AUTO";
            }

            UpsertBackgroundTaskParameter(runtimeParameters, recommendation.PrimaryParameterId, effectiveParallelism.ToString(CultureInfo.InvariantCulture));
            UpsertBackgroundTaskParameter(runtimeParameters, SchedulerParallelismEffectiveParameterId, effectiveParallelism.ToString(CultureInfo.InvariantCulture));
            UpsertBackgroundTaskParameter(runtimeParameters, SchedulerParallelismSourceParameterId, source);

            return String.Format(
                CultureInfo.InvariantCulture,
                "Scheduler parallelism applied for task '{0}' ({1}): '{2}' mode set '{3}'={4}.",
                schedule?.Task?.TaskId ?? "unknown",
                schedule?.Task?.TaskType ?? "unknown",
                configuredMode,
                recommendation.PrimaryParameterId,
                effectiveParallelism);
        }

        private int? ResolvePackageServerId(int packageId)
        {
            if (packageId <= 0)
                return null;

            PackageInfo package = PackageController.GetPackage(packageId);
            if (package == null || package.ServerId <= 0)
                return null;

            return package.ServerId;
        }

        private static string GetScheduleParameterValue(ScheduleTaskParameterInfo[] parameters, IEnumerable<string> parameterIds)
        {
            if (parameters == null)
                return String.Empty;

            foreach (string parameterId in parameterIds)
            {
                foreach (ScheduleTaskParameterInfo parameter in parameters)
                {
                    if (parameter == null || String.IsNullOrWhiteSpace(parameter.ParameterId))
                        continue;

                    if (String.Equals(parameter.ParameterId, parameterId, StringComparison.OrdinalIgnoreCase))
                        return parameter.ParameterValue ?? String.Empty;
                }
            }

            return String.Empty;
        }

        private static string NormalizeExecutionMode(string mode)
        {
            string candidate = (mode ?? String.Empty).Trim();
            if (String.Equals(candidate, ExecutionModeServerPreferred, StringComparison.OrdinalIgnoreCase))
                return ExecutionModeServerPreferred;

            if (String.Equals(candidate, ExecutionModeEnterpriseOnly, StringComparison.OrdinalIgnoreCase))
                return ExecutionModeEnterpriseOnly;

            return ExecutionModeAuto;
        }

        private static string NormalizeParallelismMode(string mode)
        {
            string candidate = (mode ?? String.Empty).Trim();
            if (String.Equals(candidate, ParallelismModeManual, StringComparison.OrdinalIgnoreCase))
                return ParallelismModeManual;

            return ParallelismModeAuto;
        }

        private void RunScheduledTaskImmediately(BackgroundTask backgroundTask)
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

            var schedule = GetScheduleComplete(backgroundTask.ScheduleId);

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

        public int StopSchedule(int scheduleId)
        {
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);

            if (accountCheck < 0)
                return accountCheck;
            
            SchedulerJob schedule = GetScheduleComplete(scheduleId);
            if (schedule == null)
                return 0;

            ReleaseScheduleLease(scheduleId, SchedulerRuntime.GetLeaseOwner());

            if (SchedulerExecutionQueue.TryCancel(scheduleId))
                return 0;

            foreach (BackgroundTask task in TaskController.GetScheduleTasks(scheduleId))
            {
                task.Status = BackgroundTaskStatus.Stopping;
                
                TaskController.UpdateTask(task);
            }
            
            return 0;

        }

                    internal bool TryAcquireScheduleLease(int scheduleId, string owner, string runToken, TimeSpan leaseDuration, out SchedulerLeaseState lease)
                    {
                        lease = null;

                        if (scheduleId <= 0 || string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(runToken))
                            return false;

                        var nowUtc = DateTime.UtcNow;
                        var leaseUntilUtc = nowUtc.Add(leaseDuration);
                        var scheduleKey = scheduleId.ToString(CultureInfo.InvariantCulture);

                        using (var transaction = Database.Database.BeginTransaction())
                        {
                            var setting = Database.SystemSettings.FirstOrDefault(s => s.SettingsName == SchedulerLeaseSettingsName && s.PropertyName == scheduleKey);
                            var current = SchedulerLeaseState.Parse(scheduleId, setting?.PropertyValue);

                            if (current != null && !current.IsExpired(nowUtc) && !current.IsOwnedBy(owner, runToken))
                                return false;

                            var nextLease = new SchedulerLeaseState(scheduleId, owner, runToken, nowUtc, leaseUntilUtc);

                            if (setting == null)
                            {
                                Database.SystemSettings.Add(new SystemSetting
                                {
                                    SettingsName = SchedulerLeaseSettingsName,
                                    PropertyName = scheduleKey,
                                    PropertyValue = nextLease.Serialize()
                                });
                            }
                            else
                            {
                                setting.PropertyValue = nextLease.Serialize();
                            }

                            Database.SaveChanges();
                            transaction.Commit();
                            lease = nextLease;
                            return true;
                        }
                    }

                    internal bool RenewScheduleLease(int scheduleId, string owner, string runToken, TimeSpan leaseDuration)
                    {
                        if (scheduleId <= 0 || string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(runToken))
                            return false;

                        var nowUtc = DateTime.UtcNow;
                        var leaseUntilUtc = nowUtc.Add(leaseDuration);
                        var scheduleKey = scheduleId.ToString(CultureInfo.InvariantCulture);

                        using (var transaction = Database.Database.BeginTransaction())
                        {
                            var setting = Database.SystemSettings.FirstOrDefault(s => s.SettingsName == SchedulerLeaseSettingsName && s.PropertyName == scheduleKey);
                            var current = SchedulerLeaseState.Parse(scheduleId, setting?.PropertyValue);

                            if (current == null || !current.IsOwnedBy(owner, runToken))
                                return false;

                            var nextLease = new SchedulerLeaseState(scheduleId, owner, runToken, nowUtc, leaseUntilUtc);
                            setting.PropertyValue = nextLease.Serialize();

                            Database.SaveChanges();
                            transaction.Commit();
                            return true;
                        }
                    }

                    internal void ReleaseScheduleLease(int scheduleId, string owner, string runToken = null)
                    {
                        if (scheduleId <= 0 || string.IsNullOrWhiteSpace(owner))
                            return;

                        var scheduleKey = scheduleId.ToString(CultureInfo.InvariantCulture);

                        using (var transaction = Database.Database.BeginTransaction())
                        {
                            var setting = Database.SystemSettings.FirstOrDefault(s => s.SettingsName == SchedulerLeaseSettingsName && s.PropertyName == scheduleKey);
                            if (setting == null)
                                return;

                            var current = SchedulerLeaseState.Parse(scheduleId, setting.PropertyValue);
                            if (current != null && current.IsOwnedBy(owner, runToken ?? current.RunToken))
                            {
                                Database.SystemSettings.Remove(setting);
                                Database.SaveChanges();
                                transaction.Commit();
                            }
                        }
                    }

                    internal bool ReleaseExpiredScheduleLease(int scheduleId)
                    {
                        if (scheduleId <= 0)
                            return false;

                        string scheduleKey = scheduleId.ToString(CultureInfo.InvariantCulture);
                        DateTime nowUtc = DateTime.UtcNow;

                        using (var transaction = Database.Database.BeginTransaction())
                        {
                            var setting = Database.SystemSettings.FirstOrDefault(s => s.SettingsName == SchedulerLeaseSettingsName && s.PropertyName == scheduleKey);
                            if (setting == null)
                                return false;

                            SchedulerLeaseState current = SchedulerLeaseState.Parse(scheduleId, setting.PropertyValue);
                            if (current != null && !current.IsExpired(nowUtc))
                                return false;

                            Database.SystemSettings.Remove(setting);
                            Database.SaveChanges();
                            transaction.Commit();
                            return true;
                        }
                    }

        public void CalculateNextStartTime(ScheduleInfo schedule)
        {
            if (SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive) < 0) return;
            if (schedule.ScheduleType == ScheduleType.OneTime)
            {
                // start time stay intact
                // we only disable this task for the next time
                schedule.NextRun = schedule.StartTime;
            }
            else if (schedule.ScheduleType == ScheduleType.Interval)
            {
                DateTime lastRun = schedule.LastRun;
                DateTime now = DateTime.Now;

                // the task is running first time by default
                DateTime nextStart = DateTime.Now;

                if (lastRun != DateTime.MinValue)
                {
                    // the task is running next times
                    nextStart = lastRun.AddSeconds(schedule.Interval);
                }

                if (nextStart < now)
                    nextStart = now; // run immediately

                // check if start time is in allowed interval
                DateTime fromTime = new DateTime(now.Year, now.Month, now.Day,
                    schedule.FromTime.Hour, schedule.FromTime.Minute, schedule.FromTime.Second);

                DateTime toTime = new DateTime(now.Year, now.Month, now.Day,
                    schedule.ToTime.Hour, schedule.ToTime.Minute, schedule.ToTime.Second);

                if (!(nextStart >= fromTime && nextStart <= toTime))
                {
                    // run task in the start of the interval, but only tomorrow
                    nextStart = fromTime.AddDays(1);
                }
                schedule.NextRun = nextStart;
            }
            else if (schedule.ScheduleType == ScheduleType.Daily)
            {
                DateTime now = DateTime.Now;
                DateTime startTime = schedule.StartTime;
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    startTime.Hour, startTime.Minute, startTime.Second);
                if (nextStart < now) // start time is in the past
                    nextStart = nextStart.AddDays(1); // run tomorrow
                schedule.NextRun = nextStart;
            }
            else if (schedule.ScheduleType == ScheduleType.Weekly)
            {
                DateTime now = DateTime.Now;
                DateTime startTime = schedule.StartTime;
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    startTime.Hour, startTime.Minute, startTime.Second);
                int todayWeekDay = (int)now.DayOfWeek;
                nextStart = nextStart.AddDays(schedule.WeekMonthDay - todayWeekDay);

                if (nextStart < now) // start time is in the past
                    nextStart = nextStart.AddDays(7); // run next week
                schedule.NextRun = nextStart;
            }
            else if (schedule.ScheduleType == ScheduleType.Monthly)
            {
                DateTime now = DateTime.Now;
                DateTime startTime = schedule.StartTime;
                DateTime nextStart = new DateTime(now.Year, now.Month, now.Day,
                    startTime.Hour, startTime.Minute, startTime.Second);
                int todayDay = now.Day;
                nextStart = nextStart.AddDays(schedule.WeekMonthDay - todayDay);

                if (nextStart < now) // start time is in the past
                    nextStart = nextStart.AddMonths(1); // run next month
                schedule.NextRun = nextStart;
            }
        }

        public int AddSchedule(ScheduleInfo schedule)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // check quota
            if (PackageController.GetPackageQuota(schedule.PackageId, Quotas.OS_SCHEDULEDTASKS).QuotaExhausted)
                return BusinessErrorCodes.ERROR_OS_SCHEDULED_TASK_QUOTA_LIMIT;

            CalculateNextStartTime(schedule);
            EnforceExecutionModeParameterAccess(schedule, false);
            int approvalCheck = EnforceHighRiskApprovalPolicy(schedule, false);
            if (approvalCheck < 0)
                return approvalCheck;

            string xmlParameters = BuildParametersXml(schedule.Parameters);

            int scheduleId = Database.AddSchedule(SecurityContext.User.UserId,
                schedule.TaskId, schedule.PackageId, schedule.ScheduleName, schedule.ScheduleTypeId,
                schedule.Interval, schedule.FromTime, schedule.ToTime, schedule.StartTime,
                schedule.NextRun, schedule.Enabled, schedule.PriorityId,
                schedule.HistoriesNumber, schedule.MaxExecutionTime, schedule.WeekMonthDay, xmlParameters);

            // re-schedule tasks
            //Scheduler.ScheduleTasks();

            return scheduleId;
        }

        public int UpdateSchedule(ScheduleInfo schedule)
        {
            // check account
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive);
            if (accountCheck < 0) return accountCheck;

            // load original schedule to preserve server-managed fields
            ScheduleInfo original = GetScheduleInternal(schedule.ScheduleId);
            schedule.LastRun = original?.LastRun ?? schedule.LastRun;
            CalculateNextStartTime(schedule);
            EnforceExecutionModeParameterAccess(schedule, true);
            int approvalCheck = EnforceHighRiskApprovalPolicy(schedule, true);
            if (approvalCheck < 0)
                return approvalCheck;

            string xmlParameters = BuildParametersXml(schedule.Parameters);

            Database.UpdateSchedule(SecurityContext.User.UserId,
                schedule.ScheduleId, schedule.TaskId, schedule.ScheduleName, schedule.ScheduleTypeId,
                schedule.Interval, schedule.FromTime, schedule.ToTime, schedule.StartTime,
                schedule.LastRun, schedule.NextRun, schedule.Enabled, schedule.PriorityId,
                schedule.HistoriesNumber, schedule.MaxExecutionTime, schedule.WeekMonthDay, xmlParameters);

            // re-schedule tasks
            //Scheduler.ScheduleTasks();

            return 0;
        }

        private string BuildParametersXml(ScheduleTaskParameterInfo[] parameters)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement nodeProps = doc.CreateElement("parameters");
            if (parameters != null)
            {
                foreach (ScheduleTaskParameterInfo parameter in parameters)
                {
                    XmlElement nodeProp = doc.CreateElement("parameter");
                    nodeProp.SetAttribute("id", parameter.ParameterId);
                    nodeProp.SetAttribute("value", parameter.ParameterValue);
                    nodeProps.AppendChild(nodeProp);
                }
            }
            return nodeProps.OuterXml;
        }

        private void EnforceExecutionModeParameterAccess(ScheduleInfo schedule, bool isUpdate)
        {
            if (schedule == null)
                return;

            List<ScheduleTaskParameterInfo> parameters = new List<ScheduleTaskParameterInfo>(schedule.Parameters ?? Array.Empty<ScheduleTaskParameterInfo>());
            string incomingMode = NormalizeExecutionMode(GetScheduleParameterValue(schedule.Parameters, SchedulerExecutionModeParameterIds));

            if (SecurityContext.User.IsInRole(SecurityContext.ROLE_ADMINISTRATOR))
            {
                UpsertExecutionModeParameter(parameters, incomingMode);
                schedule.Parameters = parameters.ToArray();
                return;
            }

            string preservedMode = ExecutionModeAuto;
            if (isUpdate && schedule.ScheduleId > 0)
            {
                SchedulerJob existingSchedule = GetScheduleComplete(schedule.ScheduleId);
                if (existingSchedule != null && existingSchedule.ScheduleInfo != null)
                {
                    preservedMode = NormalizeExecutionMode(GetScheduleParameterValue(existingSchedule.ScheduleInfo.Parameters, SchedulerExecutionModeParameterIds));
                }
            }

            UpsertExecutionModeParameter(parameters, preservedMode);
            schedule.Parameters = parameters.ToArray();
        }

        private static void UpsertExecutionModeParameter(List<ScheduleTaskParameterInfo> parameters, string mode)
        {
            if (parameters == null)
                return;

            for (int index = parameters.Count - 1; index >= 0; index--)
            {
                ScheduleTaskParameterInfo parameter = parameters[index];
                if (parameter == null || String.IsNullOrWhiteSpace(parameter.ParameterId))
                    continue;

                foreach (string alias in SchedulerExecutionModeParameterIds)
                {
                    if (String.Equals(parameter.ParameterId, alias, StringComparison.OrdinalIgnoreCase))
                    {
                        parameters.RemoveAt(index);
                        break;
                    }
                }
            }

            parameters.Add(new ScheduleTaskParameterInfo
            {
                ParameterId = SchedulerExecutionModeParameterIds[0],
                ParameterValue = NormalizeExecutionMode(mode)
            });
        }

        private int EnforceHighRiskApprovalPolicy(ScheduleInfo schedule, bool isUpdate)
        {
            if (schedule == null)
                return 0;

            bool isHighRisk = IsHighRiskSchedule(schedule);
            if (!isHighRisk)
                return 0;

            bool isAdmin = SecurityContext.User.IsInRole(SecurityContext.ROLE_ADMINISTRATOR);
            if (!isAdmin)
                return BusinessErrorCodes.ERROR_USER_ACCOUNT_NOT_ENOUGH_PERMISSIONS;

            List<ScheduleTaskParameterInfo> parameters = new List<ScheduleTaskParameterInfo>(schedule.Parameters ?? Array.Empty<ScheduleTaskParameterInfo>());

            ScheduleInfo existing = null;
            if (isUpdate && schedule.ScheduleId > 0)
            {
                SchedulerJob existingSchedule = GetScheduleComplete(schedule.ScheduleId);
                existing = existingSchedule?.ScheduleInfo;
            }

            int actorUserId = SecurityContext.User.UserId;
            int firstApprover = ResolveIntParameter(parameters, existing?.Parameters, SchedulerFirstApproverParameterId);
            int secondApprover = ResolveIntParameter(parameters, existing?.Parameters, SchedulerSecondApproverParameterId);
            string approvedAt = ResolveStringParameter(parameters, existing?.Parameters, SchedulerApprovedAtParameterId);

            if (firstApprover <= 0)
            {
                firstApprover = actorUserId;
            }
            else if (secondApprover <= 0 && actorUserId > 0 && actorUserId != firstApprover)
            {
                secondApprover = actorUserId;
                if (String.IsNullOrWhiteSpace(approvedAt))
                    approvedAt = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            }

            bool approved = firstApprover > 0 && secondApprover > 0 && firstApprover != secondApprover;
            if (!approved)
            {
                schedule.Enabled = false;
            }

            UpsertScheduleParameter(parameters, SchedulerRiskLevelParameterId, "HIGH");
            UpsertScheduleParameter(parameters, SchedulerFirstApproverParameterId, firstApprover > 0 ? firstApprover.ToString(CultureInfo.InvariantCulture) : String.Empty);
            UpsertScheduleParameter(parameters, SchedulerSecondApproverParameterId, secondApprover > 0 ? secondApprover.ToString(CultureInfo.InvariantCulture) : String.Empty);
            UpsertScheduleParameter(parameters, SchedulerApprovalStateParameterId, approved ? ApprovalStateApproved : ApprovalStatePendingSecond);
            UpsertScheduleParameter(parameters, SchedulerApprovedParameterId, approved ? "true" : "false");

            string approvedBy = approved
                ? String.Format(CultureInfo.InvariantCulture, "{0},{1}", firstApprover, secondApprover)
                : (firstApprover > 0 ? firstApprover.ToString(CultureInfo.InvariantCulture) : String.Empty);
            UpsertScheduleParameter(parameters, SchedulerApprovedByParameterId, approvedBy);
            UpsertScheduleParameter(parameters, SchedulerApprovedAtParameterId, approved ? approvedAt : String.Empty);
            schedule.Parameters = parameters.ToArray();

            return 0;
        }

        private static bool IsHighRiskExecutionApproved(ScheduleInfo schedule)
        {
            if (schedule == null)
                return false;

            string approved = GetScheduleParameterValue(schedule.Parameters, new[] { SchedulerApprovedParameterId });
            if (!String.Equals((approved ?? String.Empty).Trim(), "true", StringComparison.OrdinalIgnoreCase))
                return false;

            int firstApprover = ResolveIntParameter(schedule.Parameters, SchedulerFirstApproverParameterId);
            int secondApprover = ResolveIntParameter(schedule.Parameters, SchedulerSecondApproverParameterId);
            if (firstApprover <= 0 || secondApprover <= 0 || firstApprover == secondApprover)
                return false;

            string state = GetScheduleParameterValue(schedule.Parameters, new[] { SchedulerApprovalStateParameterId });
            return String.Equals((state ?? String.Empty).Trim(), ApprovalStateApproved, StringComparison.OrdinalIgnoreCase);
        }

        private static int ResolveIntParameter(List<ScheduleTaskParameterInfo> incoming, ScheduleTaskParameterInfo[] existing, string parameterId)
        {
            string incomingValue = ResolveStringParameter(incoming, parameterId);
            if (Int32.TryParse(incomingValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int incomingParsed))
                return incomingParsed;

            string existingValue = GetScheduleParameterValue(existing, new[] { parameterId });
            if (Int32.TryParse(existingValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int existingParsed))
                return existingParsed;

            return 0;
        }

        private static int ResolveIntParameter(ScheduleTaskParameterInfo[] parameters, string parameterId)
        {
            string value = GetScheduleParameterValue(parameters, new[] { parameterId });
            return Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed) ? parsed : 0;
        }

        private static string ResolveStringParameter(List<ScheduleTaskParameterInfo> incoming, ScheduleTaskParameterInfo[] existing, string parameterId)
        {
            string incomingValue = ResolveStringParameter(incoming, parameterId);
            if (!String.IsNullOrWhiteSpace(incomingValue))
                return incomingValue;

            return GetScheduleParameterValue(existing, new[] { parameterId });
        }

        private static string ResolveStringParameter(List<ScheduleTaskParameterInfo> parameters, string parameterId)
        {
            if (parameters == null || String.IsNullOrWhiteSpace(parameterId))
                return String.Empty;

            foreach (ScheduleTaskParameterInfo parameter in parameters)
            {
                if (parameter == null || String.IsNullOrWhiteSpace(parameter.ParameterId))
                    continue;

                if (String.Equals(parameter.ParameterId, parameterId, StringComparison.OrdinalIgnoreCase))
                    return parameter.ParameterValue ?? String.Empty;
            }

            return String.Empty;
        }

        private static bool IsHighRiskSchedule(ScheduleInfo schedule)
        {
            if (schedule == null)
                return false;

            string riskLevel = GetScheduleParameterValue(schedule.Parameters, new[] { SchedulerRiskLevelParameterId });
            if (String.Equals((riskLevel ?? String.Empty).Trim(), "HIGH", StringComparison.OrdinalIgnoreCase))
                return true;

            string taskId = (schedule.TaskId ?? String.Empty).Trim();
            if (String.IsNullOrWhiteSpace(taskId))
                return false;

            foreach (string marker in HighRiskTaskIdMarkers)
            {
                if (taskId.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private static void UpsertScheduleParameter(List<ScheduleTaskParameterInfo> parameters, string parameterId, string parameterValue)
        {
            if (parameters == null || String.IsNullOrWhiteSpace(parameterId))
                return;

            for (int index = parameters.Count - 1; index >= 0; index--)
            {
                ScheduleTaskParameterInfo existing = parameters[index];
                if (existing == null || String.IsNullOrWhiteSpace(existing.ParameterId))
                    continue;

                if (String.Equals(existing.ParameterId, parameterId, StringComparison.OrdinalIgnoreCase))
                    parameters.RemoveAt(index);
            }

            parameters.Add(new ScheduleTaskParameterInfo
            {
                ParameterId = parameterId,
                ParameterValue = parameterValue ?? String.Empty
            });
        }

        public int DeleteSchedule(int scheduleId)
        {
            if (!SecurityContext.User.IsInRole(SecurityContext.ROLE_RESELLER) && !SecurityContext.User.IsInRole(SecurityContext.ROLE_ADMINISTRATOR)) return -1;
            int accountCheck = SecurityContext.CheckAccount(DemandAccount.NotDemo | DemandAccount.IsActive | DemandAccount.IsReseller);
            if (accountCheck < 0) return accountCheck;

            ScheduleInfo schedule = GetSchedule(scheduleId);
            if (schedule == null)
                return -1;

            if (schedule.PackageId > 0)
            {
                int packageCheck = SecurityContext.CheckPackage(schedule.PackageId, DemandPackage.IsActive);
                if (packageCheck < 0)
                    return packageCheck;
            }
            else if (!SecurityContext.User.IsInRole(SecurityContext.ROLE_ADMINISTRATOR))
            {
                return BusinessErrorCodes.ERROR_USER_ACCOUNT_NOT_ENOUGH_PERMISSIONS;
            }

            // stop schedule if active
            StopSchedule(scheduleId);

            // delete schedule
            Database.DeleteSchedule(SecurityContext.User.UserId, scheduleId);

            // re-schedule tasks
            //Scheduler.ScheduleTasks();

            return 0;
        }
    }
}
