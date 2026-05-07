using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.MySql
{
    /// <inheritdoc />
    public partial class RemoveLegacyCommunicationsProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM `GlobalDnsRecords` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `PackageServices` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `ServiceItems` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `ServiceProperties` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `StorageSpaces` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `VirtualServices` WHERE `ServiceID` IN (SELECT `ServiceID` FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404));");
            migrationBuilder.Sql("DELETE FROM `Services` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404);");
            migrationBuilder.Sql("DELETE FROM `ServiceDefaultProperties` WHERE `ProviderID` IN (205, 206, 250, 1401, 1402, 1403, 1404);");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 250);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1401);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1402);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1403);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1404);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 205, true, "Office Communications Server 2007 R2", "OCS", 32, "OCS", "FuseCP.Providers.HostedSolution.OCS2007R2, FuseCP.Providers.HostedSolution" },
                    { 206, true, "OCS Edge server", "OCS_Edge", 32, "OCSEdge", "FuseCP.Providers.HostedSolution.OCSEdge2007R2, FuseCP.Providers.HostedSolution" },
                    { 250, null, "Microsoft Lync Server 2010 Multitenant Hosting Pack", "Lync", 41, "Lync2010", "FuseCP.Providers.HostedSolution.Lync2010, FuseCP.Providers.HostedSolution" },
                    { 1401, null, "Microsoft Lync Server 2013 Enterprise Edition", "Lync", 41, "Lync2013", "FuseCP.Providers.HostedSolution.Lync2013, FuseCP.Providers.HostedSolution.Lync2013" },
                    { 1402, null, "Microsoft Lync Server 2013 Multitenant Hosting Pack", "Lync", 41, "Lync2013HP", "FuseCP.Providers.HostedSolution.Lync2013HP, FuseCP.Providers.HostedSolution.Lync2013HP" },
                    { 1403, null, "Microsoft Skype for Business Server 2015", "SfB", 52, "SfB2015", "FuseCP.Providers.HostedSolution.SfB2015, FuseCP.Providers.HostedSolution.SfB2015" },
                    { 1404, null, "Microsoft Skype for Business Server 2019", "SfB", 52, "SfB2019", "FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB2019" }
                });
        }
    }
}
