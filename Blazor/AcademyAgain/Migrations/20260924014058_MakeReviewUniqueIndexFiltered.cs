using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class MakeReviewUniqueIndexFiltered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_teacher_id_student_id",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_teacher_id_student_id",
                table: "Reviews",
                columns: new[] { "teacher_id", "student_id" },
                unique: true,
                filter: "[is_deleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_teacher_id_student_id",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_teacher_id_student_id",
                table: "Reviews",
                columns: new[] { "teacher_id", "student_id" },
                unique: true);
        }
    }
}
