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
using System.Collections.Concurrent;

namespace FuseCP.EnterpriseServer
{
    internal static class SchedulerTaskWeightAdvisor
    {
        private sealed class TaskRuntimeProfile
        {
            public double DurationSecondsEma;
            public int Samples;
        }

        private static readonly ConcurrentDictionary<string, TaskRuntimeProfile> Profiles =
            new ConcurrentDictionary<string, TaskRuntimeProfile>(StringComparer.OrdinalIgnoreCase);

        private const double Alpha = 0.25;
        private const int MinSamplesForRecommendation = 3;

        public static void RecordExecution(string taskType, TimeSpan elapsed)
        {
            if (string.IsNullOrWhiteSpace(taskType))
                return;

            double seconds = Math.Max(0.1, elapsed.TotalSeconds);
            Profiles.AddOrUpdate(
                taskType,
                _ => new TaskRuntimeProfile { DurationSecondsEma = seconds, Samples = 1 },
                (_, profile) =>
                {
                    profile.DurationSecondsEma = (profile.DurationSecondsEma * (1.0 - Alpha)) + (seconds * Alpha);
                    profile.Samples++;
                    return profile;
                });
        }

        public static int? GetRecommendedWeight(string taskType, int defaultWeight, int mediumWeight, int heavyWeight)
        {
            if (string.IsNullOrWhiteSpace(taskType))
                return null;

            if (!Profiles.TryGetValue(taskType, out TaskRuntimeProfile profile) || profile.Samples < MinSamplesForRecommendation)
                return null;

            double ema = profile.DurationSecondsEma;
            if (ema >= 900)
                return Math.Max(heavyWeight + 1, heavyWeight);
            if (ema >= 300)
                return heavyWeight;
            if (ema >= 90)
                return mediumWeight;

            return defaultWeight;
        }
    }
}
