using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class MakeDeliveryDateOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Trainings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryDate",
                table: "Trainings",
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
    }
}
