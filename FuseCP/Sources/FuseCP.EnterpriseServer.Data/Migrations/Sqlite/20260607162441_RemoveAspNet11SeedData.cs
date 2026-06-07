using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FuseCP.EnterpriseServer.Data.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class RemoveAspNet11SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CRMUsers");

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "AspNet11Path", 2 });

            migrationBuilder.DeleteData(
                table: "ServiceDefaultProperties",
                keyColumns: new[] { "PropertyName", "ProviderID" },
                keyValues: new object[] { "AspNet11Pool", 2 });

            migrationBuilder.InsertData(
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
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_BANDWIDTH" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_CALL_ATTEMPTS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SERVICE_RETRY_DELAY_MS", "SCHEDULE_TASK_CALCULATE_PACKAGES_DISKSPACE" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITE" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_ATTEMPTS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_RETRY_DELAY_MS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.DeleteData(
                table: "ScheduleTaskParameters",
                keyColumns: new[] { "ParameterID", "TaskID" },
                keyValues: new object[] { "SSL_REQUEST_TIMEOUT_SECONDS", "SCHEDULE_TASK_CHECK_WEBSITES_SSL" });

            migrationBuilder.CreateTable(
                name: "CRMUsers",
                columns: table => new
                {
                    CRMUserID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountID = table.Column<int>(type: "INTEGER", nullable: false),
                    BusinessUnitID = table.Column<Guid>(type: "TEXT", nullable: true),
                    CALType = table.Column<int>(type: "INTEGER", nullable: true),
                    ChangedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CRMUserGuid = table.Column<Guid>(type: "TEXT", nullable: true)
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

            migrationBuilder.InsertData(
                table: "ServiceDefaultProperties",
                columns: new[] { "PropertyName", "ProviderID", "PropertyValue" },
                values: new object[,]
                {
                    { "AspNet11Path", 2, "%SYSTEMROOT%\\Microsoft.NET\\Framework\\v1.1.4322\\aspnet_isapi.dll" },
                    { "AspNet11Pool", 2, "ASP.NET V1.1" }
                });

            migrationBuilder.CreateIndex(
                name: "CRMUsersIdx_AccountID",
                table: "CRMUsers",
                column: "AccountID");
        }
    }
}
