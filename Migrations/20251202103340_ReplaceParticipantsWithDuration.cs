using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceParticipantsWithDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParticipantsCount",
                table: "Trainings");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Trainings",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "778b18b8-1358-45b4-85b6-257bc25e3608", "AQAAAAIAAYagAAAAENwP57AxFKY66TUqpS1NryZO3o0B8YynIuCG35jhucRiR5NjB4OGvLXbLtP7J0E0cQ==", "92b43bad-5a41-4a30-9cfe-b097d0bb25c3" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 10, 33, 40, 58, DateTimeKind.Utc).AddTicks(2012), new DateTime(2025, 12, 2, 10, 33, 40, 58, DateTimeKind.Utc).AddTicks(2013) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 10, 33, 40, 58, DateTimeKind.Utc).AddTicks(2129), new DateTime(2025, 12, 2, 10, 33, 40, 58, DateTimeKind.Utc).AddTicks(2129) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Trainings");

            migrationBuilder.AddColumn<int>(
                name: "ParticipantsCount",
                table: "Trainings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1357), new DateTime(2025, 12, 2, 9, 35, 42, 632, DateTimeKind.Utc).AddTicks(1358) });
        }
    }
}
