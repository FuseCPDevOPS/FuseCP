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

using FuseCP.Server.Client;
using FuseCP.Providers.HostedSolution;
using System.Linq;

namespace FuseCP.EnterpriseServer
{
    public class DeleteExchangeAccountsTask : SchedulerTask
    {
        public override void DoWork()
        {
            DeletedAccounts();
        }

        public void DeletedAccounts()
        {
            List<Organization> organizations = OrganizationController.GetOrganizations(TaskManager.TopTask.PackageId, true);

            int attempted = 0;
            int succeeded = 0;
            int failed = 0;

            foreach (Organization organization in organizations)
            {
                try
                {
                    List<OrganizationDeletedUser> deletedUsers = OrganizationController.GetOrganizationDeletedUsers(organization.Id);

                    foreach (OrganizationDeletedUser deletedUser in deletedUsers.Where(deletedUser => deletedUser.ExpirationDate > DateTime.UtcNow))
                    {
                        attempted++;
                        try
                        {
                            OrganizationController.DeleteUser(TaskManager.TopTask.ItemId, deletedUser.AccountId);
                            succeeded++;
                        }
                        catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                        {
                            failed++;
                            TaskManager.WriteError("DeleteExchangeAccountsTask failed for AccountId '{0}': {1}", deletedUser.AccountId.ToString(), ex.ToString());
                        }
                    }
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    failed++;
                    TaskManager.WriteError("DeleteExchangeAccountsTask failed while loading deleted users for organization '{0}'. Error: {1}", organization.OrganizationId, ex.ToString());
                }
            }

            TaskManager.Write("Delete exchange accounts finished. Attempted: {0}, succeeded: {1}, failed: {2}",
                attempted.ToString(), succeeded.ToString(), failed.ToString());
        }
    }
}
