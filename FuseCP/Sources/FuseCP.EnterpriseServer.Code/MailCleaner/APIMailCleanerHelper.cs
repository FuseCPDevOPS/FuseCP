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
using System.Collections.Specialized;
using System.IO;
using System.Net;
#if NET5_0_OR_GREATER
using System.Net.Http;
#endif
using System.Xml;
using FuseCP.Providers.Filters;

namespace FuseCP.EnterpriseServer
{
    public class APIMailCleanerHelper: ControllerBase
    {
        public APIMailCleanerHelper(ControllerBase provider) : base(provider) { }
        private StringDictionary ConvertArrayToDictionary(string[] settings)
        {
            StringDictionary r = new StringDictionary();
            foreach (string setting in settings)
            {
                int idx = setting.IndexOf('=');
                r.Add(setting.Substring(0, idx), setting.Substring(idx + 1));
            }
            return r;
        }

        private string GetServiceURL(int PlanID)
        {
            var l_URL = ServerController.GetMailFilterUrlByHostingPlan(PlanID, ResourceGroups.Filters);
            if (!String.IsNullOrEmpty(l_URL))
            {
                l_URL = l_URL.TrimEnd('/');
                l_URL += "/api/";
            }
            return l_URL;
        }

        private string GetServiceURLFromPackageId(int packageId)
        {
            //String l_URL = string.Empty;
            //var fileter =  FuseCP.Portal.ES.Services.Servers.GetPackageServiceProvider(PanelSecurity.PackageId, ResourceGroups.Filters);
            var l_URL = ServerController.GetMailFilterUrl(packageId, ResourceGroups.Filters);



            //var Session = System.Web.HttpContext.Current.Session;
            //var l_PackageID  =  Session["currentPackage"];//server id
            //System.Data.DataSet dsServers = FuseCP.Portal.ES.Services.Servers.GetRawServers();
            //if (dsServers == null)
            //    return String.Empty;

            //var l_Services = new System.Data.DataView(dsServers.Tables[1], "ServerID=" + l_PackageID.ToString() , "", System.Data.DataViewRowState.CurrentRows);
            //if(l_Services.Count == 0)
            //    return String.Empty;

            ////+ " And ServiceName = 'Mail Cleaner'"
            //int l_ServiceID = 0;
            //FuseCP.EnterpriseServer.ServiceInfo l_oServiceInfo = null;
            //Boolean MailFilterAdded = false;
            //foreach (System.Data.DataRowView l_oServiceRow in l_Services)
            //{
            //    l_ServiceID = Convert.ToInt16( l_oServiceRow["ServiceID"]);
            //    l_oServiceInfo = FuseCP.Portal.ES.Services.Servers.GetServiceInfo(Convert.ToInt16(l_ServiceID));
            //    var l_oProvider = FuseCP.Portal.ES.Services.Servers.GetProvider(l_oServiceInfo.ProviderId);
            //    if (l_oProvider.ProviderName.ToUpper().Equals("MAILCLEANER"))
            //    {
            //        MailFilterAdded = true;
            //        break;
            //    }
            //}

            //if (!MailFilterAdded)
            //    return String.Empty;


            //// load service properties and bind them
            //string[] settings = FuseCP.Portal.ES.Services.Servers.GetServiceSettings(Convert.ToInt16(l_ServiceID));
            //if (settings == null)
            //    return String.Empty;

            //var l_ServiceSettings = ConvertArrayToDictionary(settings);
            //// load resource group details
            ////var resourceGroup = FuseCP.Portal.ES.Services.Servers.GetResourceGroup(provider.GroupId);
            //l_URL = l_ServiceSettings["apiurl"];

            if (!String.IsNullOrEmpty(l_URL))
            {
                l_URL = l_URL.TrimEnd('/');
                l_URL += "/api/";
            }
            return l_URL;
        }

        private void APICall(string f_stParam, int packageId, int f_iPlanID = 0)
        {
            int serviceId = Database.GetPackageServiceId(SecurityContext.User.UserId, packageId, ResourceGroups.Filters, false);
            StringDictionary settings = ServerController.GetServiceSettings(serviceId);
            String l_URL = string.Empty;
            try
            {
                l_URL = f_iPlanID == 0 ? GetServiceURLFromPackageId(packageId) : GetServiceURL(f_iPlanID);



            }
            catch (Exception)
            {
                throw;
                //throw( new Exception("MAILCLEANER_API_404"));
            }

            if (String.IsNullOrEmpty(l_URL))
                return;

            string lstXMLResult;
#if NET5_0_OR_GREATER
            var ignoreSsl = Utils.ParseBool(settings[MailCleanerContants.IgnoreCheckSSL], true);
            using (var handler = new HttpClientHandler())
            {
                handler.UseDefaultCredentials = true;
                if (ignoreSsl)
                {
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                using (var client = new HttpClient(handler))
                {
                    lstXMLResult = client.GetStringAsync(l_URL + f_stParam).GetAwaiter().GetResult();
                }
            }
#else
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = HttpWebRequest.CreateHttp(l_URL + f_stParam);
            request.Credentials = CredentialCache.DefaultCredentials;
            if (Utils.ParseBool(settings[MailCleanerContants.IgnoreCheckSSL], true))
            {
                request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            }
            using (WebResponse response = request.GetResponse())
            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                lstXMLResult = reader.ReadToEnd();
            }
#endif

            XmlDocument xmlResult = new XmlDocument();
            xmlResult.LoadXml(lstXMLResult);
            foreach (XmlNode lxmNode in xmlResult.ChildNodes)
            {
                if (lxmNode.Name == "response" && lxmNode.FirstChild.Name == "message")
                {
                    // Response message node found; no further processing required
                    break;
                }
            }


        }

        public void DomainAdd(String f_stDomain, int packageId)
        {
            TaskManager.StartTask("MAIL_CLEANER", "ADD_DOMAIN", f_stDomain);
            try
            {
                PackageContext cntx = PackageController.GetPackageContext(packageId);
                if (cntx == null) return;
                if (Convert.ToBoolean(cntx.Quotas["Filters.Enable"].QuotaAllocatedValue))
                    APICall("domain/add/name/" + f_stDomain, packageId);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }

        public void DomainRemove(String f_stDomain, int packageId)
        {
            TaskManager.StartTask("MAIL_CLEANER", "DELETE_DOMAIN", f_stDomain);
            try
            {
                PackageContext cntx = PackageController.GetPackageContext(packageId);
                if (cntx == null) return;
                if (Convert.ToBoolean(cntx.Quotas["Filters.Enable"].QuotaAllocatedValue))
                    APICall("domain/remove/name/" + f_stDomain, packageId);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is StackOverflowException) && !(ex is AccessViolationException))
            {
                throw TaskManager.WriteError(ex);
            }
            finally
            {
                TaskManager.CompleteTask();
            }
        }
    }
}


