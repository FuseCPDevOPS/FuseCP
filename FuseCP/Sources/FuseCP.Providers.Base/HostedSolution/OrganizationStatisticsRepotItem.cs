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

﻿namespace FuseCP.Providers.HostedSolution
{
    public class OrganizationStatisticsRepotItem : BaseStatistics
    {
        public int TotalMailboxes
        {
            get;
            set;
        }

        public long TotalMailboxesSize
        {
            get;
            set;
        }

        public long TotalPublicFoldersSize
        {
            get;
            set;
        }

        public int TotalSharePointSiteCollections
        {
            get;
            set;
        }

        public long TotalSharePointSiteCollectionsSize
        {
            get;
            set;
        }

        public int TotalSharePointEnterpriseSiteCollections
        {
            get;
            set;
        }

        public long TotalSharePointEnterpriseSiteCollectionsSize
        {
            get;
            set;
        }


        public int TotalCRMUsers
        {
            get;
            set;
        }

    }
}
