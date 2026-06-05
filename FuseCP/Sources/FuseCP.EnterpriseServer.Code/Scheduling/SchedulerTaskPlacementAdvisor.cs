// Copyright (C) 2026 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

using System;

namespace FuseCP.EnterpriseServer
{
    internal enum SchedulerPlacementMode
    {
        Auto = 0,
        ServerPreferred = 1,
        EnterpriseOnly = 2
    }

    internal static class SchedulerTaskPlacementAdvisor
    {
        public static SchedulerPlacementMode GetRecommendedMode(string taskType, string taskId)
        {
            string descriptor = ((taskType ?? String.Empty) + "|" + (taskId ?? String.Empty)).ToUpperInvariant();

            if (ContainsAny(descriptor,
                "AUDITLOGREPORTTASK",
                "HOSTEDSOLUTIONREPORTTASK",
                "SUSPENDOVERUSEDPACKAGESTASK",
                "NOTIFYOVERUSEDDATABASESTASK",
                "USERPASSWORDEXPIRATIONNOTIFICATIONTASK",
                "SENDMAILNOTIFICATIONTASK",
                "BACKUPDATABASETASK"))
            {
                return SchedulerPlacementMode.EnterpriseOnly;
            }

            if (ContainsAny(descriptor,
                "CALCULATEPACKAGESDISKSPACETASK",
                "CALCULATEPACKAGESBANDWIDTHTASK",
                "CALCULATEEXCHANGEDISKSPACETASK",
                "CHECKWEBSITETASK",
                "DELETEEXCHANGEACCOUNTSTASK",
                "BACKUPTASK",
                "FTPFILESTASK",
                "DOMAINLOOKUPVIEWTASK",
                "RUNSYSTEMCOMMANDTASK",
                "ZIPFILESTASK"))
            {
                return SchedulerPlacementMode.ServerPreferred;
            }

            // SSL checks can be useful from an external perspective; keep AUTO by default.
            return SchedulerPlacementMode.Auto;
        }

        private static bool ContainsAny(string source, params string[] markers)
        {
            if (String.IsNullOrEmpty(source) || markers == null)
                return false;

            foreach (string marker in markers)
            {
                if (!String.IsNullOrEmpty(marker) && source.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }
    }
}
