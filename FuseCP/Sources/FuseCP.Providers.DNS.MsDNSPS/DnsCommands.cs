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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Text;
using System.Text.Json;
using FuseCP.Server.Utils;
using Microsoft.Management.Infrastructure;


namespace FuseCP.Providers.DNS
{
    /// <summary>This class wraps MS DNS server PowerShell commands used by the FuseCP.</summary>
    internal static class DnsCommands
    {
        /// <summary>Add parameter to PS command</summary>
        /// <param name="cmd">command</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Same command</returns>
        private static Command addParam(this Command cmd, string name, object value)
        {
            cmd.Parameters.Add(name, value);
            return cmd;
        }

        /// <summary>Add parameter without value to the PS command</summary>
        /// <param name="cmd">command</param>
        /// <param name="name">Parameter name</param>
        /// <returns>Same command</returns>
        private static Command addParam(this Command cmd, string name)
        {
            // http://stackoverflow.com/a/10304080/126995
            cmd.Parameters.Add(name, true);
            return cmd;
        }

        /// <summary>Create "Where-Object -Property ... -eq -Value ..." command</summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Command where(string property, object value)
        {
            return new Command("Where-Object")
                .addParam("Property", property)
                .addParam("eq")
                .addParam("Value", value);
        }

        /// <summary>Test-DnsServer -IPAddress 127.0.0.1</summary>
        /// <param name="ps">PowerShell host to use</param>
        /// <returns>true if localhost is an MS DNS server</returns>
        public static bool Test_DnsServer(this PowerShellHelper ps)
        {
            if (null == ps)
                throw new ArgumentNullException("ps");

            var cmd = new Command("Test-DnsServer")
                .addParam("IPAddress", IPAddress.Loopback);

            PSObject res = ps.RunPipeline(cmd).FirstOrDefault();
            return !(null == res || null == res.Properties);



        }

        #region Zones

        /// <summary>Get-DnsServerZone | Select-Object -Property ZoneName</summary>
        /// <remarks>Only primary DNS zones are returned</remarks>
        /// <returns>Array of zone names</returns>
        public static string[] Get_DnsServerZone_Names(this PowerShellHelper ps)
        {
            var allZones = ps.RunPipeline(new Command("Get-DnsServerZone"),
                where("IsAutoCreated", false));

            string[] res = allZones
                .Select(pso => new
                {
                    name = (string)pso.Properties["ZoneName"].Value,
                    type = (string)pso.Properties["ZoneType"].Value
                })
                .Where(obj => obj.type == "Primary")
                .Select(obj => obj.name)
                .ToArray();

            Log.WriteInfo("Get_DnsServerZone_Names: {{{0}}}", String.Join(", ", res));
            return res;
        }

        /// <summary>Returns true if the specified zone exists.</summary>
        /// <remarks>The PS pipeline being run: Get-DnsServerZone | Where-Object -Property ZoneName -eq -Value "name"</remarks>
        /// <param name="ps"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool ZoneExists(this PowerShellHelper ps, string name)
        {
            Log.WriteStart("ZoneExists {0}", name);
            bool res = ps.RunPipeline(new Command("Get-DnsServerZone"),
                where("ZoneName", name))
                .Any();
            Log.WriteEnd("ZoneExists: {0}", res);
            return res;
        }

        /* public enum eReplicationScope: byte
		{
			Custom, Domain, Forest, Legacy
		} */

        /// <summary>Adds a primary DNS server zone using the specified PowerShell helper.</summary>
        /// <param name="ps"></param>
        /// <param name="zoneName"></param>
        /// <param name="replicationScope">Specifies a partition on which to store an Active Directory-integrated zone.</param>
        /// <returns></returns>
        public static void Add_DnsServerPrimaryZone(this PowerShellHelper ps, string zoneName, string[] secondaryServers, bool AdMode)
        {
            Log.WriteStart("Add_DnsServerPrimaryZone {0} {{{1}}}", zoneName, String.Join(", ", secondaryServers));

            // Add-DnsServerPrimaryZone -Name zzz.com -ZoneFile zzz.com.dns
            var cmd = new Command("Add-DnsServerPrimaryZone");
            cmd.addParam("Name", zoneName);

            // Add AD zone if required
            if (AdMode)
            { cmd.addParam("ReplicationScope", "Forest"); }
            else
            { cmd.addParam("ZoneFile", zoneName + ".dns"); }


            ps.RunPipeline(cmd);

            // Set-DnsServerPrimaryZone -Name zzz.com -SecureSecondaries ... -Notify ... Servers ..
            cmd = new Command("Set-DnsServerPrimaryZone");
            cmd.addParam("Name", zoneName);

            if (secondaryServers == null || secondaryServers.Length == 0)
            {
                // transfers are not allowed
                // inParams2[ "SecureSecondaries" ] = 3;
                // inParams2[ "Notify" ] = 0;
                cmd.addParam("SecureSecondaries", "NoTransfer");
                cmd.addParam("Notify", "NoNotify");
            }
            else if (secondaryServers.Length == 1 && secondaryServers[0] == "*")
            {
                // allowed transfer from all servers
                // inParams2[ "SecureSecondaries" ] = 0;
                // inParams2[ "Notify" ] = 1;
                cmd.addParam("SecureSecondaries", "TransferAnyServer");
                cmd.addParam("Notify", "Notify");
            }
            else
            {
                // allowed transfer from specified servers
                // inParams2[ "SecureSecondaries" ] = 2;
                // inParams2[ "SecondaryServers" ] = secondaryServers;
                // inParams2[ "NotifyServers" ] = secondaryServers;
                // inParams2[ "Notify" ] = 2;
                cmd.addParam("SecureSecondaries", "TransferToSecureServers");
                cmd.addParam("Notify", "NotifyServers");
                cmd.addParam("SecondaryServers", secondaryServers);
                cmd.addParam("NotifyServers", secondaryServers);
            }
            ps.RunPipeline(cmd);
            Log.WriteEnd("Add_DnsServerPrimaryZone");
        }

        /// <summary>Call Add-DnsServerSecondaryZone cmdlet</summary>
        /// <param name="ps"></param>
        /// <param name="zoneName">a name of a zone</param>
        /// <param name="masterServers">an array of IP addresses of the master servers of the zone. You can use both IPv4 and IPv6.</param>
        public static void Add_DnsServerSecondaryZone(this PowerShellHelper ps, string zoneName, string[] masterServers)
        {
            // Add-DnsServerSecondaryZone -Name zzz.com -ZoneFile zzz.com.dns -MasterServers ...
            var cmd = new Command("Add-DnsServerSecondaryZone");
            cmd.addParam("Name", zoneName);
            cmd.addParam("ZoneFile", zoneName + ".dns");
            cmd.addParam("MasterServers", masterServers);
            ps.RunPipeline(cmd);
        }

        public static void Remove_DnsServerZone(this PowerShellHelper ps, string zoneName)
        {
            var cmd = new Command("Remove-DnsServerZone");
            cmd.addParam("Name", zoneName);
            cmd.addParam("Force");
            ps.RunPipeline(cmd);
        }
        #endregion

        /// <summary>Get all records, except the SOA</summary>
        /// <param name="ps"></param>
        /// <param name="zoneName">Name of the zone</param>
        /// <returns>Array of records</returns>
        public static DnsRecord[] GetZoneRecords(this PowerShellHelper ps, string zoneName)
        {
            bool runtimeMismatchDetected = false;
            Collection<PSObject> allRecords = ExecuteGetZoneRecords(ps, zoneName, null, ref runtimeMismatchDetected);

            if (allRecords.Count == 0)
            {
                allRecords = ExecuteGetZoneRecords(ps, zoneName, "localhost", ref runtimeMismatchDetected);
            }

            if (allRecords.Count == 0)
            {
                allRecords = ExecuteGetZoneRecords(ps, zoneName, Environment.MachineName, ref runtimeMismatchDetected);
            }

            DnsRecord[] records;
            if (allRecords.Count == 0 && runtimeMismatchDetected)
            {
                records = ExecuteGetZoneRecordsViaPwsh(zoneName, null);

                if (records.Length == 0)
                {
                    records = ExecuteGetZoneRecordsViaPwsh(zoneName, "localhost");
                }

                if (records.Length == 0)
                {
                    records = ExecuteGetZoneRecordsViaPwsh(zoneName, Environment.MachineName);
                }
            }
            else
            {
                records = allRecords.Select(o => o.asDnsRecord(zoneName))
                    .Where(r => null != r)
                    .Where(r => r.RecordType != DnsRecordType.SOA)
                    .OrderBy(r => r.RecordName)
                    .ThenBy(r => r.RecordType)
                    .ThenBy(r => r.RecordData)
                    .ToArray();
            }

            List<DnsRecord> result = new List<DnsRecord>();
            foreach (DnsRecord record in records.Where(record => !result.Any(res => res.RecordName.Equals(record.RecordName)
                && res.RecordType.Equals(record.RecordType)
                && res.RecordData.Equals(record.RecordData))))
            {
                result.Add(record);
            }
            return result.ToArray();
        }

        private static Collection<PSObject> ExecuteGetZoneRecords(PowerShellHelper ps, string zoneName, string computerName, ref bool runtimeMismatchDetected)
        {
            try
            {
                var cmd = new Command("Get-DnsServerResourceRecord");
                cmd.addParam("ZoneName", zoneName);
                cmd.addParam("ErrorAction", "Stop");

                if (!String.IsNullOrWhiteSpace(computerName))
                {
                    cmd.addParam("ComputerName", computerName);
                }

                return ps.RunPipeline(cmd) ?? new Collection<PSObject>();
            }
            catch (System.Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                if (!runtimeMismatchDetected && IsEnumerableAppendMismatch(ex))
                {
                    runtimeMismatchDetected = true;
                }

                string target = String.IsNullOrWhiteSpace(computerName) ? "default" : computerName;
                Log.WriteWarning("Get-DnsServerResourceRecord failed for zone '{0}' on target '{1}': {2}", zoneName, target, ex.Message);
                return new Collection<PSObject>();
            }
        }

        private static bool IsEnumerableAppendMismatch(System.Exception ex)
        {
            const string marker = "EnumerableExtensions.Append";
            return ex != null
                && ex.Message != null
                && ex.Message.IndexOf(marker, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static DnsRecord[] ExecuteGetZoneRecordsViaPwsh(string zoneName, string computerName)
        {
            try
            {
                string json = RunPwshDnsRecordsScript(zoneName, computerName);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return Array.Empty<DnsRecord>();
                }

                List<DnsRecord> records = ParsePwshDnsRecordsJson(zoneName, json);
                return records
                    .Where(r => r.RecordType != DnsRecordType.SOA)
                    .OrderBy(r => r.RecordName)
                    .ThenBy(r => r.RecordType)
                    .ThenBy(r => r.RecordData)
                    .ToArray();
            }
            catch (System.Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                string target = String.IsNullOrWhiteSpace(computerName) ? "default" : computerName;
                Log.WriteWarning("pwsh fallback Get-DnsServerResourceRecord failed for zone '{0}' on target '{1}': {2}", zoneName, target, ex.Message);
                return Array.Empty<DnsRecord>();
            }
        }

        private static string RunPwshDnsRecordsScript(string zoneName, string computerName)
        {
            string safeZone = (zoneName ?? string.Empty).Replace("'", "''");
            string safeComputer = (computerName ?? string.Empty).Replace("'", "''");
            bool useComputer = !string.IsNullOrWhiteSpace(computerName);

            string script = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "$ErrorActionPreference='Stop'; Import-Module DnsServer -ErrorAction Stop; $params=@{{ ZoneName='{0}' }}; if('{1}' -ne ''){{ $params.ComputerName='{1}' }}; $records=Get-DnsServerResourceRecord @params; $items=@(); foreach($rr in $records){{ $type=[string]$rr.RecordType; $data=''; $mx=0; $srvPr=0; $srvW=0; $srvPort=0; switch($type){{ 'A' {{ $data=$rr.RecordData.IPv4Address.IPAddressToString }} 'AAAA' {{ $data=$rr.RecordData.IPv6Address.IPAddressToString }} 'CNAME' {{ $data=$rr.RecordData.HostNameAlias }} 'MX' {{ $data=$rr.RecordData.MailExchange; $mx=[int]$rr.RecordData.Preference }} 'NS' {{ $data=$rr.RecordData.NameServer }} 'TXT' {{ $data=($rr.RecordData.DescriptiveText -join '') }} 'SRV' {{ $data=$rr.RecordData.DomainName; $srvPr=[int]$rr.RecordData.Priority; $srvW=[int]$rr.RecordData.Weight; $srvPort=[int]$rr.RecordData.Port }} 'PTR' {{ $data=$rr.RecordData.PtrDomainName }} default {{ $data=[string]$rr.RecordData }} }}; $ttl=0; if($rr.TimeToLive -ne $null){{ $ttl=[int][Math]::Round($rr.TimeToLive.TotalSeconds) }}; $items += [pscustomobject]@{{ RecordName=[string]$rr.HostName; RecordType=$type; RecordData=[string]$data; RecordTTL=$ttl; MxPriority=$mx; SrvPriority=$srvPr; SrvWeight=$srvW; SrvPort=$srvPort }} }}; $items | ConvertTo-Json -Compress -Depth 6",
                safeZone,
                useComputer ? safeComputer : string.Empty);

            byte[] scriptBytes = Encoding.Unicode.GetBytes(script);
            string encodedScript = Convert.ToBase64String(scriptBytes);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "pwsh",
                Arguments = "-NoLogo -NoProfile -NonInteractive -EncodedCommand " + encodedScript,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start pwsh process for DNS records query.");
            }

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "pwsh exited with code {0}: {1}", process.ExitCode, stderr));
            }

            return stdout;
        }

        private static List<DnsRecord> ParsePwshDnsRecordsJson(string zoneName, string json)
        {
            List<DnsRecord> records = new List<DnsRecord>();
            using JsonDocument doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in doc.RootElement.EnumerateArray())
                {
                    AddPwshRecord(records, zoneName, item);
                }
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                AddPwshRecord(records, zoneName, doc.RootElement);
            }

            return records;
        }

        private static void AddPwshRecord(List<DnsRecord> records, string zoneName, JsonElement item)
        {
            string typeText = ReadJsonString(item, "RecordType");
            if (string.IsNullOrWhiteSpace(typeText))
            {
                return;
            }

            if (!Enum.TryParse(typeText, true, out DnsRecordType recordType))
            {
                return;
            }

            DnsRecord record = new DnsRecord
            {
                RecordType = recordType,
                RecordName = RecordConverter.CorrectHost(zoneName, ReadJsonString(item, "RecordName")),
                RecordData = RecordConverter.RemoveTrailingDot(ReadJsonString(item, "RecordData")),
                RecordTTL = ReadJsonInt(item, "RecordTTL"),
                MxPriority = ReadJsonInt(item, "MxPriority"),
                SrvPriority = ReadJsonInt(item, "SrvPriority"),
                SrvWeight = ReadJsonInt(item, "SrvWeight"),
                SrvPort = ReadJsonInt(item, "SrvPort")
            };

            records.Add(record);
        }

        private static string ReadJsonString(JsonElement item, string name)
        {
            if (!item.TryGetProperty(name, out JsonElement value))
            {
                return string.Empty;
            }

            if (value.ValueKind == JsonValueKind.String)
            {
                return value.GetString() ?? string.Empty;
            }

            if (value.ValueKind == JsonValueKind.Null || value.ValueKind == JsonValueKind.Undefined)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        private static int ReadJsonInt(JsonElement item, string name)
        {
            if (!item.TryGetProperty(name, out JsonElement value))
            {
                return 0;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int number))
            {
                return number;
            }

            if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out number))
            {
                return number;
            }

            return 0;
        }

        #region Records add / remove

        public static void Add_DnsServerResourceRecordA(this PowerShellHelper ps, string zoneName, string Name, string address)
        {
            var cmd = new Command("Add-DnsServerResourceRecordA");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("IPv4Address", address);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordAAAA(this PowerShellHelper ps, string zoneName, string Name, string address)
        {
            var cmd = new Command("Add-DnsServerResourceRecordAAAA");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("IPv6Address", address);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordCName(this PowerShellHelper ps, string zoneName, string Name, string alias)
        {
            var cmd = new Command("Add-DnsServerResourceRecordCName");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("HostNameAlias", alias);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordMX(this PowerShellHelper ps, string zoneName, string Name, string mx, UInt16 pref)
        {
            var cmd = new Command("Add-DnsServerResourceRecordMX");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("MailExchange", mx);
            cmd.addParam("Preference", pref);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordNS(this PowerShellHelper ps, string zoneName, string Name, string NameServer)
        {
            var cmd = new Command("Add-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("NS");
            cmd.addParam("NameServer", NameServer);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordTXT(this PowerShellHelper ps, string zoneName, string Name, string txt)
        {
            var cmd = new Command("Add-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("Txt");
            cmd.addParam("DescriptiveText", txt);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordSRV(this PowerShellHelper ps, string zoneName, string Name, string DomainName, UInt16 Port, UInt16 Priority, UInt16 Weight)
        {
            var cmd = new Command("Add-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("Srv");
            cmd.addParam("DomainName", DomainName);
            cmd.addParam("Port", Port);
            cmd.addParam("Priority", Priority);
            cmd.addParam("Weight", Weight);
            ps.RunPipeline(cmd);
        }

        public static void Add_DnsServerResourceRecordPTR(this PowerShellHelper ps, string zoneName, string Name, string alias)
        {
            var cmd = new Command("Add-DnsServerResourceRecordPtr");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("PtrDomainName", alias);
            ps.RunPipeline(cmd);
        }

        public static void Remove_DnsServerResourceRecord(this PowerShellHelper ps, string zoneName, DnsRecord record)
        {
            string type;
            if (!RecordTypes.rrTypeFromRecord.TryGetValue(record.RecordType, out type))
                throw new Exception("Unknown record type");

            string Name = record.RecordName;
            if (String.IsNullOrEmpty(Name)) Name = "@";

            var cmd = new Command("Get-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("Name", Name);
            cmd.addParam("RRType", type);
            Collection<PSObject> resourceRecords = ps.RunPipeline(cmd);

            object inputObject = null;
            foreach (PSObject resourceRecord in resourceRecords)
            {
                DnsRecord dnsResourceRecord = resourceRecord.asDnsRecord(zoneName);

                bool found = false;

                switch (dnsResourceRecord.RecordType)
                {
                    case DnsRecordType.A:
                    case DnsRecordType.AAAA:
                    case DnsRecordType.CNAME:
                    case DnsRecordType.NS:
                    case DnsRecordType.TXT:
                    case DnsRecordType.PTR:
                        found = dnsResourceRecord.RecordData == record.RecordData;
                        break;
                    case DnsRecordType.SOA:
                        found = true;
                        break;
                    case DnsRecordType.MX:
                        found = (dnsResourceRecord.RecordData == record.RecordData) && (dnsResourceRecord.MxPriority == record.MxPriority);
                        break;
                    case DnsRecordType.SRV:
                        found = (dnsResourceRecord.RecordData == record.RecordData)
                            && (dnsResourceRecord.SrvPriority == record.SrvPriority)
                            && (dnsResourceRecord.SrvWeight == record.SrvWeight)
                            && (dnsResourceRecord.SrvPort == record.SrvPort);
                        break;
                }

                if (found)
                {
                    inputObject = resourceRecord;
                    break;
                }
            }

            cmd = new Command("Remove-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("InputObject", inputObject);

            cmd.addParam("Force");
            ps.RunPipeline(cmd);
        }

        public static void Remove_DnsServerResourceRecords(this PowerShellHelper ps, string zoneName, string type)
        {
            var cmd = new Command("Get-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("RRType", type);
            Collection<PSObject> resourceRecords = ps.RunPipeline(cmd);

            foreach (PSObject resourceRecord in resourceRecords)
            {
                cmd = new Command("Remove-DnsServerResourceRecord");
                cmd.addParam("ZoneName", zoneName);
                cmd.addParam("InputObject", resourceRecord);

                cmd.addParam("Force");
                ps.RunPipeline(cmd);
            }
        }

        public static void Update_DnsServerResourceRecordSOA(this PowerShellHelper ps, string zoneName,
            TimeSpan ExpireLimit, TimeSpan MinimumTimeToLive, string PrimaryServer,
            TimeSpan RefreshInterval, string ResponsiblePerson, TimeSpan RetryDelay,
            string PSComputerName)
        {

            var cmd = new Command("Get-DnsServerResourceRecord");
            cmd.addParam("ZoneName", zoneName);
            cmd.addParam("RRType", "SOA");
            Collection<PSObject> soaRecords = ps.RunPipeline(cmd);

            if (soaRecords.Count < 1)
                return;

            PSObject oldSOARecord = soaRecords[0];
            PSObject newSOARecord = oldSOARecord.Copy();

            CimInstance recordData = newSOARecord.Properties["RecordData"].Value as CimInstance;

            if (recordData == null) return;

            recordData.CimInstanceProperties["ExpireLimit"].Value = ExpireLimit;

            recordData.CimInstanceProperties["MinimumTimeToLive"].Value = MinimumTimeToLive;

            if (PrimaryServer != null)
                recordData.CimInstanceProperties["PrimaryServer"].Value = PrimaryServer;

            recordData.CimInstanceProperties["RefreshInterval"].Value = RefreshInterval;

            if (ResponsiblePerson != null)
                recordData.CimInstanceProperties["ResponsiblePerson"].Value = ResponsiblePerson;

            recordData.CimInstanceProperties["RetryDelay"].Value = RetryDelay;

            if (PSComputerName != null)
                recordData.CimInstanceProperties["PSComputerName"].Value = PSComputerName;

            UInt32 serialNumber = (UInt32)recordData.CimInstanceProperties["SerialNumber"].Value;

            // update record's serial number
            string sn = serialNumber.ToString();
            string todayDate = DateTime.Now.ToString("yyyyMMdd");
            if (sn.Length < 10 || !sn.StartsWith(todayDate))
            {
                // build a new serial number
                sn = todayDate + "01";
                serialNumber = UInt32.Parse(sn);
            }
            else
            {
                // just increment serial number
                serialNumber += 1;
            }

            recordData.CimInstanceProperties["SerialNumber"].Value = serialNumber;

            cmd = new Command("Set-DnsServerResourceRecord");
            cmd.addParam("NewInputObject", newSOARecord);
            cmd.addParam("OldInputObject", oldSOARecord);
            cmd.addParam("ZoneName", zoneName);
            ps.RunPipeline(cmd);

        }


        #endregion
    }
}
