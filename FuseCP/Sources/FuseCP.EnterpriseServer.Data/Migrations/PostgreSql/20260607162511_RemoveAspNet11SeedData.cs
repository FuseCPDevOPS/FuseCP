using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class RemoveAspNet11SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRMUsers",
                schema: "public");

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "AspNet11Path", 2 });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "AspNet11Pool", 2 });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ScheduleTaskParameters",
                columns: new[] { "ParameterID", "TaskID", "DataTypeID", "DefaultValue", "ParameterOrder" },
                values: new object[,]
                {
                    { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH", "String", "3", 1 },
                    { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH", "String", "250", 2 },
                    { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE", "String", "3", 1 },
                    { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE", "String", "250", 2 },
                    { "REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "2", 12 },
                    { "REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "250", 13 },
                    { "REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITE", "String", "15", 11 },
                    { "SSL_REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL", "String", "2", 13 },
                    { "SSL_REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL", "String", "250", 14 },
                    { "SSL_REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL", "String", "15", 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.DeleteData(
                schema: "public",
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.CreateTable(
                name: "CRMUsers",
                schema: "public",
                columns: table => new
                {
                    CRMUserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountID = table.Column<int>(type: "integer", nullable: false),
                    BusinessUnitID = table.Column<Guid>(type: "uuid", nullable: true),
                    CALType = table.Column<int>(type: "integer", nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CRMUserGuid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CRMUsers", x => x.CRMUserID);
                    table.ForeignKey(
                        name: "FK_CRMUsers_ExchangeAccounts",
                        column: x => x.AccountID,
                        principalSchema: "public",
                        principalTable: "ExchangeAccounts",
                        principalColumn: "AccountID");
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "AspNet11Path", 2, "%SYSTEMROOT%\\Microsoft.NET\\Framework\\v1.1.4322\\aspnet_isapi.dll" },
                    { "AspNet11Pool", 2, "ASP.NET V1.1" }
                });

            migrationBuilder.CreateIndex(
                name: "CRMUsersIdx_AccountID",
                schema: "public",
                table: "CRMUsers",
                column: "AccountID");
        }
    }
}
