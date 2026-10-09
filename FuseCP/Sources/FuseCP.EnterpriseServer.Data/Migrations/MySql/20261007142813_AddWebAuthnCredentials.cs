using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.MySql
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
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RpId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserHandle = table.Column<byte[]>(type: "longblob", nullable: true),
                    CredentialId = table.Column<byte[]>(type: "varbinary(3072)", nullable: true),
                    CredentialType = table.Column<int>(type: "int", nullable: false),
                    Kty = table.Column<int>(type: "int", nullable: false),
                    Alg = table.Column<int>(type: "int", nullable: false),
                    Ec2Crv = table.Column<int>(type: "int", nullable: true),
                    Ec2X = table.Column<byte[]>(type: "longblob", nullable: true),
                    Ec2Y = table.Column<byte[]>(type: "longblob", nullable: true),
                    RsaModulusN = table.Column<byte[]>(type: "longblob", nullable: true),
                    RsaExponentE = table.Column<byte[]>(type: "longblob", nullable: true),
                    OkpCrv = table.Column<int>(type: "int", nullable: true),
                    OkpX = table.Column<byte[]>(type: "longblob", nullable: true),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    Transports = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UvInitialized = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BackupEligible = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BackupState = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AttestationObject = table.Column<byte[]>(type: "longblob", nullable: true),
                    AttestationClientDataJson = table.Column<byte[]>(type: "longblob", nullable: true),
                    DisplayName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
