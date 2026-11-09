using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class RemoveLegacyProvidershMailMerakSPSfBLync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LyncUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SfBUserPlans",
                schema: "public");

            migrationBuilder.DropTable(
                name: "SfBUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "LyncUserPlans",
                schema: "public");

            // FK-safety: delete child rows before removing provider records
            migrationBuilder.Sql("DELETE FROM \"ServiceDefaultProperties\" WHERE \"ProviderID\" IN (13, 20, 22, 1306, 1404);");
            
            // Remap services to surviving providers before deleting old providers
            migrationBuilder.Sql("UPDATE \"Services\" SET \"ProviderID\" = 63 WHERE \"ProviderID\" = 20;");
            migrationBuilder.Sql("UPDATE \"Services\" SET \"ProviderID\" = 63 WHERE \"ProviderID\" = 22;");
            migrationBuilder.Sql("UPDATE \"Services\" SET \"ProviderID\" = 61 WHERE \"ProviderID\" = 13;");
            migrationBuilder.Sql("UPDATE \"Services\" SET \"ProviderID\" = 1711 WHERE \"ProviderID\" = 1306;");
            migrationBuilder.Sql("DELETE FROM \"Services\" WHERE \"ProviderID\" = 1404;");

            // Remap ServiceDefaultProperties
            migrationBuilder.Sql("UPDATE \"ServiceDefaultProperties\" SET \"ProviderID\" = 63 WHERE \"ProviderID\" = 20;");
            migrationBuilder.Sql("UPDATE \"ServiceDefaultProperties\" SET \"ProviderID\" = 63 WHERE \"ProviderID\" = 22;");
            migrationBuilder.Sql("UPDATE \"ServiceDefaultProperties\" SET \"ProviderID\" = 61 WHERE \"ProviderID\" = 13;");
            migrationBuilder.Sql("UPDATE \"ServiceDefaultProperties\" SET \"ProviderID\" = 1711 WHERE \"ProviderID\" = 1306;");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1306);

            migrationBuilder.DeleteData(
                schema: "public",
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1404);

            migrationBuilder.DropColumn(
                name: "LyncUserPlanID",
                schema: "public",
                table: "ExchangeOrganizations");

            migrationBuilder.DropColumn(
                name: "SfBUserPlanID",
                schema: "public",
                table: "ExchangeOrganizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LyncUserPlanID",
                schema: "public",
                table: "ExchangeOrganizations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SfBUserPlanID",
                schema: "public",
                table: "ExchangeOrganizations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LyncUserPlans",
                schema: "public",
                columns: table => new
                {
                    LyncUserPlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemID = table.Column<int>(type: "integer", nullable: false),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ArchivePolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Conferencing = table.Column<bool>(type: "boolean", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "boolean", nullable: false),
                    Federation = table.Column<bool>(type: "boolean", nullable: false),
                    IM = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    LyncUserPlanName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    LyncUserPlanType = table.Column<int>(type: "integer", nullable: true),
                    Mobility = table.Column<bool>(type: "boolean", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "boolean", nullable: false),
                    PublicIMConnectivity = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RemoteUserAccess = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ServerURI = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Telephony = table.Column<int>(type: "integer", nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    VoicePolicy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUserPlans", x => x.LyncUserPlanId);
                    table.ForeignKey(
                        name: "FK_LyncUserPlans_ExchangeOrganizations",
                        column: x => x.ItemID,
                        principalSchema: "public",
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfBUserPlans",
                schema: "public",
                columns: table => new
                {
                    SfBUserPlanId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ArchivePolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Conferencing = table.Column<bool>(type: "boolean", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "boolean", nullable: false),
                    Federation = table.Column<bool>(type: "boolean", nullable: false),
                    IM = table.Column<bool>(type: "boolean", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ItemID = table.Column<int>(type: "integer", nullable: false),
                    Mobility = table.Column<bool>(type: "boolean", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "boolean", nullable: false),
                    PublicIMConnectivity = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RemoteUserAccess = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ServerURI = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SfBUserPlanName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    SfBUserPlanType = table.Column<int>(type: "integer", nullable: true),
                    Telephony = table.Column<int>(type: "integer", nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    VoicePolicy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUserPlans", x => x.SfBUserPlanId);
                });

            migrationBuilder.CreateTable(
                name: "SfBUsers",
                schema: "public",
                columns: table => new
                {
                    SfBUserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountID = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SfBUserPlanID = table.Column<int>(type: "integer", nullable: false),
                    SipAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUsers", x => x.SfBUserID);
                });

            migrationBuilder.CreateTable(
                name: "LyncUsers",
                schema: "public",
                columns: table => new
                {
                    LyncUserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LyncUserPlanID = table.Column<int>(type: "integer", nullable: false),
                    AccountID = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SipAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUsers", x => x.LyncUserID);
                    table.ForeignKey(
                        name: "FK_LyncUsers_LyncUserPlans",
                        column: x => x.LyncUserPlanID,
                        principalSchema: "public",
                        principalTable: "LyncUserPlans",
                        principalColumn: "LyncUserPlanId");
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "Providers",
                columns: new[] { "ProviderID", "DisableAutoDiscovery", "DisplayName", "EditorControl", "GroupID", "ProviderName", "ProviderType" },
                values: new object[,]
                {
                    { 13, null, "Merak Mail Server 8.0.3 - 9.2.x", "Merak", 4, "Merak", "FuseCP.Providers.Mail.Merak, FuseCP.Providers.Mail.Merak" },
                    { 20, null, "hMailServer 4.2", "hMailServer", 4, "hMailServer", "FuseCP.Providers.Mail.hMailServer, FuseCP.Providers.Mail.hMailServer" },
                    { 22, null, "hMailServer 4.3", "hMailServer43", 4, "hMailServer43", "FuseCP.Providers.Mail.hMailServer43, FuseCP.Providers.Mail.hMailServer43" },
                    { 1306, null, "Hosted SharePoint Foundation 2016", "HostedSharePoint30", 20, "HostedSharePoint2016", "FuseCP.Providers.HostedSolution.HostedSharePointServer2016, FuseCP.Providers.HostedSolution.SharePoint2016" },
                    { 1404, null, "Microsoft Skype for Business Server 2019", "SfB", 52, "SfB2019", "FuseCP.Providers.HostedSolution.SfB2019, FuseCP.Providers.HostedSolution.SfB2019" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LyncUserPlans",
                schema: "public",
                table: "LyncUserPlans",
                column: "LyncUserPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "LyncUserPlansIdx_ItemID",
                schema: "public",
                table: "LyncUserPlans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "LyncUsersIdx_LyncUserPlanID",
                schema: "public",
                table: "LyncUsers",
                column: "LyncUserPlanID");
        }
    }
}
