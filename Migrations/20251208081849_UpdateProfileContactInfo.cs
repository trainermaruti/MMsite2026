using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfileContactInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0734df3c-d214-4e7a-8a72-27b69fba92b6", "AQAAAAIAAYagAAAAEGuyVqyK/eX04tXY9tDteKy0oni+vZVuAC5sMJFM37rn3PXyKs23HLrh9C2mP0+SHA==", "69ed344f-05e6-4d62-a1a5-26f3c7d9cf45" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Email", "InstagramUrl", "LinkedInUrl", "PhoneNumber", "SkillTechUrl", "UpdatedDate", "WhatsAppNumber", "YouTubeUrl" },
                values: new object[] { new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5220), "maruti_makwana@hotmail.com", "https://www.instagram.com/marutimakwana?igsh=MWttazg1dGRkbTU3cg==", "https://www.linkedin.com/in/marutimakwana/", "+91 9998114148", "https://skilltech.club", new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5221), "+91 9081908127", "https://www.youtube.com/@skilltechclub" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5658), new DateTime(2025, 12, 8, 8, 18, 48, 375, DateTimeKind.Utc).AddTicks(5659) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "CreatedDate", "Email", "InstagramUrl", "LinkedInUrl", "PhoneNumber", "SkillTechUrl", "UpdatedDate", "WhatsAppNumber", "YouTubeUrl" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(194), "maruti@example.com", "https://instagram.com/maruti", "https://linkedin.com/in/maruti", "+91-XXXXXXXXXX", "https://skilltech.com/maruti", new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(195), "+91-XXXXXXXXXX", "https://youtube.com/@maruti" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(366), new DateTime(2025, 12, 3, 9, 48, 18, 833, DateTimeKind.Utc).AddTicks(366) });
        }
    }
}
