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

using System;

namespace FuseCP.EnterpriseServer
{
    internal sealed class SchedulerParallelismRecommendation
    {
        public string PrimaryParameterId { get; set; }
        public string[] TargetParameterIds { get; set; }
        public int RecommendedValue { get; set; }
    }

    internal static class SchedulerTaskParallelismAdvisor
    {
        private static readonly string[] PackageParallelismParameterIds = { "MAX_PARALLEL_PACKAGES" };
        private static readonly string[] ExchangeParallelismParameterIds = { "MAX_PARALLEL_ORGANIZATIONS" };

        public static SchedulerParallelismRecommendation GetRecommendation(string taskId, string taskType)
        {
            string candidate = ((taskId ?? String.Empty) + "|" + (taskType ?? String.Empty)).ToUpperInvariant();
            int processorCount = Math.Max(1, Environment.ProcessorCount);
            int queueDepth = Math.Max(0, SchedulerExecutionQueue.QueuedExecutions);
            int activeUnits = Math.Max(0, SchedulerExecutionQueue.ActiveExecutionUnits);
            int concurrencyCap = Math.Max(1, SchedulerExecutionQueue.MaxConcurrentExecutions);

            if (candidate.Contains("CALCULATEPACKAGESDISKSPACE") || candidate.Contains("CALCULATEPACKAGESBANDWIDTH"))
            {
                int recommended = ComputeRecommendedParallelism(
                    baseSuggested: Math.Max(1, Math.Min(8, processorCount / 2)),
                    queueDepth: queueDepth,
                    activeUnits: activeUnits,
                    concurrencyCap: concurrencyCap,
                    min: 1,
                    max: 32);

                return new SchedulerParallelismRecommendation
                {
                    PrimaryParameterId = PackageParallelismParameterIds[0],
                    TargetParameterIds = PackageParallelismParameterIds,
                    RecommendedValue = recommended
                };
            }

            if (candidate.Contains("CALCULATEEXCHANGEDISKSPACE"))
            {
                int recommended = ComputeRecommendedParallelism(
                    baseSuggested: Math.Max(1, Math.Min(6, processorCount / 2)),
                    queueDepth: queueDepth,
                    activeUnits: activeUnits,
                    concurrencyCap: concurrencyCap,
                    min: 1,
                    max: 24);

                return new SchedulerParallelismRecommendation
                {
                    PrimaryParameterId = ExchangeParallelismParameterIds[0],
                    TargetParameterIds = ExchangeParallelismParameterIds,
                    RecommendedValue = recommended
                };
            }

            return null;
        }

        private static int ComputeRecommendedParallelism(int baseSuggested, int queueDepth, int activeUnits, int concurrencyCap, int min, int max)
        {
            int next = Math.Max(min, Math.Min(max, baseSuggested));

            // Reduce per-task fan-out when queue pressure is high so more schedules can make progress.
            if (queueDepth >= Math.Max(4, concurrencyCap * 2))
            {
                next = Math.Max(min, next - 2);
            }
            else if (queueDepth > 0)
            {
                next = Math.Max(min, next - 1);
            }

            // Allow a gentle bump when the queue is empty and runtime is mostly idle.
            if (queueDepth == 0 && activeUnits <= 1)
            {
                next = Math.Min(max, next + 1);
            }

            // Keep per-task parallelism bounded relative to scheduler execution width.
            int widthBound = Math.Max(2, concurrencyCap * 3);
            next = Math.Min(next, widthBound);

            return Math.Max(min, Math.Min(max, next));
        }
    }
}
