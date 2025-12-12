using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarutiTrainingPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Topics",
                table: "Trainings",
                type: "TEXT",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Trainings",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Trainings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SkillTechUrl",
                table: "Trainings",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Trainings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationLink",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TrainingEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SkillTechUrl",
                table: "TrainingEvents",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "TrainingEvents",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Courses",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Courses",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SkillTechUrl",
                table: "Courses",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Courses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "ContactMessages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ContactMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "ContactMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CertificateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StudentName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    StudentEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CourseTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CourseCategory = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Instructor = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DurationHours = table.Column<int>(type: "INTEGER", nullable: false),
                    Score = table.Column<decimal>(type: "TEXT", nullable: true),
                    Grade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CertificateUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRevoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    RevokedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RevocationReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContactMessageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ChangedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadAuditLogs_ContactMessages_ContactMessageId",
                        column: x => x.ContactMessageId,
                        principalTable: "ContactMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_EventId",
                table: "ContactMessages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_Status",
                table: "ContactMessages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateId",
                table: "Certificates",
                column: "CertificateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_IsDeleted",
                table: "Certificates",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_StudentEmail",
                table: "Certificates",
                column: "StudentEmail");

            migrationBuilder.CreateIndex(
                name: "IX_LeadAuditLogs_ContactMessageId",
                table: "LeadAuditLogs",
                column: "ContactMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactMessages_TrainingEvents_EventId",
                table: "ContactMessages",
                column: "EventId",
                principalTable: "TrainingEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactMessages_TrainingEvents_EventId",
                table: "ContactMessages");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "LeadAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_EventId",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_Status",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "SkillTechUrl",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "SkillTechUrl",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "TrainingEvents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SkillTechUrl",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "ContactMessages");

            migrationBuilder.AlterColumn<string>(
                name: "Topics",
                table: "Trainings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Trainings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RegistrationLink",
                table: "TrainingEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "TrainingEvents",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VideoUrl",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ThumbnailUrl",
                table: "Courses",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "ContactMessages",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "36d48de0-c394-46d6-96fe-b0886018a0c3", "AQAAAAIAAYagAAAAEDS8MxcUhMEjGBEKKN8t+IloLAJX+D8PZ/ExlGC50jUDp6zCO6/V+cDz22+APviyaQ==", "778174dc-4e9d-4e66-93a4-cfc69ee36400" });

            migrationBuilder.UpdateData(
                table: "Profiles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2025, 11, 30, 8, 54, 37, 36, DateTimeKind.Utc).AddTicks(8124), new DateTime(2025, 11, 30, 8, 54, 37, 36, DateTimeKind.Utc).AddTicks(8126) });
        }
    }
}
