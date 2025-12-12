using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceEventsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "TrainingEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TrainingEventRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainingEventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingEventRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingEventRegistrations_TrainingEvents_TrainingEventId",
                        column: x => x.TrainingEventId,
                        principalTable: "TrainingEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEventRegistrations_TrainingEventId",
                table: "TrainingEventRegistrations",
                column: "TrainingEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingEventRegistrations");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "TrainingEvents");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c842302-82d6-4211-8151-8980917d4110", "AQAAAAIAAYagAAAAEG87hm4kiovX3vZDppZ553T63TjxLWrhsu9UN9k3zpya0MHVGILX0Gw4uitYVlMfSg==", "21cb4f74-2edd-41a6-8356-f1354163abc9" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 10, 39, 1, 906, DateTimeKind.Utc).AddTicks(2874), new DateTime(2025, 12, 2, 10, 39, 1, 906, DateTimeKind.Utc).AddTicks(2874) });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 2, 10, 39, 1, 906, DateTimeKind.Utc).AddTicks(2997), new DateTime(2025, 12, 2, 10, 39, 1, 906, DateTimeKind.Utc).AddTicks(2998) });
        }
    }
}
