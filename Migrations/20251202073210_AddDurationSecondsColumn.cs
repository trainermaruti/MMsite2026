using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationSecondsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "711d705c-bd01-47f2-84d5-c88b518d2e27", "AQAAAAIAAYagAAAAEIYIala9vOGlu6E7nYDJ19BAQydr00u9VluM9sl92HKtf5UpGQyiCxZOnX3Z0z5Reg==", "e725714d-f0f0-42a9-8def-18798d5660b6" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 7, 32, 9, 894, DateTimeKind.Utc).AddTicks(3189), new DateTime(2025, 12, 2, 7, 32, 9, 894, DateTimeKind.Utc).AddTicks(3190) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Courses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1bca749a-2f22-4731-b410-45a4d08fbbc3", "AQAAAAIAAYagAAAAEGg0KK+8xc8bijBoWl3P+42RIxJY5yS7h/cfdRkB5Jls1Jahq36vkTPeLI4N7COa/A==", "83033db9-67a7-4a3d-9a6a-339b9ee942be" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 1, 7, 17, 10, 340, DateTimeKind.Utc).AddTicks(3870), new DateTime(2025, 12, 1, 7, 17, 10, 340, DateTimeKind.Utc).AddTicks(3873) });
        }
    }
}
