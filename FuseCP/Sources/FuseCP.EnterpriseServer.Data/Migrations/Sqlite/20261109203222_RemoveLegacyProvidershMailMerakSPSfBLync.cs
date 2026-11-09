using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemoveLegacyProvidershMailMerakSPSfBLync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LyncUsers");

            migrationBuilder.DropTable(
                name: "SfBUserPlans");

            migrationBuilder.DropTable(
                name: "SfBUsers");

            migrationBuilder.DropTable(
                name: "LyncUserPlans");

            // FK-safety: delete child rows before removing provider records
            migrationBuilder.Sql("DELETE FROM ServiceDefaultProperties WHERE ProviderID IN (13, 20, 22, 1306, 1404);");
            
            // Remap services to surviving providers before deleting old providers
            migrationBuilder.Sql("UPDATE Services SET ProviderID = 63 WHERE ProviderID = 20;");
            migrationBuilder.Sql("UPDATE Services SET ProviderID = 63 WHERE ProviderID = 22;");
            migrationBuilder.Sql("UPDATE Services SET ProviderID = 61 WHERE ProviderID = 13;");
            migrationBuilder.Sql("UPDATE Services SET ProviderID = 1711 WHERE ProviderID = 1306;");
            migrationBuilder.Sql("DELETE FROM Services WHERE ProviderID = 1404;");

            // Remap ServiceDefaultProperties
            migrationBuilder.Sql("UPDATE ServiceDefaultProperties SET ProviderID = 63 WHERE ProviderID = 20;");
            migrationBuilder.Sql("UPDATE ServiceDefaultProperties SET ProviderID = 63 WHERE ProviderID = 22;");
            migrationBuilder.Sql("UPDATE ServiceDefaultProperties SET ProviderID = 61 WHERE ProviderID = 13;");
            migrationBuilder.Sql("UPDATE ServiceDefaultProperties SET ProviderID = 1711 WHERE ProviderID = 1306;");

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1306);

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderID",
                keyValue: 1404);

            migrationBuilder.DropColumn(
                name: "LyncUserPlanID",
                table: "ExchangeOrganizations");

            migrationBuilder.DropColumn(
                name: "SfBUserPlanID",
                table: "ExchangeOrganizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LyncUserPlanID",
                table: "ExchangeOrganizations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SfBUserPlanID",
                table: "ExchangeOrganizations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LyncUserPlans",
                columns: table => new
                {
                    LyncUserPlanId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ArchivePolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    Conferencing = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Federation = table.Column<bool>(type: "INTEGER", nullable: false),
                    IM = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    LyncUserPlanName = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: false),
                    LyncUserPlanType = table.Column<int>(type: "INTEGER", nullable: true),
                    Mobility = table.Column<bool>(type: "INTEGER", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublicIMConnectivity = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    RemoteUserAccess = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ServerURI = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Telephony = table.Column<int>(type: "INTEGER", nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    VoicePolicy = table.Column<int>(type: "TEXT COLLATE NOCASE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUserPlans", x => x.LyncUserPlanId);
                    table.ForeignKey(
                        name: "FK_LyncUserPlans_ExchangeOrganizations",
                        column: x => x.ItemID,
                        principalTable: "ExchangeOrganizations",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SfBUserPlans",
                columns: table => new
                {
                    SfBUserPlanId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AllowOrganizeMeetingsWithExternalAnonymous = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ArchivePolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    Conferencing = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnterpriseVoice = table.Column<bool>(type: "INTEGER", nullable: false),
                    Federation = table.Column<bool>(type: "INTEGER", nullable: false),
                    IM = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    ItemID = table.Column<int>(type: "INTEGER", nullable: false),
                    Mobility = table.Column<bool>(type: "INTEGER", nullable: false),
                    MobilityEnableOutsideVoice = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublicIMConnectivity = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    RemoteUserAccess = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ServerURI = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    SfBUserPlanName = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    SfBUserPlanType = table.Column<int>(type: "INTEGER", nullable: true),
                    Telephony = table.Column<int>(type: "INTEGER", nullable: true),
                    TelephonyDialPlanPolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    TelephonyVoicePolicy = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true),
                    VoicePolicy = table.Column<int>(type: "TEXT COLLATE NOCASE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUserPlans", x => x.SfBUserPlanId);
                });

            migrationBuilder.CreateTable(
                name: "SfBUsers",
                columns: table => new
                {
                    SfBUserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SfBUserPlanID = table.Column<int>(type: "INTEGER", nullable: false),
                    SipAddress = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SfBUsers", x => x.SfBUserID);
                });

            migrationBuilder.CreateTable(
                name: "LyncUsers",
                columns: table => new
                {
                    LyncUserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LyncUserPlanID = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SipAddress = table.Column<string>(type: "TEXT COLLATE NOCASE", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LyncUsers", x => x.LyncUserID);
                    table.ForeignKey(
                        name: "FK_LyncUsers_LyncUserPlans",
                        column: x => x.LyncUserPlanID,
                        principalTable: "LyncUserPlans",
                        principalColumn: "LyncUserPlanId");
                });

            migrationBuilder.InsertData(
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
                table: "LyncUserPlans",
                column: "LyncUserPlanId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "LyncUserPlansIdx_ItemID",
                table: "LyncUserPlans",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "LyncUsersIdx_LyncUserPlanID",
                table: "LyncUsers",
                column: "LyncUserPlanID");
        }
    }
}
