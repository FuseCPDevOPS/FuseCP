using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.MySql
{
    /// <inheritdoc />
    public partial class RemoveLegacyHostedSharePoint30And2010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remap ServiceDefaultProperties to SharePoint Foundation 2016 (Provider 1306)
            migrationBuilder.Sql(@"INSERT INTO `ServiceDefaultProperties` (`PropertyName`, `ProviderID`, `PropertyValue`)
SELECT sdp.`PropertyName`, 1306, sdp.`PropertyValue`
FROM `ServiceDefaultProperties` sdp
WHERE sdp.`ProviderID` IN (200, 208)
  AND NOT EXISTS (
      SELECT 1
      FROM `ServiceDefaultProperties` dst
      WHERE dst.`ProviderID` = 1306 AND dst.`PropertyName` = sdp.`PropertyName`
  );");

            migrationBuilder.Sql(@"DELETE FROM `ServiceDefaultProperties` WHERE `ProviderID` IN (200, 208);");

            // Remap services to SharePoint Foundation 2016 (Provider 1306)
            migrationBuilder.Sql(@"UPDATE `Services` SET `ProviderID` = 1306 WHERE `ProviderID` IN (200, 208);");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 208);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 200, null, "Hosted Windows SharePoint Services 3.0", "HostedSharePoint30", 20, "HostedSharePoint30", "FuseCP.Providers.HostedSolution.HostedSharePointServer, FuseCP.Providers.HostedSolution" },
                    { 208, null, "Hosted SharePoint Foundation 2010", "HostedSharePoint30", 20, "HostedSharePoint2010", "FuseCP.Providers.HostedSolution.HostedSharePointServer2010, FuseCP.Providers.HostedSolution" }
                });
        }
    }
}
