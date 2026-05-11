using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemoveSharePoint2013Providers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remap ServiceDefaultProperties to SharePoint Foundation 2016 (Provider 1306)
            migrationBuilder.Sql(@"INSERT INTO ""ServiceDefaultProperties"" (""PropertyName"", ""ProviderID"", ""PropertyValue"")
SELECT sdp.""PropertyName"", 1306, sdp.""PropertyValue""
FROM ""ServiceDefaultProperties"" sdp
WHERE sdp.""ProviderID"" IN (1301, 1552)
  AND NOT EXISTS (
      SELECT 1
      FROM ""ServiceDefaultProperties"" dst
      WHERE dst.""ProviderID"" = 1306 AND dst.""PropertyName"" = sdp.""PropertyName""
  );");

            migrationBuilder.Sql(@"DELETE FROM ""ServiceDefaultProperties"" WHERE ""ProviderID"" IN (1301, 1552);");

            // Remap services to SharePoint Foundation 2016 (Provider 1306)
            migrationBuilder.Sql(@"UPDATE ""Services"" SET ""ProviderID"" = 1306 WHERE ""ProviderID"" IN (1301, 1552);");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1552);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 1301, null, "Hosted SharePoint Foundation 2013", "HostedSharePoint30", 20, "HostedSharePoint2013", "FuseCP.Providers.HostedSolution.HostedSharePointServer2013, FuseCP.Providers.HostedSolution.SharePoint2013" },
                    { 1552, null, "Hosted SharePoint Enterprise 2013", "HostedSharePoint30", 73, "HostedSharePoint2013Ent", "FuseCP.Providers.HostedSolution.HostedSharePointServer2013Ent, FuseCP.Providers.HostedSolution.SharePoint2013Ent" }
                });
        }
    }
}
