using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingEvents",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "WebsiteImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ImageKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteImages", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "498553a6-0d00-46d9-b4b8-a5ee4b9d561d", "AQAAAAIAAYagAAAAEKA9hORoedADG1+HxGS6cRln8uAk+ukVvfmBwIVFkL7yTQD+HdIihXaD+8v5TrojUQ==", "c1b4dd0b-1366-4043-a726-fde9fb29def1" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 9, 11, 34, 51, 510, DateTimeKind.Utc).AddTicks(9379), new DateTime(2025, 12, 9, 11, 34, 51, 510, DateTimeKind.Utc).AddTicks(9380) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 9, 11, 34, 51, 510, DateTimeKind.Utc).AddTicks(9738), new DateTime(2025, 12, 9, 11, 34, 51, 510, DateTimeKind.Utc).AddTicks(9739) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebsiteImages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "TrainingEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

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
    }
}
