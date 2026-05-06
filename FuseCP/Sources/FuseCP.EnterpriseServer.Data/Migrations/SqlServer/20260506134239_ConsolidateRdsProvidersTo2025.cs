using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class ConsolidateRdsProvidersTo2025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Services] SET [ProviderID] = 1505 WHERE [ProviderID] IN (1501, 1502, 1503, 1504);");

            migrationBuilder.Sql("DELETE FROM [Providers] WHERE [ProviderID] = 1501;");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1502);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1503);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1504);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 1502, true, "Remote Desktop Services Windows 2016", "RDS", 45, "RemoteDesktopServices2012", "FuseCP.Providers.RemoteDesktopServices.Windows2016,FuseCP.Providers.RemoteDesktopServices.Windows2016" },
                    { 1503, true, "Remote Desktop Services Windows 2019", "RDS", 45, "RemoteDesktopServices2019", "FuseCP.Providers.RemoteDesktopServices.Windows2019,FuseCP.Providers.RemoteDesktopServices.Windows2019" },
                    { 1504, true, "Remote Desktop Services Windows 2022", "RDS", 45, "RemoteDesktopServices2022", "FuseCP.Providers.RemoteDesktopServices.Windows2022,FuseCP.Providers.RemoteDesktopServices.Windows2022" }
                });
        }
    }
}
