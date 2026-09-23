using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "VacancyApplications",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Teachers",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Students",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Schedule",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Grades",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Exams",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "CandidateRequests",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "AdmissionRequests",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    audit_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "INT", nullable: true),
                    action = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    entity = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    entity_id = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    old_value = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    new_value = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.audit_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_entity_created_at",
                table: "AuditLogs",
                columns: new[] { "entity", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_user_id",
                table: "AuditLogs",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "VacancyApplications");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "CandidateRequests");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "AdmissionRequests");
        }
    }
}
