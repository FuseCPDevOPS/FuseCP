using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RenameMariaDbProviderType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Providers"" SET ""ProviderType"" = 'FuseCP.Providers.Database.MariaDB, FuseCP.Providers.Database.MariaDB', ""DisplayName"" = 'MariaDB' WHERE ""ProviderID"" = 1586;
UPDATE ""ServiceDefaultProperties"" SET ""PropertyValue"" = '%PROGRAMFILES%\MariaDB' WHERE ""ProviderID"" = 1586 AND ""PropertyName"" = 'InstallFolder';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""Providers"" SET ""ProviderType"" = 'FuseCP.Providers.Database.MariaDB117, FuseCP.Providers.Database.MariaDB', ""DisplayName"" = 'MariaDB 11.7' WHERE ""ProviderID"" = 1586;
UPDATE ""ServiceDefaultProperties"" SET ""PropertyValue"" = '%PROGRAMFILES%\MariaDB 11.7' WHERE ""ProviderID"" = 1586 AND ""PropertyName"" = 'InstallFolder';");
        }
    }
}
