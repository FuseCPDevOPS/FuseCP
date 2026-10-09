using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddWebAuthnCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebAuthnCredentials",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RpId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    UserHandle = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CredentialId = table.Column<byte[]>(type: "BLOB", nullable: true),
                    CredentialType = table.Column<int>(type: "INTEGER", nullable: false),
                    Kty = table.Column<int>(type: "INTEGER", nullable: false),
                    Alg = table.Column<int>(type: "INTEGER", nullable: false),
                    Ec2Crv = table.Column<int>(type: "INTEGER", nullable: true),
                    Ec2X = table.Column<byte[]>(type: "BLOB", nullable: true),
                    Ec2Y = table.Column<byte[]>(type: "BLOB", nullable: true),
                    RsaModulusN = table.Column<byte[]>(type: "BLOB", nullable: true),
                    RsaExponentE = table.Column<byte[]>(type: "BLOB", nullable: true),
                    OkpCrv = table.Column<int>(type: "INTEGER", nullable: true),
                    OkpX = table.Column<byte[]>(type: "BLOB", nullable: true),
                    SignCount = table.Column<long>(type: "INTEGER", nullable: false),
                    Transports = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UvInitialized = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackupEligible = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackupState = table.Column<bool>(type: "INTEGER", nullable: false),
                    AttestationObject = table.Column<byte[]>(type: "BLOB", nullable: true),
                    AttestationClientDataJson = table.Column<byte[]>(type: "BLOB", nullable: true),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebAuthnCredentials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebAuthnCredentials_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "WebAuthnCredentialsIdx_CredentialId",
                table: "WebAuthnCredentials",
                column: "CredentialId");

            migrationBuilder.CreateIndex(
                name: "WebAuthnCredentialsIdx_UserId",
                table: "WebAuthnCredentials",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebAuthnCredentials");
        }
    }
}
