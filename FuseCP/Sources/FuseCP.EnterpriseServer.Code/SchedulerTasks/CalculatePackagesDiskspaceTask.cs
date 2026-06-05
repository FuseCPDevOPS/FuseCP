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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FuseCP.EnterpriseServer.Code.SharePoint;
using FuseCP.Providers;
using FuseCP.Providers.HostedSolution;
using FuseCP.Providers.SharePoint;
using FuseCP.Server.Client;

namespace FuseCP.EnterpriseServer
{
    public class CalculatePackagesDiskspaceTask : SchedulerTask
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

            CalculateDiskspace();
        }

        public void CalculateDiskspace()
        {
            // get all owned packages
            List<PackageInfo> packages = PackageController.GetPackagePackages(TaskManager.TopTask.PackageId, true);
            TaskManager.Write("Packages to calculate: " + packages.Count);
            TaskManager.Write("Diskspace package parallelism: {0}", maxParallelPackages.ToString());

            int packageSuccessCount = 0;
            int packageFailureCount = 0;
            int processedCount = 0;

            if (maxParallelPackages <= 1 || packages.Count <= 1)
            {
                foreach (PackageInfo package in packages)
                {
                    // calculating package diskspace
                    if (CalculatePackage(package.PackageId))
                        packageSuccessCount++;
                    else
                        packageFailureCount++;

                    processedCount++;
                    if (processedCount % ProgressLogInterval == 0)
                    {
                        TaskManager.Write("Diskspace progress: processed {0}/{1} packages", processedCount.ToString(), packages.Count.ToString());
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
                        TaskManager.Write("Diskspace progress: processed {0}/{1} packages", processed.ToString(), packages.Count.ToString());
                    }
                });
            }

            TaskManager.Write("Diskspace calculation finished. Total packages: {0}, successful: {1}, failed: {2}",
                packages.Count.ToString(), packageSuccessCount.ToString(), packageFailureCount.ToString());
            TaskManager.Write("Diskspace service call failures. Timeout: {0}, Other errors: {1}",
                totalServiceTimeoutFailures.ToString(), totalServiceErrorFailures.ToString());
        }

        public bool CalculatePackage(int packageId)
        {
            try
            {
                // get all package items
                List<ServiceProviderItem> items = PackageController.GetServiceItemsForStatistics(
                    0, packageId, true, false, false, false);

                //TaskManager.Write("Items: " + items.Count);

                // order items by service
                Dictionary<int, List<ServiceProviderItem>> orderedItems =
                    PackageController.OrderServiceItemsByServices(items);

                // calculate statistics for each service set
                List<ServiceProviderItemDiskSpace> itemsDiskspace = new List<ServiceProviderItemDiskSpace>();
                int serviceFailures = 0;
                foreach (int serviceId in orderedItems.Keys)
                {
                    ServiceProviderItemDiskSpace[] serviceDiskspace = CalculateItems(packageId, serviceId, orderedItems[serviceId]);
                    if (serviceDiskspace == null)
                    {
                        serviceFailures++;
                        continue;
                    }

                    itemsDiskspace.AddRange(serviceDiskspace.Where(ds => ds != null));
                }

                if (serviceFailures > 0)
                {
                    TaskManager.WriteError("Diskspace partial result for package '{0}': {1} service(s) failed and were skipped",
                        packageId.ToString(), serviceFailures.ToString());
                }

                // update info in the database
                string xml = BuildDiskSpaceStatisticsXml(itemsDiskspace.ToArray());
                PackageController.UpdatePackageDiskSpace(packageId, xml);
                //TaskManager.Write("XML: " + xml);

                // suspend package if requested
                if (suspendOverused)
                {
                    // disk space
                    QuotaValueInfo dsQuota = PackageController.GetPackageQuota(packageId, Quotas.OS_DISKSPACE);

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
                TaskManager.WriteError(String.Format("Error calculating diskspace for '{0}' space of user '{1}': {2}",
                    package.PackageName, user.Username, ex));

                return false;
            }
        }

        private int GetExchangeServiceID(int packageId)
        {
            return PackageController.GetPackageServiceId(packageId, ResourceGroups.Exchange);
        }


        public ServiceProviderItemDiskSpace[] CalculateItems(int packageId, int serviceId, List<ServiceProviderItem> items)
        {
            // convert items to SoapObjects
            List<SoapServiceProviderItem> objItems = new List<SoapServiceProviderItem>();
            
            //hack for organization... Refactoring!!!

           
            List<ServiceProviderItemDiskSpace> organizationDiskSpaces = new List<ServiceProviderItemDiskSpace>(items.Count);
            PackageContext packageContext = null;
            bool packageContextLoaded = false;
            bool hasSharePointFoundation = false;
            bool hasSharePointEnterprise = false;
            int exchangeServiceId = 0;

            if (items.Count > 0)
            {
                packageContext = PackageController.GetPackageContext(packageId);
                packageContextLoaded = packageContext != null;
                if (packageContextLoaded)
                {
                    hasSharePointFoundation = packageContext.Groups.ContainsKey(ResourceGroups.SharepointFoundationServer);
                    hasSharePointEnterprise = packageContext.Groups.ContainsKey(ResourceGroups.SharepointEnterpriseServer);
                }

                exchangeServiceId = GetExchangeServiceID(packageId);
            }

            foreach (ServiceProviderItem item in items)
            {
                long size = 0;
                if (item is Organization)
                {
                    Organization org = (Organization) item;

                    //Exchange DiskSpace
                    if (!string.IsNullOrEmpty(org.GlobalAddressList))
                    {
                        try
                        {
                            if (exchangeServiceId > 0)
                            {
                                ServiceProvider exchangeProvider = ExchangeServerController.GetExchangeServiceProvider(exchangeServiceId, item.ServiceId);

                                SoapServiceProviderItem soapOrg = SoapServiceProviderItem.Wrap(org);
                                ServiceProviderItemDiskSpace[] itemsDiskspace =
                                    exchangeProvider.GetServiceItemsDiskSpace(new SoapServiceProviderItem[] { soapOrg });

                                if (itemsDiskspace != null && itemsDiskspace.Length > 0)
                                {
                                    size += itemsDiskspace[0].DiskSpace;
                                }
                            }
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            if (SchedulerTaskReliability.IsTimeout(ex))
                                Interlocked.Increment(ref totalServiceTimeoutFailures);
                            else
                                Interlocked.Increment(ref totalServiceErrorFailures);

                            TaskManager.WriteError("Diskspace exchange error for Organization '{1}' ({2}). Error: {0}",
                                ex.ToString(),
                                org.Id.ToString(),
                                SchedulerTaskReliability.IsTimeout(ex) ? "timeout" : "error");
                        }
                    }

                    // Crm DiskSpace
                    if (org.CrmOrganizationId != Guid.Empty)
                    {
                        //CalculateCrm DiskSpace
                    }

                    //SharePoint DiskSpace

                    int res;

                    if (packageContextLoaded && hasSharePointFoundation)
                    {
                        try
                        {
                            SharePointSiteDiskSpace[] sharePointSiteDiskSpaces =
                                HostedSharePointServerController.CalculateSharePointSitesDiskSpace(org.Id, out res);
                            if (res == 0)
                                size += sharePointSiteDiskSpaces.Sum(s => s.DiskSpace);
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            if (SchedulerTaskReliability.IsTimeout(ex))
                                Interlocked.Increment(ref totalServiceTimeoutFailures);
                            else
                                Interlocked.Increment(ref totalServiceErrorFailures);

                            TaskManager.WriteError("Diskspace SharePoint Foundation error for Organization '{1}' ({2}). Error: {0}",
                                ex.ToString(),
                                org.Id.ToString(),
                                SchedulerTaskReliability.IsTimeout(ex) ? "timeout" : "error");
                        }
                    }

                    if (packageContextLoaded && hasSharePointEnterprise)
                    {
                        try
                        {
                            SharePointSiteDiskSpace[] sharePointSiteDiskSpaces =
                                HostedSharePointServerEntController.CalculateSharePointSitesDiskSpace(org.Id, out res);
                            if (res == 0)
                                size += sharePointSiteDiskSpaces.Sum(s => s.DiskSpace);
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            if (SchedulerTaskReliability.IsTimeout(ex))
                                Interlocked.Increment(ref totalServiceTimeoutFailures);
                            else
                                Interlocked.Increment(ref totalServiceErrorFailures);

                            TaskManager.WriteError("Diskspace SharePoint Enterprise error for Organization '{1}' ({2}). Error: {0}",
                                ex.ToString(),
                                org.Id.ToString(),
                                SchedulerTaskReliability.IsTimeout(ex) ? "timeout" : "error");
                        }
                    }

                    ServiceProviderItemDiskSpace tmp = new ServiceProviderItemDiskSpace();
                    tmp.ItemId = item.Id;
                    tmp.DiskSpace = size;
                    organizationDiskSpaces.Add(tmp);
                }
                else
                    objItems.Add(SoapServiceProviderItem.Wrap(item));
            }
            
            
            var retry = SchedulerTaskReliability.ExecuteWithRetry(
                () =>
                {
                    if (objItems.Count > 0)
                    {
                        ServiceProvider prov = new ServiceProvider();
                        ServiceProviderProxy.Init(prov, serviceId);
                        ServiceProviderItemDiskSpace[] itemsDiskSpace = prov.GetServiceItemsDiskSpace(objItems.ToArray());
                        if (itemsDiskSpace != null && itemsDiskSpace.Length > 0)
                            organizationDiskSpaces.AddRange(itemsDiskSpace);
                    }

                    return organizationDiskSpaces.ToArray();
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
                        "Diskspace error in Service ID '{1}' on attempt {2} ({3}). Error: {0}",
                        ex.ToString(),
                        serviceId.ToString(),
                        currentAttempt.ToString(),
                        isTimeout ? "timeout" : "error");
                });

            if (retry.Success)
                return retry.Value;

            TaskManager.WriteWarning("Service ID '{0}' skipped after {1} failed diskspace attempts. Last failure type: {2}",
                serviceId.ToString(),
                serviceCallAttempts.ToString(),
                retry.LastWasTimeout ? "timeout" : "error");
            return organizationDiskSpaces.ToArray();
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

        private string BuildDiskSpaceStatisticsXml(ServiceProviderItemDiskSpace[] itemsDiskspace)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<items>");

			if (itemsDiskspace != null)
			{
				foreach (ServiceProviderItemDiskSpace item in itemsDiskspace)
				{
					sb.Append("<item id=\"").Append(item.ItemId).Append("\"")
						.Append(" bytes=\"").Append(item.DiskSpace).Append("\"></item>\n");
				}
			}

            sb.Append("</items>");
            return sb.ToString();
        }
    }
}



