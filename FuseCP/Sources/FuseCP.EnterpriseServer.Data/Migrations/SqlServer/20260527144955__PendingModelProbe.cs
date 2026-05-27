using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuseCP.EnterpriseServer.Data.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class _PendingModelProbe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRMUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "CommentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentID");

            migrationBuilder.CreateTable(
                name: "CRMUsers",
                columns: table => new
                {
                    CRMUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    BusinessUnitID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CALType = table.Column<int>(type: "int", nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CRMUserGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMUsers", x => x.CRMUserID);
                    table.ForeignKey(
                        name: "FK_CRMUsers_ExchangeAccounts",
                        column: x => x.AccountID,
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateIndex(
                name: "CRMUsersIdx_AccountID",
                table: "CRMUsers",
                column: "AccountID");
        }
    }
}
