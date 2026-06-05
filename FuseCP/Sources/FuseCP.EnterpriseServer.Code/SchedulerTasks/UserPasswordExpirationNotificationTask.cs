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
using FuseCP.Providers.HostedSolution;

namespace FuseCP.EnterpriseServer
{
    public class UserPasswordExpirationNotificationTask : SchedulerTask
    {
        private static readonly string DaysBeforeNotify = "DAYS_BEFORE_EXPIRATION";

        public override void DoWork()
        {
            BackgroundTask topTask = TaskManager.TopTask;

            int daysBeforeNotify;
            if (!int.TryParse((string)topTask.GetParamValue(DaysBeforeNotify), out daysBeforeNotify))
            {
                TaskManager.WriteWarning("Specify 'Notify before (days)' task parameter");
                return;
            }

            OrganizationController.DeleteAllExpiredTokens();

            var owner = UserController.GetUser(topTask.EffectiveUserId);
            var packages = PackageController.GetMyPackages(topTask.EffectiveUserId);
            int processedOrganizations = 0;
            int failedOrganizations = 0;

            foreach (var package in packages)
            {
                try
                {
                    var organizations = ExchangeServerController.GetExchangeOrganizations(package.PackageId, true);

                    foreach (var organization in organizations)
                    {
                        try
                        {
                            var usersWithExpiredPasswords = OrganizationController.GetOrganizationUsersWithExpiredPassword(organization.Id, daysBeforeNotify);
                            var generalSettings = OrganizationController.GetOrganizationGeneralSettings(organization.Id);
                            var logoUrl = generalSettings != null ? generalSettings.OrganizationLogoUrl : string.Empty;

                            foreach (var user in usersWithExpiredPasswords)
                            {
                                try
                                {
                                    user.ItemId = organization.Id;

                                    if (string.IsNullOrEmpty(user.PrimaryEmailAddress))
                                    {
                                        TaskManager.WriteWarning(string.Format("Unable to send email to {0} user (organization: {1}), user primary email address is not set.", user.DisplayName, organization.OrganizationId));
                                        continue;
                                    }

                                    OrganizationController.SendUserExpirationPasswordEmail(owner, user, "Scheduler Password Expiration Notification", user.PrimaryEmailAddress, logoUrl);
                                }
                                catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                                {
                                    failedOrganizations++;
                                    TaskManager.WriteError("UserPasswordExpirationNotificationTask failed for user '{0}' in organization '{1}'. Error: {2}", user.DisplayName, organization.OrganizationId, ex.ToString());
                                }
                            }

                            processedOrganizations++;
                        }
                        catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                        {
                            failedOrganizations++;
                            TaskManager.WriteError("UserPasswordExpirationNotificationTask failed for organization '{0}'. Error: {1}", organization.OrganizationId, ex.ToString());
                        }
                    }
                }
                catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
                {
                    failedOrganizations++;
                    TaskManager.WriteError("UserPasswordExpirationNotificationTask failed while loading organizations for package '{0}'. Error: {1}", package.PackageId.ToString(), ex.ToString());
                }
            }

            TaskManager.Write("UserPasswordExpirationNotificationTask finished. Processed organizations: {0}, failures: {1}", processedOrganizations.ToString(), failedOrganizations.ToString());
        }
    }
}
