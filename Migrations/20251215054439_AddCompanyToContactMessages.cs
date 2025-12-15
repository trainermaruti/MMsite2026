using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToContactMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceIp",
                table: "ContactMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ContactMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95a9a9c3-36a3-4906-85cd-d4656c16cd0b", "AQAAAAIAAYagAAAAEPtRkqBNjYILF+lMgj0mdCickVhyNVYw1Nc+5D6WPEqtwitRhQZ167mGWSEehuPE4Q==", "81cc0f03-14f2-4d0c-b2a9-72f95f2d21a8" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 15, 5, 44, 38, 836, DateTimeKind.Utc).AddTicks(7201), new DateTime(2025, 12, 15, 5, 44, 38, 836, DateTimeKind.Utc).AddTicks(7202) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 15, 5, 44, 38, 836, DateTimeKind.Utc).AddTicks(7356), new DateTime(2025, 12, 15, 5, 44, 38, 836, DateTimeKind.Utc).AddTicks(7357) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "SourceIp",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ContactMessages");

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
    }
}
