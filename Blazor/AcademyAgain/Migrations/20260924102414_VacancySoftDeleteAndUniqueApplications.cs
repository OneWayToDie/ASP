using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class VacancySoftDeleteAndUniqueApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "Vacancies",
                type: "BIT",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_VacancyApplications_candidate_user_id_vacancy_id",
                table: "VacancyApplications",
                columns: new[] { "candidate_user_id", "vacancy_id" },
                unique: true,
                filter: "[is_deleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VacancyApplications_candidate_user_id_vacancy_id",
                table: "VacancyApplications");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "Vacancies");
        }
    }
}
