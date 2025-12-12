using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillTechUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SkillTechUrl",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "CreatedDate", "SkillTechUrl", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5194), "https://skilltech.com/maruti", new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5194) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5316), new DateTime(2025, 12, 3, 9, 38, 43, 774, DateTimeKind.Utc).AddTicks(5317) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SkillTechUrl",
                table: "Profiles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "410ecc07-08e2-4f31-81f6-c87e542a9a66", "AQAAAAIAAYagAAAAEF9UmF6L03/zeeIoUcvYvaFbJcO3eRgNeTIeKSt9bBq6RotsswoTjodgWa6YqH3MAA==", "a3bdc88c-c67c-45ea-a639-cda9ebb2f697" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1619), new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1620) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1740), new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1744) });
        }
    }
}
