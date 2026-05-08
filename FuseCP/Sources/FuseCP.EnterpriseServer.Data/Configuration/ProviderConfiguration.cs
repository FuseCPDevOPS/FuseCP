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
using FuseCP.EnterpriseServer.Data.Configuration;
using FuseCP.EnterpriseServer.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
#if NetCore
using Microsoft.EntityFrameworkCore;
#endif
#if NetFX
using System.Data.Entity;
#endif

namespace FuseCP.EnterpriseServer.Data.Configuration;

public partial class ProviderConfiguration: EntityTypeConfiguration<Provider>
{
    public override void Configure() {
        HasKey(e => e.ProviderId).HasName("PK_Provider");

#if NetCore
        Property(e => e.ProviderId).ValueGeneratedNever();

        HasOne(d => d.Group).WithMany(p => p.Providers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Providers_ResourceGroups");
#else
        HasRequired(d => d.Group).WithMany(p => p.Providers);
#endif

		#region Seed Data
		HasData(() => new Provider[] {
			//new Provider() { ProviderId = 1, DisplayName = "Windows Server 2003", EditorControl = "Windows2003", GroupId = 1, ProviderName = "Windows2003", ProviderType = "FuseCP.Providers.OS.Windows2003, FuseCP.Providers.OS.Windows2003" },
			new Provider() { ProviderId = 4, DisplayName = "MailEnable Server 1.x - 7.x", EditorControl = "MailEnable", GroupId = 4, ProviderName = "MailEnable", ProviderType = "FuseCP.Providers.Mail.MailEnable, FuseCP.Providers.Mail.MailEnable" },
			new Provider() { ProviderId = 7, DisplayName = "Microsoft DNS Server", EditorControl = "MSDNS", GroupId = 7, ProviderName = "MSDNS", ProviderType = "FuseCP.Providers.DNS.MsDNS, FuseCP.Providers.DNS.MsDNS" },
			new Provider() { ProviderId = 8, DisplayName = "AWStats Statistics Service", EditorControl = "AWStats", GroupId = 8, ProviderName = "AWStats", ProviderType = "FuseCP.Providers.Statistics.AWStats, FuseCP.Providers.Statistics.AWStats" },
			new Provider() { ProviderId = 10, DisplayName = "SmarterStats 3.x", EditorControl = "SmarterStats", GroupId = 8, ProviderName = "SmarterStats", ProviderType = "FuseCP.Providers.Statistics.SmarterStats, FuseCP.Providers.Statistics.SmarterS" +
				"tats" },
			new Provider() { ProviderId = 12, DisplayName = "Gene6 FTP Server 3.x", EditorControl = "Gene6FTP", GroupId = 3, ProviderName = "Gene6FTP", ProviderType = "FuseCP.Providers.FTP.Gene6, FuseCP.Providers.FTP.Gene6" },
			new Provider() { ProviderId = 13, DisplayName = "Merak Mail Server 8.0.3 - 9.2.x", EditorControl = "Merak", GroupId = 4, ProviderName = "Merak", ProviderType = "FuseCP.Providers.Mail.Merak, FuseCP.Providers.Mail.Merak" },
			new Provider() { ProviderId = 18, DisplayName = "MDaemon 9.x - 11.x", EditorControl = "MDaemon", GroupId = 4, ProviderName = "MDaemon", ProviderType = "FuseCP.Providers.Mail.MDaemon, FuseCP.Providers.Mail.MDaemon" },
			new Provider() { ProviderId = 19, DisableAutoDiscovery = true, DisplayName = "ArGoSoft Mail Server 1.x", EditorControl = "ArgoMail", GroupId = 4, ProviderName = "ArgoMail",
				ProviderType = "FuseCP.Providers.Mail.ArgoMail, FuseCP.Providers.Mail.ArgoMail" },
			new Provider() { ProviderId = 20, DisplayName = "hMailServer 4.2", EditorControl = "hMailServer", GroupId = 4, ProviderName = "hMailServer", ProviderType = "FuseCP.Providers.Mail.hMailServer, FuseCP.Providers.Mail.hMailServer" },
			new Provider() { ProviderId = 21, DisplayName = "Ability Mail Server 2.x", EditorControl = "AbilityMailServer", GroupId = 4, ProviderName = "AbilityMailServer", ProviderType = "FuseCP.Providers.Mail.AbilityMailServer, FuseCP.Providers.Mail.AbilityMailServ" +
				"er" },
			new Provider() { ProviderId = 22, DisplayName = "hMailServer 4.3", EditorControl = "hMailServer43", GroupId = 4, ProviderName = "hMailServer43", ProviderType = "FuseCP.Providers.Mail.hMailServer43, FuseCP.Providers.Mail.hMailServer43" },
			new Provider() { ProviderId = 24, DisplayName = "ISC BIND 8.x - 9.x", EditorControl = "Bind", GroupId = 7, ProviderName = "Bind", ProviderType = "FuseCP.Providers.DNS.IscBind, FuseCP.Providers.DNS.Bind" },
			new Provider() { ProviderId = 25, DisplayName = "Serv-U FTP 6.x", EditorControl = "ServU", GroupId = 3, ProviderName = "ServU", ProviderType = "FuseCP.Providers.FTP.ServU, FuseCP.Providers.FTP.ServU" },
			new Provider() { ProviderId = 26, DisplayName = "FileZilla FTP Server 0.9", EditorControl = "FileZilla", GroupId = 3, ProviderName = "FileZilla", ProviderType = "FuseCP.Providers.FTP.FileZilla, FuseCP.Providers.FTP.FileZilla" },
			new Provider() { ProviderId = 31, DisplayName = "SmarterStats 4.x", EditorControl = "SmarterStats", GroupId = 8, ProviderName = "SmarterStats", ProviderType = "FuseCP.Providers.Statistics.SmarterStats4, FuseCP.Providers.Statistics.Smarter" +
				"Stats" },
			new Provider() { ProviderId = 56, DisableAutoDiscovery = true, DisplayName = "PowerDNS", EditorControl = "PowerDNS", GroupId = 7, ProviderName = "PowerDNS",
				ProviderType = "FuseCP.Providers.DNS.PowerDNS, FuseCP.Providers.DNS.PowerDNS" },
			new Provider() { ProviderId = 61, DisplayName = "Merak Mail Server 10.x", EditorControl = "Merak", GroupId = 4, ProviderName = "Merak", ProviderType = "FuseCP.Providers.Mail.Merak10, FuseCP.Providers.Mail.Merak10" },
			new Provider() { ProviderId = 62, DisplayName = "SmarterStats 5.x +", EditorControl = "SmarterStats", GroupId = 8, ProviderName = "SmarterStats", ProviderType = "FuseCP.Providers.Statistics.SmarterStats5, FuseCP.Providers.Statistics.Smarter" +
				"Stats" },
			new Provider() { ProviderId = 63, DisplayName = "hMailServer 5.x", EditorControl = "hMailServer5", GroupId = 4, ProviderName = "hMailServer5", ProviderType = "FuseCP.Providers.Mail.hMailServer5, FuseCP.Providers.Mail.hMailServer5" },
			new Provider() { ProviderId = 67, DisplayName = "SmarterMail 100.x +", EditorControl = "SmarterMail100x", GroupId = 4, ProviderName = "SmarterMail", ProviderType = "FuseCP.Providers.Mail.SmarterMail100, FuseCP.Providers.Mail.SmarterMail100" },
			new Provider() { ProviderId = 93, DisplayName = "Hosted Microsoft Exchange Server 2019", EditorControl = "Exchange", GroupId = 12, ProviderName = "Exchange2016", ProviderType = "FuseCP.Providers.HostedSolution.Exchange2019, FuseCP.Providers.HostedSolution." +
				"Exchange2019" },
			//new Provider() { ProviderId = 100, DisplayName = "Windows Server 2008", EditorControl = "Windows2008", GroupId = 1, ProviderName = "Windows2008", ProviderType = "FuseCP.Providers.OS.Windows2008, FuseCP.Providers.OS.Windows2008" },
			new Provider() { ProviderId = 103, DisplayName = "Hosted Organizations", EditorControl = "Organizations", GroupId = 13, ProviderName = "Organizations", ProviderType = "FuseCP.Providers.HostedSolution.OrganizationProvider, FuseCP.Providers.HostedS" +
				"olution" },
			//new Provider() { ProviderId = 104, DisplayName = "Windows Server 2012", EditorControl = "Windows2012", GroupId = 1, ProviderName = "Windows2012", ProviderType = "FuseCP.Providers.OS.Windows2012, FuseCP.Providers.OS.Windows2012" },
			new Provider() { ProviderId = 110, DisplayName = "Cerberus FTP Server 6.x", EditorControl = "CerberusFTP6", GroupId = 3, ProviderName = "CerberusFTP6", ProviderType = "FuseCP.Providers.FTP.CerberusFTP6, FuseCP.Providers.FTP.CerberusFTP6" },
			new Provider() { ProviderId = 111, DisplayName = "Windows Server 2016", EditorControl = "Windows2008", GroupId = 1, ProviderName = "Windows2016", ProviderType = "FuseCP.Providers.OS.Windows2016, FuseCP.Providers.OS.Windows2016" },
			new Provider() { ProviderId = 112, DisplayName = "Internet Information Services 10.0", EditorControl = "IIS70", GroupId = 2, ProviderName = "IIS100", ProviderType = "FuseCP.Providers.Web.IIs100, FuseCP.Providers.Web.IIs100" },
			new Provider() { ProviderId = 113, DisplayName = "Microsoft FTP Server 10.0", EditorControl = "MSFTP70", GroupId = 3, ProviderName = "MSFTP100", ProviderType = "FuseCP.Providers.FTP.MsFTP100, FuseCP.Providers.FTP.IIs100" },
			new Provider() { ProviderId = 160, DisplayName = "IceWarp Mail Server", EditorControl = "IceWarp", GroupId = 4, ProviderName = "IceWarp", ProviderType = "FuseCP.Providers.Mail.IceWarp, FuseCP.Providers.Mail.IceWarp" },
			new Provider() { ProviderId = 320, DisplayName = "MySQL Server 9.0", EditorControl = "MySQL", GroupId = 90, ProviderName = "MySQL", ProviderType = "FuseCP.Providers.Database.MySqlServer90, FuseCP.Providers.Database.MySQL" },
			new Provider() { ProviderId = 352, DisableAutoDiscovery = true, DisplayName = "Microsoft Hyper-V 2016", EditorControl = "HyperV2012R2", GroupId = 33, ProviderName = "HyperV2016",
				ProviderType = "FuseCP.Providers.Virtualization.HyperV2016, FuseCP.Providers.Virtualization.Hy" +
				"perV2016" },
			new Provider() { ProviderId = 370, DisableAutoDiscovery = true, DisplayName = "Proxmox Virtualization (remote)", EditorControl = "Proxmox", GroupId = 167, ProviderName = "Proxmox (remote)",
				ProviderType = "FuseCP.Providers.Virtualization.Proxmoxvps, FuseCP.Providers.Virtualization.Pr" +
				"oxmoxvps" },
			new Provider() { ProviderId = 371, DisableAutoDiscovery = false, DisplayName = "Proxmox Virtualization", EditorControl = "Proxmox", GroupId = 167, ProviderName = "Proxmox",
				ProviderType = "FuseCP.Providers.Virtualization.ProxmoxvpsLocal, FuseCP.Providers.Virtualizati" +
				"on.Proxmoxvps" },
			new Provider() { ProviderId = 500, DisplayName = "Unix System", EditorControl = "Unix", GroupId = 1, ProviderName = "UnixSystem", ProviderType = "FuseCP.Providers.OS.Unix, FuseCP.Providers.OS.Unix" },
			new Provider() { ProviderId = 600, DisplayName = "Enterprise Storage Windows 2012", EditorControl = "EnterpriseStorage", GroupId = 44, ProviderName = "EnterpriseStorage2012", ProviderType = "FuseCP.Providers.EnterpriseStorage.Windows2012, FuseCP.Providers.EnterpriseSto" +
				"rage.Windows2012" },
			new Provider() { ProviderId = 700, DisplayName = "Storage Spaces Windows 2012", EditorControl = "StorageSpaceServices", GroupId = 49, ProviderName = "StorageSpace2012", ProviderType = "FuseCP.Providers.StorageSpaces.Windows2012, FuseCP.Providers.StorageSpaces.Win" +
				"dows2012" },
			new Provider() { ProviderId = 1306, DisplayName = "Hosted SharePoint Foundation 2016", EditorControl = "HostedSharePoint30", GroupId = 20, ProviderName = "HostedSharePoint2016", ProviderType = "FuseCP.Providers.HostedSolution.HostedSharePointServer2016, FuseCP.Providers.H" +
				"ostedSolution.SharePoint2016" },
			new Provider() { ProviderId = 1404, DisplayName = "Microsoft Skype for Business Server 2019", EditorControl = "SfB", GroupId = 52, ProviderName = "SfB2019", ProviderType = "FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB20" +
				"19" },
			new Provider() { ProviderId = 1505, DisableAutoDiscovery = true, DisplayName = "Remote Desktop Services Windows 2025", EditorControl = "RDS", GroupId = 45, ProviderName = "RemoteDesktopServices2025",
				ProviderType = "FuseCP.Providers.RemoteDesktopServices.Windows2025,FuseCP.Providers.RemoteDesk" +
				"topServices.Windows2025" },
			new Provider() { ProviderId = 1586, DisplayName = "MariaDB 11.7", EditorControl = "MariaDB", GroupId = 50, ProviderName = "MariaDB", ProviderType = "FuseCP.Providers.Database.MariaDB117, FuseCP.Providers.Database.MariaDB" },
			new Provider() { ProviderId = 1601, DisableAutoDiscovery = true, DisplayName = "Mail Cleaner", EditorControl = "MailCleaner", GroupId = 61, ProviderName = "MailCleaner",
				ProviderType = "FuseCP.Providers.Filters.MailCleaner, FuseCP.Providers.Filters.MailCleaner" },
			new Provider() { ProviderId = 1602, DisableAutoDiscovery = true, DisplayName = "SpamExperts Mail Filter", EditorControl = "SpamExperts", GroupId = 61, ProviderName = "SpamExperts",
				ProviderType = "FuseCP.Providers.Filters.SpamExperts, FuseCP.Providers.Filters.SpamExperts" },
			new Provider() { ProviderId = 1701, DisplayName = "Microsoft SQL Server 2016", EditorControl = "MSSQL", GroupId = 71, ProviderName = "MsSQL", ProviderType = "FuseCP.Providers.Database.MsSqlServer2016, FuseCP.Providers.Database.SqlServer" },
			new Provider() { ProviderId = 1702, DisplayName = "Hosted SharePoint Enterprise 2016", EditorControl = "HostedSharePoint30", GroupId = 73, ProviderName = "HostedSharePoint2016Ent", ProviderType = "FuseCP.Providers.HostedSolution.HostedSharePointServer2016Ent, FuseCP.Provider" +
				"s.HostedSolution.SharePoint2016Ent" },
			new Provider() { ProviderId = 1704, DisplayName = "Microsoft SQL Server 2017", EditorControl = "MSSQL", GroupId = 72, ProviderName = "MsSQL",
				ProviderType = "FuseCP.Providers.Database.MsSqlServer2017, FuseCP.Providers.Database.SqlServer" },
			new Provider() { ProviderId = 1705, DisplayName = "Microsoft SQL Server 2019", EditorControl = "MSSQL", GroupId = 74, ProviderName = "MsSQL",
				ProviderType = "FuseCP.Providers.Database.MsSqlServer2019, FuseCP.Providers.Database.SqlServer" },
			new Provider() { ProviderId = 1706, DisplayName = "Microsoft SQL Server 2022", EditorControl = "MSSQL", GroupId = 75, ProviderName = "MsSQL", ProviderType = "FuseCP.Providers.Database.MsSqlServer2022, FuseCP.Providers.Database.SqlServer" },
			new Provider() { ProviderId = 1707, DisplayName = "Microsoft SQL Server 2025", EditorControl = "MSSQL", GroupId = 76, ProviderName = "MsSQL", ProviderType = "FuseCP.Providers.Database.MsSqlServer2025, FuseCP.Providers.Database.SqlServer" },
			new Provider() { ProviderId = 1711, DisplayName = "Hosted SharePoint 2019", EditorControl = "HostedSharePoint30", GroupId = 73, ProviderName = "HostedSharePoint2019", ProviderType = "FuseCP.Providers.HostedSolution.HostedSharePointServer2019, FuseCP.Providers.H" +
				"ostedSolution.SharePoint2019" },
			new Provider() { ProviderId = 1800, DisplayName = "Windows Server 2019", EditorControl = "Windows2012", GroupId = 1, ProviderName = "Windows2019", ProviderType = "FuseCP.Providers.OS.Windows2019, FuseCP.Providers.OS.Windows2019" },
			new Provider() { ProviderId = 1801, DisableAutoDiscovery = true, DisplayName = "Microsoft Hyper-V 2019", EditorControl = "HyperV2012R2", GroupId = 33, ProviderName = "HyperV2019",
				ProviderType = "FuseCP.Providers.Virtualization.HyperV2019, FuseCP.Providers.Virtualization.Hy" +
				"perV2019" },
			new Provider() { ProviderId = 1802, DisplayName = "Windows Server 2022", EditorControl = "Windows2012", GroupId = 1, ProviderName = "Windows2022", ProviderType = "FuseCP.Providers.OS.Windows2022, FuseCP.Providers.OS.Windows2022" },
			new Provider() { ProviderId = 1803, DisableAutoDiscovery = true, DisplayName = "Microsoft Hyper-V 2022", EditorControl = "HyperV2012R2", GroupId = 33, ProviderName = "HyperV2022",
				ProviderType = "FuseCP.Providers.Virtualization.HyperV2022, FuseCP.Providers.Virtualization.Hy" +
				"perV2022" },
			new Provider() { ProviderId = 1804, DisplayName = "Windows Server 2025", EditorControl = "Windows2012", GroupId = 1, ProviderName = "Windows2025",
				ProviderType = "FuseCP.Providers.OS.Windows2025, FuseCP.Providers.OS.Windows2025" },
			new Provider() { ProviderId = 1805, DisableAutoDiscovery = true, DisplayName = "Microsoft Hyper-V 2025", EditorControl = "HyperV2012R2", GroupId = 33, ProviderName = "HyperV2025",
				ProviderType = "FuseCP.Providers.Virtualization.HyperV2025, FuseCP.Providers.Virtualization.Hy" +
				"perV2025" },
			new Provider() { ProviderId = 1902, DisplayName = "MsDNSPS", EditorControl = "MSDNS", GroupId = 7, ProviderName = "MsDNSPS", ProviderType = "FuseCP.Providers.DNS.MsDNSPS, FuseCP.Providers.DNS.MsDNSPS" },
			new Provider() { ProviderId = 1903, DisplayName = "SimpleDNS Plus 9.x", EditorControl = "SimpleDNS", GroupId = 7, ProviderName = "SimpleDNS", ProviderType = "FuseCP.Providers.DNS.SimpleDNS9, FuseCP.Providers.DNS.SimpleDNS90" },
			new Provider() { ProviderId = 1910, DisplayName = "vsftpd FTP Server 3", EditorControl = "vsftpd", GroupId = 3, ProviderName = "vsftpd", ProviderType = "FuseCP.Providers.FTP.VsFtp3, FuseCP.Providers.FTP.VsFtp" },
			new Provider() { ProviderId = 1911, DisplayName = "Apache Web Server 2.4", EditorControl = "Apache", GroupId = 2, ProviderName = "Apache", ProviderType = "FuseCP.Providers.Web.Apache24, FuseCP.Providers.Web.Apache" }
		});
		#endregion
	}
}
