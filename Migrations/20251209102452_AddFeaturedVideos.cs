using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturedVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "MaxParticipants",
                table: "TrainingEvents",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "FeaturedVideos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedVideos", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9c49b4f-5413-4516-bb64-2cc0e45fe533", "AQAAAAIAAYagAAAAENOdYscQmOpF0gWq6BxQDlwRtwKJSlQChHV0mJOFBmoOHztCw4X/eFWePCJy5u8TfA==", "8b2fa3b5-a97b-4053-adcc-73e0249eb3c3" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 9, 10, 24, 51, 925, DateTimeKind.Utc).AddTicks(1642), new DateTime(2025, 12, 9, 10, 24, 51, 925, DateTimeKind.Utc).AddTicks(1642) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 9, 10, 24, 51, 925, DateTimeKind.Utc).AddTicks(1902), new DateTime(2025, 12, 9, 10, 24, 51, 925, DateTimeKind.Utc).AddTicks(1902) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeaturedVideos");

            migrationBuilder.AlterColumn<string>(
                name: "TimeZone",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxParticipants",
                table: "TrainingEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0734df3c-d214-4e7a-8a72-27b69fba92b6", "AQAAAAIAAYagAAAAEGuyVqyK/eX04tXY9tDteKy0oni+vZVuAC5sMJFM37rn3PXyKs23HLrh9C2mP0+SHA==", "69ed344f-05e6-4d62-a1a5-26f3c7d9cf45" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5220), new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5221) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5658), new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5659) });
        }
    }
}
