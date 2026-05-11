using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class RemapLegacySpsProvidersToSharePoint2016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remap ServiceDefaultProperties to SharePoint Foundation 2016 (Provider 1306).
            migrationBuilder.Sql(@"INSERT INTO public.""ServiceDefaultProperties"" (""PropertyName"", ""ProviderID"", ""PropertyValue"")
SELECT sdp.""PropertyName"", 1306, sdp.""PropertyValue""
FROM public.""ServiceDefaultProperties"" sdp
WHERE sdp.""ProviderID"" IN (15, 23)
  AND NOT EXISTS (
      SELECT 1
      FROM public.""ServiceDefaultProperties"" dst
      WHERE dst.""ProviderID"" = 1306 AND dst.""PropertyName"" = sdp.""PropertyName""
  );");

            migrationBuilder.Sql(@"DELETE FROM public.""ServiceDefaultProperties"" WHERE ""ProviderID"" IN (15, 23);");

            // Remap services to SharePoint Foundation 2016 (Provider 1306).
            migrationBuilder.Sql(@"UPDATE public.""Services"" SET ""ProviderID"" = 1306 WHERE ""ProviderID"" IN (15, 23);");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 23);
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
                    { 15, true, "Windows SharePoint Services 2.0", "Sps20", 9, "Sps20", "FuseCP.Providers.SharePoint.Sps20, FuseCP.Providers.SharePoint.Sps20" },
                    { 23, null, "Windows SharePoint Services 3.0", "Sps20", 9, "Sps20", "FuseCP.Providers.SharePoint.Sps30, FuseCP.Providers.SharePoint.Sps30" }
                });
        }
    }
}
