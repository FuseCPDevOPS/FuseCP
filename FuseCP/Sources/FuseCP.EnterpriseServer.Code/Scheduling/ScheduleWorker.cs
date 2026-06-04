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

#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace FuseCP.EnterpriseServer.Code;


public class ScheduleWorker: BackgroundService
{
	public static bool Collect = false;

	public ILogger<ScheduleWorker> Log;

	public ScheduleWorker(ILogger<ScheduleWorker> logger)
	{
		Log = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		if (!Web.Services.Configuration.SchedulerEnabled) return;
		SchedulerExecutionQueue.ConfigureGlobalMaxConcurrentExecutions(Web.Services.Configuration.SchedulerGlobalMaxConcurrentExecutions);
		SchedulerExecutionQueue.ConfigureMaxConcurrentExecutions(Web.Services.Configuration.SchedulerMaxConcurrentExecutions);
		SchedulerAdaptiveTuner adaptiveTuner = null;
		if (Web.Services.Configuration.SchedulerAutoTuneEnabled)
		{
			adaptiveTuner = new SchedulerAdaptiveTuner(
				Log,
				Web.Services.Configuration.SchedulerMinConcurrentExecutions,
				Web.Services.Configuration.SchedulerMaxAutoConcurrentExecutions,
				Web.Services.Configuration.SchedulerAutoScaleUpCpuThresholdPercent,
				Web.Services.Configuration.SchedulerAutoScaleDownCpuThresholdPercent,
				Web.Services.Configuration.SchedulerAutoScaleDownMemoryThresholdPercent);
		}

		Log.LogInformation("Scheduler worker started...");
		Log.LogInformation(
			"Scheduler concurrency set: per-affinity max={PerAffinityMaxConcurrentExecutions}, global max={GlobalMaxConcurrentExecutions}",
			SchedulerExecutionQueue.MaxConcurrentExecutions,
			SchedulerExecutionQueue.MaxGlobalConcurrentExecutions);
		if (adaptiveTuner != null)
		{
			Log.LogInformation(
				"Scheduler autotune enabled (min={MinConcurrency}, max={MaxConcurrency}, scaleUpCpu<={ScaleUpCpu}%, scaleDownCpu>={ScaleDownCpu}%, scaleDownMem>={ScaleDownMem}%)",
				Web.Services.Configuration.SchedulerMinConcurrentExecutions,
				Web.Services.Configuration.SchedulerMaxAutoConcurrentExecutions,
				Web.Services.Configuration.SchedulerAutoScaleUpCpuThresholdPercent,
				Web.Services.Configuration.SchedulerAutoScaleDownCpuThresholdPercent,
				Web.Services.Configuration.SchedulerAutoScaleDownMemoryThresholdPercent);
		}

		await Task.Delay(5000, stoppingToken);

		int runs = 0;

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				adaptiveTuner?.TuneIfNeeded();

				using (var scheduler = new Scheduler())
				{
					scheduler.Start();
				}
				if (Collect && ++runs >= 10)
				{
					runs = 0;
					// Leave collection to runtime heuristics; explicit full collections hurt throughput.
				}
			}
			catch (Exception swallowedEx) when (!(swallowedEx is OutOfMemoryException) && !(swallowedEx is StackOverflowException) && !(swallowedEx is AccessViolationException))
			{
				Log.LogError(swallowedEx, "Scheduler worker loop failed");
				System.Diagnostics.Trace.TraceWarning("Exception swallowed: " + swallowedEx.Message);
			}

			await Task.Delay(5000, stoppingToken);
		}
	}
}
#endif


