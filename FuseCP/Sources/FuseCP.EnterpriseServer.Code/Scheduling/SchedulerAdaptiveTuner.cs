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
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FuseCP.EnterpriseServer.Code;

internal sealed class SchedulerAdaptiveTuner
{
	private readonly ILogger _log;
	private readonly int _minConcurrency;
	private readonly int _maxConcurrency;
	private readonly int _scaleUpCpuThreshold;
	private readonly int _scaleDownCpuThreshold;
	private readonly int _scaleDownMemoryThreshold;
	private readonly Process _process;

	private DateTime _lastSampleAt;
	private TimeSpan _lastCpu;
	private DateTime _lastTuneAt;

	public SchedulerAdaptiveTuner(
		ILogger log,
		int minConcurrency,
		int maxConcurrency,
		int scaleUpCpuThreshold,
		int scaleDownCpuThreshold,
		int scaleDownMemoryThreshold)
	{
		_log = log;
		_minConcurrency = Math.Max(1, minConcurrency);
		_maxConcurrency = Math.Max(_minConcurrency, maxConcurrency);
		_scaleUpCpuThreshold = Math.Clamp(scaleUpCpuThreshold, 5, 95);
		_scaleDownCpuThreshold = Math.Clamp(scaleDownCpuThreshold, _scaleUpCpuThreshold + 5, 99);
		_scaleDownMemoryThreshold = Math.Clamp(scaleDownMemoryThreshold, 50, 99);
		_process = Process.GetCurrentProcess();

		_lastSampleAt = DateTime.UtcNow;
		_lastCpu = _process.TotalProcessorTime;
		_lastTuneAt = DateTime.UtcNow;
	}

	public void TuneIfNeeded()
	{
		DateTime now = DateTime.UtcNow;
		if ((now - _lastTuneAt).TotalSeconds < 15)
		{
			return;
		}

		double cpuPercent = GetProcessCpuPercent(now);
		double memoryLoadPercent = GetMemoryLoadPercent();

		int current = SchedulerExecutionQueue.MaxConcurrentExecutions;
		int queued = SchedulerExecutionQueue.QueuedExecutions;
		int active = SchedulerExecutionQueue.ActiveExecutions;
		int activeUnits = SchedulerExecutionQueue.ActiveExecutionUnits;
		int next = current;

		if (cpuPercent >= _scaleDownCpuThreshold || memoryLoadPercent >= _scaleDownMemoryThreshold)
		{
			next = Math.Max(_minConcurrency, current - 1);
		}
		else if (cpuPercent <= _scaleUpCpuThreshold && memoryLoadPercent < _scaleDownMemoryThreshold - 10 && queued > 0 && activeUnits >= Math.Max(1, current - 1))
		{
			next = Math.Min(_maxConcurrency, current + 1);
		}

		if (next != current)
		{
			SchedulerExecutionQueue.ConfigureMaxConcurrentExecutions(next);
			_log.LogInformation(
				"Scheduler autotune adjusted max concurrency from {Previous} to {Current} (cpu={CpuPercent:F1}%, mem={MemoryPercent:F1}%, queued={Queued}, active={Active}, activeUnits={ActiveUnits})",
				current,
				next,
				cpuPercent,
				memoryLoadPercent,
				queued,
				active,
				activeUnits);
		}

		_lastTuneAt = now;
	}

	private double GetProcessCpuPercent(DateTime now)
	{
		TimeSpan cpuNow = _process.TotalProcessorTime;
		double elapsedMs = Math.Max((now - _lastSampleAt).TotalMilliseconds, 1);
		double cpuMs = (cpuNow - _lastCpu).TotalMilliseconds;

		_lastSampleAt = now;
		_lastCpu = cpuNow;

		double normalized = cpuMs / (elapsedMs * Environment.ProcessorCount);
		return Math.Clamp(normalized * 100.0, 0, 100);
	}

	private static double GetMemoryLoadPercent()
	{
		var info = GC.GetGCMemoryInfo();
		if (info.HighMemoryLoadThresholdBytes <= 0)
		{
			return 0;
		}

		double ratio = (double)info.MemoryLoadBytes / info.HighMemoryLoadThresholdBytes;
		return Math.Clamp(ratio * 100.0, 0, 100);
	}
}
#endif
