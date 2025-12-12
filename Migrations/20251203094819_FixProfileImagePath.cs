using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class FixProfileImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "35afc5f7-b832-4653-8d64-6ae46da6e567", "AQAAAAIAAYagAAAAECNUZrgoZDEmHQ4LYEFoMczSusRzP/RnER0YK1VKG9r1Y4HKGYd9dI/lUU16VtpPxA==", "7fac422f-db27-499f-b902-265b101dbe81" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ProfileImageUrl", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(194), "/images/44.png", new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(195) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(366), new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(366) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bb95fd39-33a5-458c-bf7b-4e2a99424b00", "AQAAAAIAAYagAAAAEMJDUWh6RwFQky18c9I95AjhzgTTGEEkizu2yqD6Rty8EtBmv5c7qpLKzNcBu0QGbw==", "f113c026-ed3e-410a-a05e-6541da9771cb" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ProfileImageUrl", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5194), "/images/profile-placeholder.jpg", new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5194) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5316), new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5317) });
        }
    }
}
