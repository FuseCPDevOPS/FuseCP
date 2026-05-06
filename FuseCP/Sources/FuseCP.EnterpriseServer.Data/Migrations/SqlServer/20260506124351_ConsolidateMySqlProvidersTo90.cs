using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class ConsolidateMySqlProvidersTo90 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Services] SET [ProviderID] = 320 WHERE [ProviderID] IN (6, 17, 30, 301, 302, 303, 304, 305, 306, 307, 308);");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 302);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 303);

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 6 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 6 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 6 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 6 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 6 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 17 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 17 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 17 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 17 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 17 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 30 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 30 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 30 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 30 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 30 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 301 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 301 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 301 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 301 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 301 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "sslmode", 304 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "sslmode", 305 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "sslmode", 306 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "sslmode", 307 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "ExternalAddress", 308 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InstallFolder", 308 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "InternalAddress", 308 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootLogin", 308 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "RootPassword", 308 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "sslmode", 308 });

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 301);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 304);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 305);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 306);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 307);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 308);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 6, null, "MySQL Server 4.x", "MySQL", 6, "MySQL", "FuseCP.Providers.Database.MySqlServer, FuseCP.Providers.Database.MySQL" },
                    { 17, null, "MySQL Server 5.0", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer50, FuseCP.Providers.Database.MySQL" },
                    { 30, null, "MySQL Server 5.1", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer51, FuseCP.Providers.Database.MySQL" },
                    { 301, null, "MySQL Server 5.5", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer55, FuseCP.Providers.Database.MySQL" },
                    { 302, null, "MySQL Server 5.6", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer56, FuseCP.Providers.Database.MySQL" },
                    { 303, null, "MySQL Server 5.7", "MySQL", 11, "MySQL", "FuseCP.Providers.Database.MySqlServer57, FuseCP.Providers.Database.MySQL" },
                    { 304, null, "MySQL Server 8.0", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer80, FuseCP.Providers.Database.MySQL" },
                    { 305, null, "MySQL Server 8.1", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer81, FuseCP.Providers.Database.MySQL" },
                    { 306, null, "MySQL Server 8.2", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer82, FuseCP.Providers.Database.MySQL" },
                    { 307, null, "MySQL Server 8.3", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer83, FuseCP.Providers.Database.MySQL" },
                    { 308, null, "MySQL Server 8.4", "MySQL", 90, "MySQL", "FuseCP.Providers.Database.MySqlServer84, FuseCP.Providers.Database.MySQL" }
                });

            migrationBuilder.InsertData(
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "ExternalAddress", 6, "localhost" },
                    { "InstallFolder", 6, "%PROGRAMFILES%\\MySQL\\MySQL Server 4.1" },
                    { "InternalAddress", 6, "localhost,3306" },
                    { "RootLogin", 6, "root" },
                    { "RootPassword", 6, "" },
                    { "ExternalAddress", 17, "localhost" },
                    { "InstallFolder", 17, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.0" },
                    { "InternalAddress", 17, "localhost,3306" },
                    { "RootLogin", 17, "root" },
                    { "RootPassword", 17, "" },
                    { "ExternalAddress", 30, "localhost" },
                    { "InstallFolder", 30, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.1" },
                    { "InternalAddress", 30, "localhost,3306" },
                    { "RootLogin", 30, "root" },
                    { "RootPassword", 30, "" },
                    { "ExternalAddress", 301, "localhost" },
                    { "InstallFolder", 301, "%PROGRAMFILES%\\MySQL\\MySQL Server 5.5" },
                    { "InternalAddress", 301, "localhost,3306" },
                    { "RootLogin", 301, "root" },
                    { "RootPassword", 301, "" },
                    { "ExternalAddress", 304, "localhost" },
                    { "InstallFolder", 304, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 304, "localhost,3306" },
                    { "RootLogin", 304, "root" },
                    { "RootPassword", 304, "" },
                    { "sslmode", 304, "True" },
                    { "ExternalAddress", 305, "localhost" },
                    { "InstallFolder", 305, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 305, "localhost,3306" },
                    { "RootLogin", 305, "root" },
                    { "RootPassword", 305, "" },
                    { "sslmode", 305, "True" },
                    { "ExternalAddress", 306, "localhost" },
                    { "InstallFolder", 306, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 306, "localhost,3306" },
                    { "RootLogin", 306, "root" },
                    { "RootPassword", 306, "" },
                    { "sslmode", 306, "True" },
                    { "ExternalAddress", 307, "localhost" },
                    { "InstallFolder", 307, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 307, "localhost,3306" },
                    { "RootLogin", 307, "root" },
                    { "RootPassword", 307, "" },
                    { "sslmode", 307, "True" },
                    { "ExternalAddress", 308, "localhost" },
                    { "InstallFolder", 308, "%PROGRAMFILES%\\MySQL\\MySQL Server 8.0" },
                    { "InternalAddress", 308, "localhost,3306" },
                    { "RootLogin", 308, "root" },
                    { "RootPassword", 308, "" },
                    { "sslmode", 308, "True" }
                });
        }
    }
}
