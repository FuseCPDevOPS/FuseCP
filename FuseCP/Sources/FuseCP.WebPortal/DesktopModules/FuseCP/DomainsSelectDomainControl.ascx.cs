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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using FuseCP.EnterpriseServer;
using FuseCP.Providers.Web;

namespace FuseCP.Portal
{
    public partial class DomainsSelectDomainControl : FuseCPControlBase
    {
        public bool HideIdnDomains
        {
            get { return (ViewState["HideIdnDomains"] != null) && (bool)ViewState["HideIdnDomains"]; }
            set { ViewState["HideIdnDomains"] = value; }
        }

        public bool HideWebSites
        {
            get { return (ViewState["HideWebSites"] != null) && (bool)ViewState["HideWebSites"]; }
            set { ViewState["HideWebSites"] = value; }
        }

        public bool HidePreviewDomain
        {
            get { return (ViewState["HidePreviewDomain"] != null) && (bool)ViewState["HidePreviewDomain"]; }
            set { ViewState["HidePreviewDomain"] = value; }
        }

        public bool HideMailDomains
        {
            get { return (ViewState["HideMailDomains"] != null) && (bool)ViewState["HideMailDomains"]; }
            set { ViewState["HideMailDomains"] = value; }
        }

        public bool HideMailDomainPointers
        {
            get { return (ViewState["HideMailDomainPointers"] != null) && (bool)ViewState["HideMailDomainPointers"]; }
            set { ViewState["HideMailDomainPointers"] = value; }
        }


        public bool HideDomainPointers
        {
            get { return (ViewState["HideDomainPointers"] != null) && (bool)ViewState["HideDomainPointers"]; }
            set { ViewState["HideDomainPointers"] = value; }
        }

        public bool HideDomainsSubDomains
        {
            get { return (ViewState["HideDomainsSubDomains"] != null) && (bool)ViewState["HideDomainsSubDomains"]; }
            set { ViewState["HideDomainsSubDomains"] = value; }
        }

        public int PackageId
        {
            get { return (ViewState["PackageId"] != null) ? (int)ViewState["PackageId"] : 0; }
            set { ViewState["PackageId"] = value; }
        }

        public int DomainId
        {
            get
            {
                return Utils.ParseInt(ddlDomains.SelectedValue, 0);
            }
        }

        public string DomainName
        {
            get
            {
                return ddlDomains.SelectedItem.Text.ToLower();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDomains();
            }

        }

        private void BindDomains()
        {
            DomainInfo[] domains = ES.Services.Servers.GetMyDomains(PackageId);

            if (HideIdnDomains)
            {
                domains = domains.Where(d => !Utils.IsIdnDomain(d.DomainName)).ToArray();
            }

            WebSite[] sites = null;
            Hashtable htSites = new Hashtable();
            Hashtable htMailDomainPointers = new Hashtable();
            if (HideWebSites)
            {
                sites = ES.Services.WebServers.GetWebSites(PackageId, false);

                foreach (WebSite w in sites)
                {
                    if (htSites[w.Name.ToLower()] == null) htSites.Add(w.Name.ToLower(), 1);

                    DomainInfo[] pointers = ES.Services.WebServers.GetWebSitePointers(w.Id);
                    foreach (DomainInfo p in pointers.Where(p => htSites[p.DomainName.ToLower()] == null))
                    {
                        htSites.Add(p.DomainName.ToLower(), 1);
                    }
                }
            }

            if (HideMailDomainPointers)
            {
                Providers.Mail.MailDomain[] mailDomains = ES.Services.MailServers.GetMailDomains(PackageId, false);

                foreach (DomainInfo[] pointers in mailDomains.Select(mailDomain => ES.Services.MailServers.GetMailDomainPointers(mailDomain.Id) ?? Array.Empty<DomainInfo>()))
                {

                    foreach (DomainInfo p in pointers.Where(p => htMailDomainPointers[p.DomainName.ToLower()] == null))
                    {
                        htMailDomainPointers.Add(p.DomainName.ToLower(), 1);
                    }
                }
            }


            ddlDomains.Items.Clear();

            // add "select" item
            ddlDomains.Items.Insert(0, new ListItem(GetLocalizedString("Text.SelectDomain"), ""));

            ddlDomains.Items.AddRange(domains.Where(domain =>
                {
                    string domainName = domain.DomainName.ToLower();
                    bool siteIsVisible = !HideWebSites
                        || (domain.WebSiteId <= 0 && (htSites == null || htSites[domainName] == null));
                    bool mailPointersAreVisible = !HideMailDomainPointers || htMailDomainPointers[domainName] == null;
                    bool previewDomainIsVisible = !HidePreviewDomain || !domain.IsPreviewDomain;
                    bool mailDomainIsVisible = !HideMailDomains || domain.MailDomainId <= 0;
                    bool domainPointerIsVisible = !HideDomainPointers || !domain.IsDomainPointer;
                    bool subDomainIsVisible = !HideDomainsSubDomains || domain.IsDomainPointer;

                    return siteIsVisible
                        && mailPointersAreVisible
                        && previewDomainIsVisible
                        && mailDomainIsVisible
                        && domainPointerIsVisible
                        && subDomainIsVisible;
                })
                .Select(domain => new ListItem(domain.DomainName.ToLower(), domain.DomainId.ToString()))
                .ToArray());

            if (Request.Cookies["CreatedDomainId"] != null)
                Utils.SelectListItem(ddlDomains, Request.Cookies["CreatedDomainId"].Value);
        }
    }
}
