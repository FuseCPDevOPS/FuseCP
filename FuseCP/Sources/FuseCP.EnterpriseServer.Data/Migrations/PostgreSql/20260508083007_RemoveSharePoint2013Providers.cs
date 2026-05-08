using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class RemoveSharePoint2013Providers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1552);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "public",
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
