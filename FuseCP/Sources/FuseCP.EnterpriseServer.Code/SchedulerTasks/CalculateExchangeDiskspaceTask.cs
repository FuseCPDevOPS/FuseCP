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
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FuseCP.Server.Client;
using FuseCP.Providers.HostedSolution;

namespace FuseCP.EnterpriseServer
{
	public class CalculateExchangeDiskspaceTask : SchedulerTask
    {
        private int maxParallelOrganizations = 1;

        public override void DoWork()
        {
            var topTask = TaskManager.TopTask;
            int suggestedParallelism = Math.Max(1, Math.Min(6, Environment.ProcessorCount / 2));
            maxParallelOrganizations = NormalizeInt(topTask.GetParamValue("MAX_PARALLEL_ORGANIZATIONS"), suggestedParallelism, 1, 24);
            CalculateDiskspace();
        }

        public void CalculateDiskspace()
        {
			// get all space organizations recursively
			List<Organization> items = ExchangeServerController.GetExchangeOrganizations(TaskManager.TopTask.PackageId, true);
            int successCount = 0;
            int failureCount = 0;
			TaskManager.Write("Exchange diskspace organization parallelism: {0}", maxParallelOrganizations.ToString());

            if (maxParallelOrganizations <= 1 || items.Count <= 1)
            {
				foreach (Organization item in items)
				{
                    try
                    {
                        ExchangeServerController.CalculateOrganizationDiskspaceInternal(item.Id);
                        successCount++;
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        failureCount++;
                        TaskManager.WriteError("Exchange diskspace error for organization '{0}': {1}", item.Id.ToString(), ex.ToString());
                    }
				}
            }
            else
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = maxParallelOrganizations };
                Parallel.ForEach(items, options, item =>
                {
                    try
                    {
                        ExchangeServerController.CalculateOrganizationDiskspaceInternal(item.Id);
                        Interlocked.Increment(ref successCount);
                    }
                    catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                    {
                        Interlocked.Increment(ref failureCount);
                        TaskManager.WriteError("Exchange diskspace error for organization '{0}': {1}", item.Id.ToString(), ex.ToString());
                    }
                });
            }

            TaskManager.Write("Exchange diskspace calculation finished. Total organizations: {0}, successful: {1}, failed: {2}",
                items.Count.ToString(), successCount.ToString(), failureCount.ToString());
        }

        private static int NormalizeInt(object rawValue, int defaultValue, int min, int max)
        {
            int parsed;
            if (!int.TryParse(Convert.ToString(rawValue), out parsed))
                parsed = defaultValue;

            if (parsed < min)
                parsed = min;
            if (parsed > max)
                parsed = max;

            return parsed;
        }
    }
}
