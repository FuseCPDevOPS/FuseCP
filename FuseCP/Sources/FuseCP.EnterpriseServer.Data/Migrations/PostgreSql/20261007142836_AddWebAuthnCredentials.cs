using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class AddWebAuthnCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebAuthnCredentials",
                schema: "public",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RpId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserHandle = table.Column<byte[]>(type: "bytea", nullable: true),
                    CredentialId = table.Column<byte[]>(type: "bytea", nullable: true),
                    CredentialType = table.Column<int>(type: "integer", nullable: false),
                    Kty = table.Column<int>(type: "integer", nullable: false),
                    Alg = table.Column<int>(type: "integer", nullable: false),
                    Ec2Crv = table.Column<int>(type: "integer", nullable: true),
                    Ec2X = table.Column<byte[]>(type: "bytea", nullable: true),
                    Ec2Y = table.Column<byte[]>(type: "bytea", nullable: true),
                    RsaModulusN = table.Column<byte[]>(type: "bytea", nullable: true),
                    RsaExponentE = table.Column<byte[]>(type: "bytea", nullable: true),
                    OkpCrv = table.Column<int>(type: "integer", nullable: true),
                    OkpX = table.Column<byte[]>(type: "bytea", nullable: true),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    Transports = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UvInitialized = table.Column<bool>(type: "boolean", nullable: false),
                    BackupEligible = table.Column<bool>(type: "boolean", nullable: false),
                    BackupState = table.Column<bool>(type: "boolean", nullable: false),
                    AttestationObject = table.Column<byte[]>(type: "bytea", nullable: true),
                    AttestationClientDataJson = table.Column<byte[]>(type: "bytea", nullable: true),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebAuthnCredentials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_WebAuthnCredentials_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "WebAuthnCredentialsIdx_CredentialId",
                schema: "public",
                table: "WebAuthnCredentials",
                column: "CredentialId");

            migrationBuilder.CreateIndex(
                name: "WebAuthnCredentialsIdx_UserId",
                schema: "public",
                table: "WebAuthnCredentials",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebAuthnCredentials",
                schema: "public");
        }
    }
}
