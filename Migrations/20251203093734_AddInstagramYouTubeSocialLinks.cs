using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddInstagramYouTubeSocialLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "Profiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

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
                columns: new[] { "CreatedDate", "InstagramUrl", "UpdatedDate", "YouTubeUrl" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1619), "https://instagram.com/maruti", new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1620), "https://youtube.com/@maruti" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1740), new DateTime(2025, 12, 3, 9, 37, 33, 728, DateTimeKind.Utc).AddTicks(1744) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "Profiles");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "764079f4-82b4-4c95-b08a-c5e9252d9805", "AQAAAAIAAYagAAAAEIh7w2bkEtHBEC2GOpF5fm3O+8QxmLOh9QucjE9YZc6IacJxzMakbYmjj8FyflrBkg==", "3adf5bc9-7343-43cf-91af-dcca03652645" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 11, 4, 39, 675, DateTimeKind.Utc).AddTicks(3915), new DateTime(2025, 12, 2, 11, 4, 39, 675, DateTimeKind.Utc).AddTicks(3917) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 11, 4, 39, 675, DateTimeKind.Utc).AddTicks(4225), new DateTime(2025, 12, 2, 11, 4, 39, 675, DateTimeKind.Utc).AddTicks(4226) });
        }
    }
}
