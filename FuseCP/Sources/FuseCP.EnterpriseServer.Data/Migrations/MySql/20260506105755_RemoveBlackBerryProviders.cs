using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.MySql
{
    /// <inheritdoc />
    public partial class RemoveBlackBerryProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "UserName", 204 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "UtilityPath", 204 });

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 204);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 203, true, "BlackBerry 4.1", "BlackBerry", 31, "BlackBerry 4.1", "FuseCP.Providers.HostedSolution.BlackBerryProvider, FuseCP.Providers.HostedSolution" },
                    { 204, true, "BlackBerry 5.0", "BlackBerry5", 31, "BlackBerry 5.0", "FuseCP.Providers.HostedSolution.BlackBerry5Provider, FuseCP.Providers.HostedSolution" }
                });

            migrationBuilder.InsertData(
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "UserName", 204, "admin" },
                    { "UtilityPath", 204, "C:\\Program Files\\Research In Motion\\BlackBerry Enterprise Server Resource Kit\\BlackBerry Enterprise Server User Administration Tool" }
                });
        }
    }
}
