using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemoveDeprecatedProvidersExchangeAndSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1501);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 5, null, "Microsoft SQL Server 2000", "MSSQL", 5, "MSSQL", "FuseCP.Providers.Database.MsSqlServer, FuseCP.Providers.Database.SqlServer" },
                    { 16, null, "Microsoft SQL Server 2005", "MSSQL", 10, "MSSQL", "FuseCP.Providers.Database.MsSqlServer2005, FuseCP.Providers.Database.SqlServer" },
                    { 27, null, "Hosted Microsoft Exchange Server 2007", "Exchange", 12, "Exchange2007", "FuseCP.Providers.HostedSolution.Exchange2007, FuseCP.Providers.HostedSolution" },
                    { 32, null, "Hosted Microsoft Exchange Server 2010", "Exchange", 12, "Exchange2010", "FuseCP.Providers.HostedSolution.Exchange2010, FuseCP.Providers.HostedSolution" },
                    { 90, null, "Hosted Microsoft Exchange Server 2010 SP2", "Exchange", 12, "Exchange2010SP2", "FuseCP.Providers.HostedSolution.Exchange2010SP2, FuseCP.Providers.HostedSolution" },
                    { 202, null, "Microsoft SQL Server 2008", "MSSQL", 22, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2008, FuseCP.Providers.Database.SqlServer" },
                    { 209, null, "Microsoft SQL Server 2012", "MSSQL", 23, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2012, FuseCP.Providers.Database.SqlServer" },
                    { 1203, null, "Microsoft SQL Server 2014", "MSSQL", 46, "MsSQL", "FuseCP.Providers.Database.MsSqlServer2014, FuseCP.Providers.Database.SqlServer" },
                    { 1501, true, "Remote Desktop Services Windows 2012", "RDS", 45, "RemoteDesktopServices2012", "FuseCP.Providers.RemoteDesktopServices.Windows2012,FuseCP.Providers.RemoteDesktopServices.Windows2012" }
                });
        }
    }
}
