using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class RemoveDeprecatedProvidersExchangeAndSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 300);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 350);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 351);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 400);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1501);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1703);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1901);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 2, null, "Internet Information Services 6.0", "IIS60", 2, "IIS60", "FuseCP.Providers.Web.IIs60, FuseCP.Providers.Web.IIs60" },
                    { 3, null, "Microsoft FTP Server 6.0", "MSFTP60", 3, "MSFTP60", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs60" },
                    { 5, null, "Microsoft SQL Server 2000", "MSSQL", 5, "MSSQL", "FuseCP.Providers.Database.MsSqlServer, FuseCP.Providers.Database.SqlServer" },
                    { 9, null, "SimpleDNS Plus 4.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS, FuseCP.Providers.DNS.SimpleDNS" },
                    { 11, null, "SmarterMail 2.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail2, FuseCP.Providers.Mail.SmarterMail2" },
                    { 14, null, "SmarterMail 3.x - 4.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail3, FuseCP.Providers.Mail.SmarterMail3" },
                    { 16, null, "Microsoft SQL Server 2005", "MSSQL", 10, "MSSQL", "FuseCP.Providers.Database.MsSqlServer2005, FuseCP.Providers.Database.SqlServer" },
                    { 27, null, "Hosted Microsoft Exchange Server 2007", "Exchange", 12, "Exchange2007", "FuseCP.Providers.HostedSolution.Exchange2007, FuseCP.Providers.HostedSolution" },
                    { 28, null, "SimpleDNS Plus 5.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS5, FuseCP.Providers.DNS.SimpleDNS50" },
                    { 29, null, "SmarterMail 5.x", "SmarterMail50", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail5, FuseCP.Providers.Mail.SmarterMail5" },
                    { 32, null, "Hosted Microsoft Exchange Server 2010", "Exchange", 12, "Exchange2010", "FuseCP.Providers.HostedSolution.Exchange2010, FuseCP.Providers.HostedSolution" },
                    { 60, null, "SmarterMail 6.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail6, FuseCP.Providers.Mail.SmarterMail6" },
                    { 64, null, "SmarterMail 7.x - 8.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail7, FuseCP.Providers.Mail.SmarterMail7" },
                    { 65, null, "SmarterMail 9.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail9, FuseCP.Providers.Mail.SmarterMail9" },
                    { 66, null, "SmarterMail 10.x +", "SmarterMail100", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail10, FuseCP.Providers.Mail.SmarterMail10" },
                    { 90, null, "Hosted Microsoft Exchange Server 2010 SP2", "Exchange", 12, "Exchange2010SP2", "FuseCP.Providers.HostedSolution.Exchange2010SP2, FuseCP.Providers.HostedSolution" },
                    { 91, null, "Hosted Microsoft Exchange Server 2013", "Exchange", 12, "Exchange2013", "FuseCP.Providers.HostedSolution.Exchange2013, FuseCP.Providers.HostedSolution.Exchange2013" },
                    { 92, null, "Hosted Microsoft Exchange Server 2016", "Exchange", 12, "Exchange2016", "FuseCP.Providers.HostedSolution.Exchange2016, FuseCP.Providers.HostedSolution.Exchange2016" },
                    { 101, null, "Internet Information Services 7.0", "IIS70", 2, "IIS70", "FuseCP.Providers.Web.IIs70, FuseCP.Providers.Web.IIs70" },
                    { 102, null, "Microsoft FTP Server 7.0", "MSFTP70", 3, "MSFTP70", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs70" },
                    { 105, null, "Internet Information Services 8.0", "IIS70", 2, "IIS80", "FuseCP.Providers.Web.IIs80, FuseCP.Providers.Web.IIs80" },
                    { 106, null, "Microsoft FTP Server 8.0", "MSFTP70", 3, "MSFTP80", "FuseCP.Providers.FTP.MsFTP80, FuseCP.Providers.FTP.IIs80" },
                    { 202, null, "Microsoft SQL Server 2008", "MSSQL", 22, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2008, FuseCP.Providers.Database.SqlServer" },
                    { 209, null, "Microsoft SQL Server 2012", "MSSQL", 23, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2012, FuseCP.Providers.Database.SqlServer" },
                    { 300, true, "Microsoft Hyper-V", "HyperV", 30, "HyperV", "FuseCP.Providers.Virtualization.HyperV, FuseCP.Providers.Virtualization.HyperV" },
                    { 350, true, "Microsoft Hyper-V 2012 R2", "HyperV2012R2", 33, "HyperV2012R2", "FuseCP.Providers.Virtualization.HyperV2012R2, FuseCP.Providers.Virtualization.HyperV2012R2" },
                    { 351, true, "Microsoft Hyper-V Virtual Machine Management", "HyperVvmm", 33, "HyperVvmm", "FuseCP.Providers.Virtualization.HyperVvmm, FuseCP.Providers.Virtualization.HyperVvmm" },
                    { 400, true, "Microsoft Hyper-V For Private Cloud", "HyperVForPrivateCloud", 40, "HyperVForPC", "FuseCP.Providers.VirtualizationForPC.HyperVForPC, FuseCP.Providers.VirtualizationForPC.HyperVForPC" },
                    { 1203, null, "Microsoft SQL Server 2014", "MSSQL", 46, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2014, FuseCP.Providers.Database.SqlServer" },
                    { 1501, true, "Remote Desktop Services Windows 2012", "RDS", 45, "RemoteDesktopServices2012", "FuseCP.Providers.RemoteDesktopServices.Windows2012,FuseCP.Providers.RemoteDesktopServices.Windows2012" },
                    { 1703, null, "SimpleDNS Plus 6.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS6, FuseCP.Providers.DNS.SimpleDNS60" },
                    { 1901, null, "SimpleDNS Plus 8.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS8, FuseCP.Providers.DNS.SimpleDNS80" }
                });
        }
    }
}
