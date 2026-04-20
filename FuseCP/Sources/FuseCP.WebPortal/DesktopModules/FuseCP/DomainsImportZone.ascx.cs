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
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using FuseCP.Providers.DNS;

namespace FuseCP.Portal
{
    public partial class DomainsImportZone : FuseCPModuleBase
    {
        private const int MaxZoneFileSizeBytes = 1024 * 1024;
        private const int MaxZoneRecordCount = 5000;
        private static readonly string[] AllowedZoneContentTypes = { "application/json", "text/json", "text/plain" };

        protected void Page_Load(object sender, EventArgs e)
        {
            //Get the domain information
            var domain = ES.Services.Servers.GetDomain(PanelRequest.DomainID);
            //Set the text of the literal to the domain name
            litDomainName.Text = domain.DomainName;
        }
    
        protected void UploadZoneFile_OnClick(object sender, EventArgs e)
        {
            if (Request?.Files == null || Request.Files.Count != 1)
            {
                ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
                return;
            }

            //Get the uploaded zone file
            var zoneFile = file.PostedFile;
            //First check that there was actually a file uploaded
            if (zoneFile != null && zoneFile.ContentLength > 0)
            {
                var originalFileName = Path.GetFileName(zoneFile.FileName ?? string.Empty);
                if (!string.Equals(originalFileName, zoneFile.FileName, StringComparison.Ordinal))
                {
                    ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
                    return;
                }

                var extension = Path.GetExtension(zoneFile.FileName);
                if (!string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
                {
                    ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
                    return;
                }

                if (zoneFile.ContentLength > MaxZoneFileSizeBytes)
                {
                    ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
                    return;
                }

                if (!AllowedZoneContentTypes.Contains(zoneFile.ContentType ?? string.Empty, StringComparer.OrdinalIgnoreCase))
                {
                    ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
                    return;
                }

                //Get the contents from the file
                using var reader = new StreamReader(zoneFile.InputStream);
                var contents = reader.ReadToEnd();
                try
                {
                    //Get the domain id that gets used throughout the method
                    var domainId = PanelRequest.DomainID;
                    //Try and parse the JSON to an array of DNSRecords
                    var importRecords = JsonConvert.DeserializeObject<DnsRecord[]>(contents);
                    if (importRecords == null || importRecords.Length == 0 || importRecords.Length > MaxZoneRecordCount)
                    {
                        ShowErrorMessage("DOMAIN_IMPORT");
                        return;
                    }
                    //Get the existing records on the DNS server
                    var existingRecords = ES.Services.Servers.GetDnsZoneRecords(domainId);
                    //Get the records that are new to the zone
                    var newRecords = importRecords.Except(existingRecords);
                    //Loop through add operation results for new records
                    foreach (var result in newRecords.Select(record => ES.Services.Servers.AddDnsZoneRecord(
                        domainId,
                        record.RecordName,
                        record.RecordType,
                        record.RecordData,
                        record.MxPriority,
                        record.SrvPriority,
                        record.SrvWeight,
                        record.SrvPort,
                        record.RecordTTL)).Where(result => result < 0))
                    {
                        ShowResultMessage(result);
                    }
                    //Show success message
                    ShowSuccessMessage("DOMAIN_IMPORT");
                }
                catch (System.Exception ex) when (!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException) && !(ex is System.AccessViolationException))
                {
                    //Show error message
                    ShowErrorMessage("DOMAIN_IMPORT");
                }
            }
            else
            {
                //Show error message
                ShowErrorMessage("DOMAIN_IMPORT_NO_FILE");
            }
        }
    }
}
