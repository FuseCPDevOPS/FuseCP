using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class ConsolidateSharePointEnterpriseProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [ServiceDefaultProperties] ([PropertyName], [ProviderID], [PropertyValue])
SELECT sdp.[PropertyName], 1711, sdp.[PropertyValue]
FROM [ServiceDefaultProperties] sdp
WHERE sdp.[ProviderID] = 1702
  AND NOT EXISTS (
      SELECT 1
      FROM [ServiceDefaultProperties] dst
      WHERE dst.[ProviderID] = 1711 AND dst.[PropertyName] = sdp.[PropertyName]
  );");

            migrationBuilder.Sql(@"DELETE FROM [ServiceDefaultProperties] WHERE [ProviderID] = 1702;");
            migrationBuilder.Sql(@"UPDATE [Services] SET [ProviderID] = 1711 WHERE [ProviderID] = 1702;");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1702);

            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1711,
                columns: new[] { "DisplayName", "ProviderName" },
                values: new object[] { "SharePoint Enterprise", "SharepointEnterprise" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1711,
                columns: new[] { "DisplayName", "ProviderName" },
                values: new object[] { "Hosted SharePoint 2019", "HostedSharePoint2019" });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[] { 1702, null, "Hosted SharePoint Enterprise 2016", "HostedSharePoint30", 73, "HostedSharePoint2016Ent", "FuseCP.Providers.HostedSolution.HostedSharePointServer2016Ent, FuseCP.Providers.HostedSolution.SharePoint2016Ent" });
        }
    }
}
