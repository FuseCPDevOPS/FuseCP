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
using System.Management;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace FuseCP.Providers.Virtualization
{
    /// <summary>
    /// DEPRECATED: This class will be removed in the next release (after 2025?).
    /// Please use <c>MiManager</c> instead.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal class Wmi
    {
        readonly string nameSpace = null;
        readonly string computerName = null;
        ManagementScope scope = null;

        public Wmi(string nameSpace) : this(nameSpace, null)
        {
        }

        public Wmi(string computerName, string nameSpace)
        {
            this.nameSpace = nameSpace;
            this.computerName = computerName;
        }

        internal ManagementObjectCollection ExecuteWmiQuery(string query, params object[] args)
        {
            if (args != null && args.Length > 0)
                query = String.Format(query, args);

            using ManagementObjectSearcher searcher = new ManagementObjectSearcher(GetScope(),
                new ObjectQuery(query));
            return searcher.Get();
        }

        internal ManagementObject GetWmiObject(string className, string filter, params object[] args)
        {
            ManagementObjectCollection col = GetWmiObjects(className, filter, args);
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = col.GetEnumerator();
            return enumerator.MoveNext() ? (ManagementObject)enumerator.Current : null;
        }

        internal ManagementObject GetWmiObject(string className)
        {
            return GetWmiObject(className, null);
        }

        internal ManagementObjectCollection GetWmiObjects(string className, string filter, params object[] args)
        {
            string query = "select * from " + className;
            if (!String.IsNullOrEmpty(filter))
                query += " where " + filter;
            return ExecuteWmiQuery(query, args);
        }

        internal ManagementObjectCollection GetWmiObjects(string className)
        {
            return GetWmiObjects(className, null);
        }

        internal ManagementObject GetWmiObjectByPath(string path)
        {
            return new ManagementObject(GetScope(), new ManagementPath(path), null);
        }

        internal ManagementClass GetWmiClass(string className)
        {
            return new ManagementClass(GetScope(), new ManagementPath(className), null);
        }

        internal ManagementObject GetRelatedWmiObject(ManagementObject obj, string className)
        {
            ManagementObjectCollection col = obj.GetRelated(className);
            ManagementObjectCollection.ManagementObjectEnumerator enumerator = col.GetEnumerator();
            return enumerator.MoveNext() ? (ManagementObject)enumerator.Current : null;
        }

        internal void Dump(ManagementBaseObject obj)
        {
#if DEBUG
            foreach (PropertyData prop in obj.Properties)
            {
                string typeName = prop.Value == null ? "null" : prop.Value.GetType().ToString();
                Debug.WriteLine(prop.Name + ": " + prop.Value + " (" + typeName + ")");
            }
#endif
        }

        // Converts a given datetime in DMTF format to System.DateTime object.
        internal static System.DateTime ToDateTime(string dmtfDate)
        {
            return ManagementDateTimeConverter.ToDateTime(dmtfDate);
        }

        // Converts a given System.DateTime object to DMTF datetime format.
        internal string ToDmtfDateTime(System.DateTime date)
        {
            return ManagementDateTimeConverter.ToDmtfDateTime(date);
        }

        public ManagementScope GetScope()
        {
            if (scope != null)
                return scope;

            // create new scope
            if (String.IsNullOrEmpty(computerName))
            {
                // local
                scope = new ManagementScope(nameSpace);
            }
            else
            {
                // remote
                ConnectionOptions options = new ConnectionOptions();

                string path = String.Format(@"\\{0}\{1}", computerName, nameSpace);
                scope = new ManagementScope(path, options);
            }

            // connect
            scope.Connect();
            return scope;
        }
    }
}
