using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteTitle = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    OgImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    FaviconUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ContactFormReceiverEmail = table.Column<string>(type: "TEXT", nullable: true),
                    SecondaryNotificationEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EmailNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaintenanceMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowUpcomingEvents = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowCoursesSection = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShowProfileSection = table.Column<bool>(type: "INTEGER", nullable: false),
                    SmtpConfigured = table.Column<bool>(type: "INTEGER", nullable: false),
                    SendGridConfigured = table.Column<bool>(type: "INTEGER", nullable: false),
                    AzureOpenAIConfigured = table.Column<bool>(type: "INTEGER", nullable: false),
                    AppVersion = table.Column<string>(type: "TEXT", nullable: false),
                    LastBackupDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DatabaseSize = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f3da7cc-7101-4e35-902d-39dd2da702a1", "AQAAAAIAAYagAAAAEKE5MoI/rY8hoPpNMjx0Sihi5N0yUukGBs9Xz6RVOLidu7flT3jJlQk1cDRszH3pIA==", "b6ac9ac2-00d7-4bd2-83f3-680dfcacdf7f" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1219), new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1219) });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "AppVersion", "AzureOpenAIConfigured", "ContactFormReceiverEmail", "CreatedDate", "DatabaseSize", "EmailNotificationsEnabled", "FaviconUrl", "LastBackupDate", "MaintenanceMode", "MetaDescription", "MetaKeywords", "OgImageUrl", "SecondaryNotificationEmail", "SendGridConfigured", "ShowCoursesSection", "ShowProfileSection", "ShowUpcomingEvents", "SiteTitle", "SmtpConfigured", "UpdatedDate" },
                values: new object[] { 1, "1.0.0", false, null, new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1357), null, true, null, null, false, "Professional Azure & AI Training by Maruti Makwana - Corporate Trainer specializing in Cloud Computing and Artificial Intelligence", "Azure Training, AI Training, Cloud Computing, Corporate Training, Machine Learning, DevOps", null, null, false, true, true, true, "Maruti Makwana Training Portal", false, new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1358) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3bb4a18c-dc1f-4bfc-b53b-d09e6ba3265c", "AQAAAAIAAYagAAAAEOTvkmWsAFNHmT3Ad6dqdinO8GYL/9wE1hx271QEUMB+xpp7NH8DNqiQLM7i7hG7lA==", "0bdec2e5-b228-4821-8063-a1f86a35fb7d" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 7, 33, 38, 2, DateTimeKind.Utc).AddTicks(9825), new DateTime(2025, 12, 2, 7, 33, 38, 2, DateTimeKind.Utc).AddTicks(9825) });
        }
    }
}
