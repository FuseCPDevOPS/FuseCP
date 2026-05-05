using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class RemoveDeprecatedProvidersAllSqlTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 60);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 64);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 65);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 66);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 91);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 92);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 101);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 102);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 105);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 106);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 201);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 300);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 350);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 351);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 400);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1201);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1202);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1205);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1206);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1703);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1901);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 209);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 210);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 460);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 461);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 462);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 463);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 464);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 465);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 466);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Quotas",
                keyColumn: "QuotaID",
                keyValue: 467);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "CRM_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RecordDefaultTTL", 55 });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RecordMinimumTTL", 55 });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ResourceGroups",
                keyColumn: "GroupID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ResourceGroups",
                keyColumn: "GroupID",
                keyValue: 24);

            migrationBuilder.CreateTable(
                name: "BruteForceLogs",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Layer = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Succeeded = table.Column<bool>(type: "boolean", nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BruteForceLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpSecurityPolicies",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IpRange = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsWhitelist = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SeverityLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpSecurityPolicy", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BruteForceLogs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "IpSecurityPolicies",
                schema: "public");

            migrationBuilder.InsertData(
                schema: "public",
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 2, null, "Internet Information Services 6.0", "IIS60", 2, "IIS60", "FuseCP.Providers.Web.IIs60, FuseCP.Providers.Web.IIs60" },
                    { 3, null, "Microsoft FTP Server 6.0", "MSFTP60", 3, "MSFTP60", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs60" },
                    { 9, null, "SimpleDNS Plus 4.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS, FuseCP.Providers.DNS.SimpleDNS" },
                    { 11, null, "SmarterMail 2.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail2, FuseCP.Providers.Mail.SmarterMail2" },
                    { 14, null, "SmarterMail 3.x - 4.x", "SmarterMail", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail3, FuseCP.Providers.Mail.SmarterMail3" },
                    { 28, null, "SimpleDNS Plus 5.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS5, FuseCP.Providers.DNS.SimpleDNS50" },
                    { 29, null, "SmarterMail 5.x", "SmarterMail50", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail5, FuseCP.Providers.Mail.SmarterMail5" },
                    { 60, null, "SmarterMail 6.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail6, FuseCP.Providers.Mail.SmarterMail6" },
                    { 64, null, "SmarterMail 7.x - 8.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail7, FuseCP.Providers.Mail.SmarterMail7" },
                    { 65, null, "SmarterMail 9.x", "SmarterMail60", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail9, FuseCP.Providers.Mail.SmarterMail9" },
                    { 66, null, "SmarterMail 10.x +", "SmarterMail100", 4, "SmarterMail", "FuseCP.Providers.Mail.SmarterMail10, FuseCP.Providers.Mail.SmarterMail10" },
                    { 91, null, "Hosted Microsoft Exchange Server 2013", "Exchange", 12, "Exchange2013", "FuseCP.Providers.HostedSolution.Exchange2013, FuseCP.Providers.HostedSolution.Exchange2013" },
                    { 92, null, "Hosted Microsoft Exchange Server 2016", "Exchange", 12, "Exchange2016", "FuseCP.Providers.HostedSolution.Exchange2016, FuseCP.Providers.HostedSolution.Exchange2016" },
                    { 101, null, "Internet Information Services 7.0", "IIS70", 2, "IIS70", "FuseCP.Providers.Web.IIs70, FuseCP.Providers.Web.IIs70" },
                    { 102, null, "Microsoft FTP Server 7.0", "MSFTP70", 3, "MSFTP70", "FuseCP.Providers.FTP.MsFTP, FuseCP.Providers.FTP.IIs70" },
                    { 105, null, "Internet Information Services 8.0", "IIS70", 2, "IIS80", "FuseCP.Providers.Web.IIs80, FuseCP.Providers.Web.IIs80" },
                    { 106, null, "Microsoft FTP Server 8.0", "MSFTP70", 3, "MSFTP80", "FuseCP.Providers.FTP.MsFTP80, FuseCP.Providers.FTP.IIs80" },
                    { 300, true, "Microsoft Hyper-V", "HyperV", 30, "HyperV", "FuseCP.Providers.Virtualization.HyperV, FuseCP.Providers.Virtualization.HyperV" },
                    { 350, true, "Microsoft Hyper-V 2012 R2", "HyperV2012R2", 33, "HyperV2012R2", "FuseCP.Providers.Virtualization.HyperV2012R2, FuseCP.Providers.Virtualization.HyperV2012R2" },
                    { 351, true, "Microsoft Hyper-V Virtual Machine Management", "HyperVvmm", 33, "HyperVvmm", "FuseCP.Providers.Virtualization.HyperVvmm, FuseCP.Providers.Virtualization.HyperVvmm" },
                    { 400, true, "Microsoft Hyper-V For Private Cloud", "HyperVForPrivateCloud", 40, "HyperVForPC", "FuseCP.Providers.VirtualizationForPC.HyperVForPC, FuseCP.Providers.VirtualizationForPC.HyperVForPC" },
                    { 1703, null, "SimpleDNS Plus 6.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS6, FuseCP.Providers.DNS.SimpleDNS60" },
                    { 1901, null, "SimpleDNS Plus 8.x", "SimpleDNS", 7, "SimpleDNS", "FuseCP.Providers.DNS.SimpleDNS8, FuseCP.Providers.DNS.SimpleDNS80" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ResourceGroups",
                columns: new[] { "GroupID", "GroupController", "GroupName", "GroupOrder", "ShowGroup" },
                values: new object[,]
                {
                    { 21, null, "Hosted CRM", 16, true },
                    { 24, null, "Hosted CRM2013", 16, true }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ScheduleTaskParameters",
                columns: new[] { "ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder" },
                values: new object[] { "CRM_REPORT", "SCHEDULE_TASK_HOSTED_SOLUTION_REPORT", "Boolean", "true", 6 });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "RecordDefaultTTL", 55, "86400" },
                    { "RecordMinimumTTL", 55, "3600" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 201, null, "Hosted MS CRM 4.0", "CRM", 21, "CRM", "FuseCP.Providers.HostedSolution.CRMProvider, FuseCP.Providers.HostedSolution" },
                    { 1201, null, "Hosted MS CRM 2011", "CRM2011", 21, "CRM", "FuseCP.Providers.HostedSolution.CRMProvider2011, FuseCP.Providers.HostedSolution.CRM2011" },
                    { 1202, null, "Hosted MS CRM 2013", "CRM2011", 24, "CRM", "FuseCP.Providers.HostedSolution.CRMProvider2013, FuseCP.Providers.HostedSolution.Crm2013" },
                    { 1205, null, "Hosted MS CRM 2015", "CRM2011", 24, "CRM", "FuseCP.Providers.HostedSolution.CRMProvider2015, FuseCP.Providers.HostedSolution.Crm2015" },
                    { 1206, null, "Hosted MS CRM 2016", "CRM2011", 24, "CRM", "FuseCP.Providers.HostedSolution.CRMProvider2016, FuseCP.Providers.HostedSolution.Crm2016" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Quotas",
                columns: new[] { "QuotaID", "GroupID", "HideQuota", "ItemTypeID", "PerOrganization", "QuotaDescription", "QuotaName", "QuotaOrder", "QuotaTypeID", "ServiceQuota" },
                values: new object[,]
                {
                    { 209, 21, null, null, 1, "Full licenses per organization", "HostedCRM.Users", 2.0, 3, false },
                    { 210, 21, null, null, null, "CRM Organization", "HostedCRM.Organization", 1.0, 1, false },
                    { 460, 21, null, null, null, "Max Database Size, MB", "HostedCRM.MaxDatabaseSize", 5.0, 3, false },
                    { 461, 21, null, null, 1, "Limited licenses per organization", "HostedCRM.LimitedUsers", 3.0, 3, false },
                    { 462, 21, null, null, 1, "ESS licenses per organization", "HostedCRM.ESSUsers", 4.0, 3, false },
                    { 463, 24, null, null, null, "CRM Organization", "HostedCRM2013.Organization", 1.0, 1, false },
                    { 464, 24, null, null, null, "Max Database Size, MB", "HostedCRM2013.MaxDatabaseSize", 5.0, 3, false },
                    { 465, 24, null, null, 1, "Essential licenses per organization", "HostedCRM2013.EssentialUsers", 2.0, 3, false },
                    { 466, 24, null, null, 1, "Basic licenses per organization", "HostedCRM2013.BasicUsers", 3.0, 3, false },
                    { 467, 24, null, null, 1, "Professional licenses per organization", "HostedCRM2013.ProfessionalUsers", 4.0, 3, false }
                });
        }
    }
}
