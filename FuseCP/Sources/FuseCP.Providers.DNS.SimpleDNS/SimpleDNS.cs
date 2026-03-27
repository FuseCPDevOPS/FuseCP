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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Versioning;

using FuseCP.Server.Utils;
using FuseCP.Providers.Utils;
using Microsoft.Win32;

namespace FuseCP.Providers.DNS
{
	public class SimpleDNS : HostingServiceProviderBase, IDnsServer
    {
        #region Properties
        protected int ExpireLimit
		{
			get { return ProviderSettings.GetInt("ExpireLimit"); }
		}

		protected int MinimumTTL
		{
			get { return ProviderSettings.GetInt("MinimumTTL"); }
		}

		protected int RefreshInterval
		{
			get { return ProviderSettings.GetInt("RefreshInterval"); }
		}

		protected int RetryDelay
		{
			get { return ProviderSettings.GetInt("RetryDelay"); }
		}

		protected string SimpleDnsUrl
		{
			get { return ProviderSettings["SimpleDnsUrl"]; }
		}

	    protected string AdminLogin
	    {
            get { return ProviderSettings["AdminLogin"]; }
	    }

	    protected string SimpleDnsPassword
	    {
            get { return ProviderSettings["Password"]; }
	    }

	    #endregion

		#region Zones
		public virtual bool ZoneExists(string zoneName)
		{
            try
            {
                ExecuteDnsQuery("getzone", "zone=" + zoneName);
                return true;
            }
            catch (WebException ex)
            {
                Log.WriteWarning(ex.ToString());
                return false;
            }
		}

		public virtual string[] GetZones()
		{
			string response = ExecuteDnsQuery("zonelist", null);
			string[] lines = response.Split('\r', '\n');

			return lines
				.Where(line => line.Trim() != "")
				.Select(RemoveTrailingDot)
				.ToArray();
		}

		public virtual void AddPrimaryZone(string zoneName, string[] secondaryServers)
		{
			// CREATE PRIMARY DNS ZONE
			List<DnsRecord> records = new List<DnsRecord>();

			// add "Zone transfers" record
			if (secondaryServers != null && secondaryServers.Length != 0)
			{
				DnsRecord zt = new DnsRecord();
				zt.RecordType = DnsRecordType.Other;
				zt.RecordName = "";
				zt.RecordText = secondaryServers.Length == 1 && secondaryServers[0] == "*"
					? ";$AllowZT *"
					: ";$AllowZT " + String.Join(" ", secondaryServers);

				records.Add(zt);
			}

			// add SOA record
			DnsSOARecord soa = new DnsSOARecord();
			soa.RecordType = DnsRecordType.SOA;
			soa.RecordName = "";
			soa.PrimaryNsServer = System.Net.Dns.GetHostEntry("LocalHost").HostName;
			soa.PrimaryPerson = "hostmaster";//"hostmaster." + zoneName;
			records.Add(soa);

			// add DNS zone
			UpdateZone(zoneName, records);
		}

		public virtual void AddSecondaryZone(string zoneName, string[] masterServers)
		{
			//;$PrimaryIP 127.0.0.1
			//;$MinimumTTL  0

			// CREATE SECONDARY DNS ZONE
			List<DnsRecord> records = new List<DnsRecord>();

			// add DNS zone
			UpdateZone(zoneName, records, masterServers);
		}

		public virtual DnsRecord[] GetZoneRecords(string zoneName)
		{
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);
			return records
				.Where(record => record.RecordType != DnsRecordType.SOA && record.RecordType != DnsRecordType.Other)
				.ToArray();
		}

		private List<DnsRecord> GetZoneRecordsArrayList(string zoneName)
		{
			string response = ExecuteDnsQuery("getzone", "zone=" + zoneName);
			if (response.IndexOf("(404) Not Found") != -1)
				return new List<DnsRecord>();

			// parse zone file
			return ParseZoneFileToArrayList(zoneName, response);
		}

		public virtual void DeleteZone(string zoneName)
		{
			ExecuteDnsQuery("removezone", "zone=" + zoneName);
		}
		#endregion

		#region Resource records
		public virtual void AddZoneRecord(string zoneName, DnsRecord record)
		{
			try
			{
				if (record.RecordType == DnsRecordType.A)
					AddARecord(zoneName, record.RecordName, record.RecordData);
				else if (record.RecordType == DnsRecordType.AAAA)
					AddAAAARecord(zoneName, record.RecordName, record.RecordData);
				else if (record.RecordType == DnsRecordType.CNAME)
					AddCNameRecord(zoneName, record.RecordName, record.RecordData);
				else if (record.RecordType == DnsRecordType.MX)
					AddMXRecord(zoneName, record.RecordName, record.RecordData, record.MxPriority);
				else if (record.RecordType == DnsRecordType.NS)
					AddNsRecord(zoneName, record.RecordName, record.RecordData);
				else if (record.RecordType == DnsRecordType.TXT)
					AddTxtRecord(zoneName, record.RecordName, record.RecordData);
			}
			catch (WebException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to create record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
			catch (InvalidOperationException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to create record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
			catch (ArgumentException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to create record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
		}

		public virtual void AddZoneRecords(string zoneName, DnsRecord[] records)
		{
			foreach (DnsRecord record in records)
				AddZoneRecord(zoneName, record);
		}

		public virtual void DeleteZoneRecord(string zoneName, DnsRecord record)
		{
			try
			{
				if (record.RecordType == DnsRecordType.A || record.RecordType == DnsRecordType.AAAA || record.RecordType == DnsRecordType.CNAME)
					record.RecordName = CorrectRecordName(zoneName, record.RecordName);

				// delete record
				DeleteRecord(zoneName, record.RecordType, record.RecordName, record.RecordData);
			}
			catch (WebException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to delete record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
			catch (InvalidOperationException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to delete record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
			catch (ArgumentException ex)
			{
				// log exception
				Log.WriteError(
					String.Format(
						"Simple DNS: Unable to delete record (Name '{0}', Data '{1}', Text '{2}') to zone '{3}'"
						, record.RecordName
						, record.RecordData
						, record.RecordText
						, zoneName
						)
					, ex
				);
			}
		}

		public virtual void DeleteZoneRecords(string zoneName, DnsRecord[] records)
		{
			foreach (DnsRecord record in records)
				DeleteZoneRecord(zoneName, record);
		}
		#endregion

		#region A & AAAA records
		private void AddARecord(string zoneName, string host, string ip)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete A record
			//DeleteARecordInternal(records, zoneName, host);

            //check if user tries to add existent zone record
            if (records.Any(dnsRecord =>
                String.Compare(dnsRecord.RecordName, host, StringComparison.OrdinalIgnoreCase) == 0
                && String.Compare(dnsRecord.RecordData, ip, StringComparison.OrdinalIgnoreCase) == 0))
            {
                return;
            }

			// add new A record
			DnsRecord record = new DnsRecord();
			record.RecordType = DnsRecordType.A;
			record.RecordName = host;
			record.RecordData = ip;
			records.Add(record);

		    // update zone
			UpdateZone(zoneName, records);
		}

		private void AddAAAARecord(string zoneName, string host, string ip) {
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete AAAA record
			//DeleteARecordInternal(records, zoneName, host);

			//check if user tries to add existent zone record
			if (records.Any(dnsRecord =>
				String.Compare(dnsRecord.RecordName, host, StringComparison.OrdinalIgnoreCase) == 0
                && String.Compare(dnsRecord.RecordData, ip, StringComparison.OrdinalIgnoreCase) == 0))
			{
				return;
			}

			// add new AAAA record
			DnsRecord record = new DnsRecord();
			record.RecordType = DnsRecordType.AAAA;
			record.RecordName = host;
			record.RecordData = ip;
			records.Add(record);

			// update zone
			UpdateZone(zoneName, records);
		}

		private void DeleteRecord(string zoneName, DnsRecordType recordType,
			string recordName, string recordData)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete record
			DeleteRecord(zoneName, records, recordType, recordName, recordData);

			// update zone
			UpdateZone(zoneName, records);
		}

		private void DeleteRecord(string zoneName, List<DnsRecord> records, DnsRecordType recordType,
			string recordName, string recordData)
		{
			// delete record from the array
			int i = 0;
			while (i < records.Count)
			{
				if (records[i].RecordType == recordType
					&& (recordName == null || String.Compare(records[i].RecordName, recordName, true) == 0)
					&& (recordData == null || String.Compare(records[i].RecordData, recordData, true) == 0))
				{
					records.RemoveAt(i);
					break;
				}
				i++;
			}
		}

		#endregion

		#region NS records
		private void AddNsRecord(string zoneName, string host, string nameServer)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete NS record
			//DeleteNsRecordInternal(records, zoneName, nameServer);

            //check if user tries to add existent zone record
            if (records.Any(dnsRecord =>
                String.Compare(dnsRecord.RecordName, host, StringComparison.OrdinalIgnoreCase) == 0
                && String.Compare(dnsRecord.RecordData, nameServer, StringComparison.OrdinalIgnoreCase) == 0))
            {
                return;
            }

			// add new NS record
			DnsRecord record = new DnsRecord();
			record.RecordType = DnsRecordType.NS;
			record.RecordName = host;
			record.RecordData = nameServer;
			records.Add(record);

			// update zone
			UpdateZone(zoneName, records);
		}

		#endregion

		#region MX records
		private void AddMXRecord(string zoneName, string host, string mailServer, int mailServerPriority)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete MX record
			//DeleteMXRecordInternal(records, zoneName, mailServer);

            //check if user tries to add existent zone record
            if (records.Any(dnsRecord =>
                dnsRecord.RecordType == DnsRecordType.MX
                && String.Compare(dnsRecord.RecordName, host, StringComparison.OrdinalIgnoreCase) == 0
                && String.Compare(dnsRecord.RecordData, mailServer, StringComparison.OrdinalIgnoreCase) == 0
                && dnsRecord.MxPriority == mailServerPriority))
            {
                return;
            }

			// add new MX record
			DnsRecord record = new DnsRecord();
			record.RecordType = DnsRecordType.MX;
			record.RecordName = host;
			record.MxPriority = mailServerPriority;
			record.RecordData = mailServer;
			records.Add(record);

			// update zone
			UpdateZone(zoneName, records);
		}

		#endregion

		#region CNAME records
		private void AddCNameRecord(string zoneName, string alias, string targetHost)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// Find existing CNAME record. This is needed in order not to have two records after web-site is created.
			//  - verify if this record already exists... if so, we update is, otherwise create a new one
			DnsRecord record = records.Find(
					delegate(DnsRecord r) 
					{
						bool isSameRecord = (r.RecordType == DnsRecordType.CNAME) && (String.Compare(r.RecordName, alias, StringComparison.OrdinalIgnoreCase) == 0);
						if (isSameRecord)
						{
							return true;
						}

						return false;
					}
				);

			//  - if no existing records were found - add one :)
			if (record == null)
			{
				// add new CNAME record
				record = new DnsRecord();
				record.RecordType = DnsRecordType.CNAME;
				record.RecordName = alias;
				records.Add(record);
			}

			record.RecordData = targetHost;

			// update zone
			UpdateZone(zoneName, records);
		}

		#endregion

		#region TXT records
		private void AddTxtRecord(string zoneName, string host, string text)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete TXT record
			//DeleteTxtRecordInternal(records, zoneName, text);

            //check if user tries to add existent zone record
            if (records.Any(dnsRecord =>
                String.Compare(dnsRecord.RecordName, host, StringComparison.OrdinalIgnoreCase) == 0
                && String.Compare(dnsRecord.RecordData, text, StringComparison.OrdinalIgnoreCase) == 0))
            {
                return;
            }

			// add new TXT record
			DnsRecord record = new DnsRecord();
			record.RecordType = DnsRecordType.TXT;
			record.RecordName = host;
			record.RecordData = text;
			records.Add(record);

			// update zone
			UpdateZone(zoneName, records);
		}

		#endregion

		#region SOA record
		public virtual void UpdateSoaRecord(string zoneName, string host, string primaryNsServer,
			string primaryPerson)
		{
			// get all zone records
			List<DnsRecord> records = GetZoneRecordsArrayList(zoneName);

			// delete SOA record
			DeleteRecord(zoneName, records, DnsRecordType.SOA, null, null);

			// add new TXT record
			DnsSOARecord soa = new DnsSOARecord();
			soa.RecordType = DnsRecordType.SOA;
			soa.RecordName = "";
			soa.PrimaryNsServer = primaryNsServer;
			soa.PrimaryPerson = primaryPerson;
			records.Add(soa);

			// update primary person contact
			//if (soa.PrimaryPerson.ToLower().EndsWith(zoneName.ToLower()))
			//    soa.PrimaryPerson = soa.PrimaryPerson.Substring(0, (soa.PrimaryPerson.Length - zoneName.Length) - 1);

			// update zone
			UpdateZone(zoneName, records);
		}

		#endregion

		#region IHostingServiceProvier methods
		public override void DeleteServiceItems(ServiceProviderItem[] items)
		{
			foreach (DnsZone item in items.OfType<DnsZone>())
			{
				try
				{
					// delete DNS zone
					DeleteZone(item.Name);
				}
				catch (WebException ex)
				{
					Log.WriteError(String.Format("Error deleting '{0}' SimpleDNS zone", item.Name), ex);
				}
				catch (InvalidOperationException ex)
				{
					Log.WriteError(String.Format("Error deleting '{0}' SimpleDNS zone", item.Name), ex);
				}
				catch (ArgumentException ex)
				{
					Log.WriteError(String.Format("Error deleting '{0}' SimpleDNS zone", item.Name), ex);
				}
			}
		}
		#endregion

		#region Protected methods
		protected virtual DnsRecord[] ParseZoneFile(string zoneName, string zf)
		{
			return ParseZoneFileToArrayList(zoneName, zf).ToArray();
		}

		protected virtual List<DnsRecord> ParseZoneFileToArrayList(string zoneName, string zf)
		{
			List<DnsRecord> records = new List<DnsRecord>();
			using StringReader reader = new StringReader(zf);
			string zfLine = null;

			DnsSOARecord soa = null;
			while ((zfLine = reader.ReadLine()) != null)
			{
				//string line = Regex.Replace(zfLine, "\\s+", " ").Trim();

				string[] columns = zfLine.Split('\t');

				string recordName = "";
				string recordType = "";
				string recordData = "";
				string recordData2 = "";

				recordName = columns[0];
				if (columns.Length > 1 && columns.Length > 2)
					recordType = columns[2];
				if (columns.Length > 3)
					recordData = columns[3];
				if (columns.Length > 4)
					recordData2 = columns[4].Trim();

				if (recordType == "IN SOA")
				{
					string[] dataColumns = recordData.Split(' ');

					// parse SOA record
					soa = new DnsSOARecord();
					soa.RecordType = DnsRecordType.SOA;
					soa.RecordName = "";
					soa.PrimaryNsServer = RemoveTrailingDot(dataColumns[0]);
					soa.PrimaryPerson = RemoveTrailingDot(dataColumns[1]);
					soa.RecordText = zfLine;
					if (dataColumns[2] != "(")
						soa.SerialNumber = dataColumns[2];

					// add to the collection
					records.Add(soa);
				}
				else if (recordData2.IndexOf("; Serial number") != -1)
				{
					string[] dataColumns = recordData2.Split(' ');

					// append soa serial number
					soa.SerialNumber = dataColumns[0];
				}
				else if (recordType == "NS") // NS record with empty host
				{
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.NS;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.RecordData = CorrectRecordData(zoneName, recordData);
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (recordType == "A") // A record
				{
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.A;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.RecordData = recordData;
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (recordType == "AAAA") // A record
				{
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.AAAA;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.RecordData = recordData;
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (recordType == "CNAME") // CNAME record
				{
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.CNAME;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.RecordData = CorrectRecordData(zoneName, recordData, recordData);
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (recordType == "MX") // MX record
				{
					string[] dataColumns = recordData.Split(' ');

					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.MX;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.MxPriority = Int32.Parse(dataColumns[0]);
					r.RecordData = CorrectRecordData(zoneName, dataColumns[1]);
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (recordType == "TXT") // TXT record
				{
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.TXT;
					r.RecordName = CorrectRecordName(zoneName, recordName);
					r.RecordData = recordData.Substring(1, recordData.Length - 2);
					r.RecordText = zfLine;
					records.Add(r);
				}
				else if (zfLine.StartsWith(";$AllowZT")
					|| zfLine.StartsWith(";$PrimaryIP")
					|| zfLine.StartsWith(";$MinimumTTL"))
				{
					// unknown record just keep it line
					DnsRecord r = new DnsRecord();
					r.RecordType = DnsRecordType.Other;
					r.RecordName = "";
					r.RecordText = zfLine;
					records.Add(r);
				}

				//Debug.WriteLine(zfLine);
			}
			return records;
		}


		protected virtual void UpdateZone(string zoneName, List<DnsRecord> records)
		{
			UpdateZone(zoneName, records, null);
		}

		protected virtual void UpdateZone(string zoneName, List<DnsRecord> records, string[] masterServers)
		{
			// build zone file
			StringBuilder sb = new StringBuilder();

			// add FuseCP comment
			sb.Append(";$; Updated with FuseCP DNS API ").Append(DateTime.Now).Append("\r");

			// render comment/service records
			foreach (DnsRecord rr in records)
			{
				if (rr.RecordText != null && rr.RecordText.StartsWith(";") && rr.RecordType != DnsRecordType.TXT)
				{
					sb.Append(rr.RecordText);

					// add line break
					sb.Append("\r");
				}
			}

			// render SOA record
			foreach (DnsRecord rr in records)
			{
				string host = "";
				string type = "";
				string data = "";

				if (rr is DnsSOARecord)
				{
					type = "IN SOA";
					DnsSOARecord soa = (DnsSOARecord)rr;
					host = soa.RecordName;
					data = String.Format("{0} {1} {2} {3} {4} {5} {6}",
						CorrectSOARecord(zoneName, soa.PrimaryNsServer),
						CorrectSOARecord(zoneName, soa.PrimaryPerson),
						UpdateSerialNumber(soa.SerialNumber),
						RefreshInterval,
						RetryDelay,
						ExpireLimit,
						MinimumTTL);

					// add line to the zone file
					sb.Append(BuildRecordName(zoneName, host)).Append("\t");
					sb.Append("\t");
					sb.Append(type).Append("\t");
					sb.Append(data);

					// add line break
					sb.Append("\r");
				}
			}

			// render all other records
			foreach (DnsRecord rr in records)
			{
				string host = String.Empty;
				string type = String.Empty;
				string data = String.Empty;
				string name = String.Empty;

				if (rr.RecordType == DnsRecordType.A)
				{
					type = "A";
					host = rr.RecordName;
					data = rr.RecordData;
					name = BuildRecordName(zoneName, host);
				}
				else if (rr.RecordType == DnsRecordType.AAAA) {
					type = "AAAA";
					host = rr.RecordName;
					data = rr.RecordData;
					name = BuildRecordName(zoneName, host);
				}
				else if (rr.RecordType == DnsRecordType.NS)
				{
					type = "NS";
					host = rr.RecordName;
					data = BuildRecordData(zoneName, rr.RecordData);
					name = BuildRecordName(zoneName, host);
				}
				else if (rr.RecordType == DnsRecordType.CNAME)
				{
					type = "CNAME";
					host = rr.RecordName;
					data = BuildRecordData(zoneName, rr.RecordData, rr.RecordData);
					name = host;
				}
				else if (rr.RecordType == DnsRecordType.MX)
				{
					type = "MX";
					host = rr.RecordName;
					data = String.Format("{0} {1}",
						rr.MxPriority,
						BuildRecordData(zoneName, rr.RecordData));
					name = BuildRecordName(zoneName, host);
				}
				else if (rr.RecordType == DnsRecordType.TXT)
				{
					type = "TXT";
					host = rr.RecordName;
					data = "\"" + rr.RecordData + "\"";
					name = BuildRecordName(zoneName, host);
				}

				// add line to the zone file
				if (type != "")
				{
					sb.Append(name).Append("\t");
					if (type == "NS")
						sb.Append(MinimumTTL);
					sb.Append("\t");
					sb.Append(type).Append("\t");
					sb.Append(data);

					// add line break
					sb.Append("\r");
				}
			}

			string zoneFile = sb.ToString();

			// update zone file
			string queryParams = "zone=" + zoneName + "&data=" + zoneFile;
			if (masterServers != null && masterServers.Length > 0)
				queryParams += "&masterip=" + masterServers[0];

			// execute query
			ExecuteDnsQuery("updatezone", queryParams);
		}
		#endregion

		#region private methods
		private string CorrectRecordName(string zoneName, string host)
		{
			if (host == "" || host == "@")
				return "";
			else if (host.EndsWith("."))
				return RemoveTrailingDot(host);
			else
				return host;
		}

		private string CorrectRecordData(string zoneName, string host, string recordData)
		{
			if (recordData == "@")
			{
				return zoneName;
			}

			return CorrectRecordData(zoneName, host);
		}

		private string CorrectRecordData(string zoneName, string host)
		{
			if (host == "" || host == "@")
				return "";
			else if (host.EndsWith("."))
				return RemoveTrailingDot(host);
			else
				return host + "." + zoneName;
		}

		protected string BuildRecordName(string zoneName, string host)
		{
			if (host == "")
				return "@";
			else if (host.EndsWith(zoneName))
				return host.Substring(0, host.Length - zoneName.Length - 1);
			else
				return host;
		}

		protected string BuildRecordData(string zoneName, string host, string recordData)
		{
			if (String.Compare(zoneName, recordData, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return "@";
			}

			return BuildRecordData(zoneName, host);
		}

		protected string BuildRecordData(string zoneName, string host)
		{
			try
			{

				if (host == "")
					return "@";
				else if (host.EndsWith("." + zoneName))
					return host.Substring(0, host.Length - zoneName.Length - 1);
				else
					return host + ".";

			}
			catch (ArgumentOutOfRangeException ex)
			{
				Log.WriteError(
					  String.Format(
						"Simple DNS: Cannot build record data. Zone name: {0}, Host {1}."
						, zoneName
						, host
					  )
					, ex
				);

				throw;
			}
		}

		protected string CorrectSOARecord(string zoneName, string data)
		{
			if (data == "")
				return "@";
			else if (data.EndsWith("." + zoneName))
				return data.Substring(0, data.Length - zoneName.Length - 1);
			else if (data.IndexOf(".") == -1)
				return data;
			else
				return data + ".";
		}

		protected string UpdateSerialNumber(string serialNumber)
		{
			// update record's serial number
			string sn = serialNumber;
			string todayDate = DateTime.Now.ToString("yyyyMMdd");
			if (sn == null || sn.Length < 10 || !sn.StartsWith(todayDate))
			{
				// build a new serial number
				return todayDate + "01";
			}
			else
			{
				// just increment serial number
				int newSerialNumber = Int32.Parse(serialNumber);
				newSerialNumber += 1;
				return newSerialNumber.ToString();
			}
		}

		private string RemoveTrailingDot(string str)
		{
			return str.Length == 0 || str[str.Length - 1] != '.' ? str : str.Substring(0, str.Length - 1);



		}

		private static string UrlEncodeData(string value)
		{
			return String.IsNullOrEmpty(value) ? String.Empty : Uri.EscapeDataString(value);
		}

		private static string BuildUrlEncodedPostData(string postData)
		{
			if (postData == null)
			{
				return null;
			}

			StringBuilder urlEncoded = new StringBuilder();
			char[] reserved = { '?', '=', '&' };

			int i = 0;
			while (i < postData.Length)
			{
				int j = postData.IndexOfAny(reserved, i);
				if (j == -1)
				{
					urlEncoded.Append(UrlEncodeData(postData.Substring(i, postData.Length - i)));
					break;
				}

				urlEncoded.Append(UrlEncodeData(postData.Substring(i, j - i)));
				urlEncoded.Append(postData.Substring(j, 1));
				i = j + 1;
			}

			return urlEncoded.ToString();
		}


        private string ExecuteDnsQuery(string command, string postData)
		{
			StringBuilder sb = new StringBuilder();

			string endpoint = SimpleDnsUrl + "/" + command;
			HttpMethod method = postData == null ? HttpMethod.Get : HttpMethod.Post;

			using HttpClient client = new HttpClient();
			using HttpRequestMessage req = new HttpRequestMessage(method, endpoint);
			req.Headers.UserAgent.ParseAdd("FuseCP-SimpleDNS-Provider/1.0");

			if (!String.IsNullOrEmpty(SimpleDnsPassword))
			{
				string authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(AdminLogin + ":" + SimpleDnsPassword));
				req.Headers.Authorization = new AuthenticationHeaderValue("Basic", authToken);
			}

			if (postData != null)
			{
				string urlEncodedData = BuildUrlEncodedPostData(postData);
				req.Content = new StringContent(urlEncodedData, Encoding.ASCII, "application/x-www-form-urlencoded");
			}

			using HttpResponseMessage result = client.Send(req);
			result.EnsureSuccessStatusCode();
			using Stream receiveStream = result.Content.ReadAsStream();
			using StreamReader sr = new StreamReader(receiveStream, Encoding.UTF8);

			//Console.WriteLine("\r\nResponse stream received");
			Char[] read = new Char[256];
			int count = sr.Read(read, 0, 256);

			//Console.WriteLine("HTML...\r\n");
			while (count > 0)
			{
				String str = new String(read, 0, count);
				sb.Append(str);
				count = sr.Read(read, 0, 256);
			}

            return sb.ToString();
		}
		#endregion

		private bool IsSimpleDnsApiAvailable()
		{
			try
			{
				ExecuteDnsQuery("zonelist", null);
				return true;
			}
			catch
			{
				return false;
			}
		}

		[SupportedOSPlatform("windows")]
		private bool IsSimpleDnsVersion4InstalledOnWindows()
		{
			string productName = null;
			string productVersion = null;

			RegistryKey HKLM = Registry.LocalMachine;

			RegistryKey key = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");

			if (key != null)
			{
				String[] names = key.GetSubKeyNames();

				foreach (string s in names)
				{
					RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
					if (subkey != null && !String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
					{
						productName = (string)subkey.GetValue("DisplayName");
					}
					if (productName != null && productName.Equals("Simple DNS Plus"))
					{
						if (subkey != null) productVersion = (string)subkey.GetValue("DisplayVersion");
						break;
					}
				}

				if (!String.IsNullOrEmpty(productVersion))
				{
					string[] split = productVersion.Split(new char[] { '.' });
					return split[0].Equals("4");
				}

				//checking x64 platform
				key = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");

				if (key == null)
				{
					return false;
				}

				names = key.GetSubKeyNames();

				foreach (string s in names)
				{
					RegistryKey subkey = HKLM.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\" + s);
					if (subkey != null && !String.IsNullOrEmpty((string)subkey.GetValue("DisplayName")))
					{
						productName = (string)subkey.GetValue("DisplayName");
					}
					if (productName != null && productName.Equals("Simple DNS Plus"))
					{
						if (subkey != null) productVersion = (string)subkey.GetValue("DisplayVersion");
						break;
					}
				}

				if (!String.IsNullOrEmpty(productVersion))
				{
					string[] split = productVersion.Split(new[] { '.' });
					return split[0].Equals("4");
				}
			}

			return false;
		}

        public override bool IsInstalled()
        {
            if (IsSimpleDnsApiAvailable())
                return true;

            if (!OperatingSystem.IsWindows())
                return false;

            return IsSimpleDnsVersion4InstalledOnWindows();
        }

       
	}
}
