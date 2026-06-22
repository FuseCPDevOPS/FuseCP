using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class ConsolidateMsDnsProvidersToMsDnsPs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "admode", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "expirelimit", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "minimumttl", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "nameservers", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RecordDefaultTTL", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RecordMinimumTTL", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "refreshinterval", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "responsibleperson", 410 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "retrydelay", 410 });

            migrationBuilder.Sql("UPDATE [Services] SET [ProviderID] = 1902 WHERE [ProviderID] = 410;");
            migrationBuilder.Sql("UPDATE [ServiceDefaultProperties] SET [ProviderID] = 1902 WHERE [ProviderID] = 410;");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 410);

            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1902,
                columns: new[] { "DisplayName", "ProviderName", "ProviderType" },
                values: new object[] { "MsDNSPS", "MsDNSPS", "FuseCP.Providers.DNS.MsDNSPS, FuseCP.Providers.DNS.MsDNSPS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1902,
                columns: new[] { "DisplayName", "ProviderName", "ProviderType" },
                values: new object[] { "Microsoft DNS Server 2016", "MSDNS.2016", "FuseCP.Providers.DNS.MsDNS2016, FuseCP.Providers.DNS.MsDNS2016" });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[] { 410, null, "Microsoft DNS Server 2012+", "MSDNS", 7, "MSDNS.2012", "FuseCP.Providers.DNS.MsDNS2012, FuseCP.Providers.DNS.MsDNS2012" });

            migrationBuilder.InsertData(
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "admode", 410, "False" },
                    { "expirelimit", 410, "1209600" },
                    { "minimumttl", 410, "86400" },
                    { "nameservers", 410, "ns1.yourdomain.com;ns2.yourdomain.com" },
                    { "RecordDefaultTTL", 410, "86400" },
                    { "RecordMinimumTTL", 410, "3600" },
                    { "refreshinterval", 410, "3600" },
                    { "responsibleperson", 410, "hostmaster.[DOMAIN_NAME]" },
                    { "retrydelay", 410, "600" }
                });
        }
    }
}
