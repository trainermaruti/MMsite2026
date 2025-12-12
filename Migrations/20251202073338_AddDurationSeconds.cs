using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationSeconds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
