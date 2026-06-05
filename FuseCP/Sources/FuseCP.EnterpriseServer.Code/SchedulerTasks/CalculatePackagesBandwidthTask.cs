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
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using FuseCP.Providers;
using FuseCP.Server.Client;
using System.Linq;

namespace FuseCP.EnterpriseServer
{
    public class CalculatePackagesBandwidthTask : SchedulerTask
    {
        private const int ProgressLogInterval = 25;
        private readonly bool suspendOverused = false;
        private int serviceCallAttempts = 3;
        private int serviceRetryBaseDelayMs = 250;
        private int maxParallelPackages = 1;
        private int totalServiceTimeoutFailures;
        private int totalServiceErrorFailures;

        public override void DoWork()
        {
            // Input parameters:
            //  - SUSPEND_OVERUSED_PACKAGES
            //  - SERVICE_CALL_ATTEMPTS
            //  - SERVICE_RETRY_DELAY_MS
            //  - MAX_PARALLEL_PACKAGES

            var topTask = TaskManager.TopTask;
            serviceCallAttempts = NormalizeInt(topTask.GetParamValue("SERVICE_CALL_ATTEMPTS"), 3, 1, 8);
            serviceRetryBaseDelayMs = NormalizeInt(topTask.GetParamValue("SERVICE_RETRY_DELAY_MS"), 250, 0, 5000);
            int suggestedParallelism = Math.Max(1, Math.Min(8, Environment.ProcessorCount / 2));
            maxParallelPackages = NormalizeInt(topTask.GetParamValue("MAX_PARALLEL_PACKAGES"), suggestedParallelism, 1, 32);
            totalServiceTimeoutFailures = 0;
            totalServiceErrorFailures = 0;

            CalculateBandwidth();
        }

        public void CalculateBandwidth()
        {
            // get all owned packages
            List<PackageInfo> packages = PackageController.GetPackagePackages(TaskManager.TopTask.PackageId, true);
            TaskManager.Write("Packages to calculate: " + packages.Count);
            TaskManager.Write("Bandwidth package parallelism: {0}", maxParallelPackages.ToString());

            int packageSuccessCount = 0;
            int packageFailureCount = 0;
            int processedCount = 0;

            if (maxParallelPackages <= 1 || packages.Count <= 1)
            {
                foreach (PackageInfo package in packages)
                {
                    // calculating package bandwidth
                    if (CalculatePackage(package.PackageId))
                        packageSuccessCount++;
                    else
                        packageFailureCount++;

                    processedCount++;
                    if (processedCount % ProgressLogInterval == 0)
                    {
                        TaskManager.Write("Bandwidth progress: processed {0}/{1} packages", processedCount.ToString(), packages.Count.ToString());
                    }
                }
            }
            else
            {
                var options = new ParallelOptions { MaxDegreeOfParallelism = maxParallelPackages };
                Parallel.ForEach(packages, options, package =>
                {
                    bool success = CalculatePackage(package.PackageId);
                    if (success)
                        Interlocked.Increment(ref packageSuccessCount);
                    else
                        Interlocked.Increment(ref packageFailureCount);

                    int processed = Interlocked.Increment(ref processedCount);
                    if (processed % ProgressLogInterval == 0 || processed == packages.Count)
                    {
                        TaskManager.Write("Bandwidth progress: processed {0}/{1} packages", processed.ToString(), packages.Count.ToString());
                    }
                });
            }

            TaskManager.Write("Bandwidth calculation finished. Total packages: {0}, successful: {1}, failed: {2}",
                packages.Count.ToString(), packageSuccessCount.ToString(), packageFailureCount.ToString());
            TaskManager.Write("Bandwidth service call failures. Timeout: {0}, Other errors: {1}",
                totalServiceTimeoutFailures.ToString(), totalServiceErrorFailures.ToString());
        }

        public bool CalculatePackage(int packageId)
        {
            DateTime since = PackageController.GetPackageBandwidthUpdate(packageId);
            DateTime nextUpdate = DateTime.Now;

            try
            {
                // get all package items
                List<ServiceProviderItem> items = PackageController.GetServiceItemsForStatistics(
                    0, packageId, false, true, false, false);

                // order items by service
                Dictionary<int, List<ServiceProviderItem>> orderedItems =
                    PackageController.OrderServiceItemsByServices(items);

                // calculate statistics for each service set
                List<ServiceProviderItemBandwidth> itemsBandwidth = new List<ServiceProviderItemBandwidth>(items.Count);
                int serviceFailures = 0;
                foreach (int serviceId in orderedItems.Keys)
                {
                    ServiceProviderItemBandwidth[] serviceBandwidth = CalculateItems(serviceId, orderedItems[serviceId], since);
                    if (serviceBandwidth == null)
                    {
                        serviceFailures++;
                        continue;
                    }

                    itemsBandwidth.AddRange(serviceBandwidth.Where(bw => bw != null));
                }

                if (serviceFailures > 0)
                {
                    TaskManager.WriteError("Bandwidth partial result for package '{0}': {1} service(s) failed and were skipped",
                        packageId.ToString(), serviceFailures.ToString());
                }

                // update info in the database
                string xml = BuildDiskBandwidthStatisticsXml(itemsBandwidth.ToArray());
                PackageController.UpdatePackageBandwidth(packageId, xml);

                // advance update timestamp only when all services succeeded.
                if (serviceFailures == 0)
                    PackageController.UpdatePackageBandwidthUpdate(packageId, nextUpdate);

                // suspend package if requested
                if (suspendOverused)
                {
                    // disk space
                    QuotaValueInfo dsQuota = PackageController.GetPackageQuota(packageId, Quotas.OS_BANDWIDTH);

                    if (dsQuota.QuotaExhausted)
                        PackageController.ChangePackageStatus(null, packageId, PackageStatus.Suspended, false);
                }

                return serviceFailures == 0;
            }
            catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
            {
                // load package details
                PackageInfo package = PackageController.GetPackage(packageId);

                // load user details
                UserInfo user = PackageController.GetPackageOwner(package.PackageId);

                // log error
                TaskManager.WriteError(String.Format("Error calculating bandwidth for '{0}' space of user '{1}': {2}",
                    package.PackageName, user.Username, ex));

                return false;
            }
        }

        public ServiceProviderItemBandwidth[] CalculateItems(int serviceId, List<ServiceProviderItem> items,
            DateTime since)
        {
            // convert items to SoapObjects
            var objItems = items.Select(SoapServiceProviderItem.Wrap).ToList();

            if (objItems.Count == 0)
                return Array.Empty<ServiceProviderItemBandwidth>();

            var retry = SchedulerTaskReliability.ExecuteWithRetry(
                () =>
                {
                    ServiceProvider prov = new ServiceProvider();
                    ServiceProviderProxy.Init(prov, serviceId);
                    return prov.GetServiceItemsBandwidth(objItems.ToArray(), since);
                },
                serviceCallAttempts,
                serviceRetryBaseDelayMs,
                (currentAttempt, ex, isTimeout) =>
                {
                    if (isTimeout)
                        Interlocked.Increment(ref totalServiceTimeoutFailures);
                    else
                        Interlocked.Increment(ref totalServiceErrorFailures);

                    TaskManager.WriteError(
                        "Bandwidth error in Service ID '{1}' on attempt {2} ({3}). Error: {0}",
                        ex.ToString(),
                        serviceId.ToString(),
                        currentAttempt.ToString(),
                        isTimeout ? "timeout" : "error");
                });

            if (retry.Success)
                return retry.Value;

            TaskManager.WriteWarning("Service ID '{0}' skipped after {1} failed bandwidth attempts. Last failure type: {2}",
                serviceId.ToString(),
                serviceCallAttempts.ToString(),
                retry.LastWasTimeout ? "timeout" : "error");
            return null;
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

        private string BuildDiskBandwidthStatisticsXml(ServiceProviderItemBandwidth[] itemsBandwidth)
        {
            int estimatedItems = itemsBandwidth == null ? 0 : itemsBandwidth.Length;
            StringBuilder sb = new StringBuilder(Math.Max(64, estimatedItems * 80));
            sb.Append("<items>");

			if (itemsBandwidth != null)
			{
				CultureInfo culture = CultureInfo.InvariantCulture;

                foreach (ServiceProviderItemBandwidth item in itemsBandwidth)
                {
                    if (item == null || item.Days == null)
                        continue;

                    foreach (DailyStatistics day in item.Days)
                    {
                        string dt = new DateTime(day.Year, day.Month, day.Day).ToString("MM/dd/yyyy", culture);
                        sb.Append("<item id=\"").Append(item.ItemId).Append("\"")
                            .Append(" date=\"").Append(dt).Append("\"")
                            .Append(" sent=\"").Append(day.BytesSent).Append("\"")
                            .Append(" received=\"").Append(day.BytesReceived).Append("\"")
                            .Append("></item>\n");
                    }
                }
			}

            sb.Append("</items>");
            return sb.ToString();
        }
    }
}



