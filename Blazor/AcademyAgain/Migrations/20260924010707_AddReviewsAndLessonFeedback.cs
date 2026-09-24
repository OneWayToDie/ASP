using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsAndLessonFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LessonFeedbacks",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lesson_id = table.Column<long>(type: "BIGINT", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<byte>(type: "TINYINT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonFeedbacks", x => x.feedback_id);
                });

            migrationBuilder.CreateTable(
                name: "LessonFeedbackTags",
                columns: table => new
                {
                    feedback_id = table.Column<int>(type: "int", nullable: false),
                    template_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonFeedbackTags", x => new { x.feedback_id, x.template_id });
                });

            migrationBuilder.CreateTable(
                name: "PraiseTemplates",
                columns: table => new
                {
                    template_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    text = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    category = table.Column<byte>(type: "TINYINT", nullable: false),
                    sort_order = table.Column<int>(type: "INT", nullable: true),
                    is_active = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PraiseTemplates", x => x.template_id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacher_id = table.Column<int>(type: "int", nullable: false),
                    student_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<byte>(type: "TINYINT", nullable: false),
                    text = table.Column<string>(type: "NVARCHAR(1000)", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    is_deleted = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.review_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonFeedbacks_lesson_id_student_id",
                table: "LessonFeedbacks",
                columns: new[] { "lesson_id", "student_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonFeedbacks_student_id",
                table: "LessonFeedbacks",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_LessonFeedbackTags_template_id",
                table: "LessonFeedbackTags",
                column: "template_id");

            migrationBuilder.CreateIndex(
                name: "IX_PraiseTemplates_category",
                table: "PraiseTemplates",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_teacher_id",
                table: "Reviews",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_teacher_id_student_id",
                table: "Reviews",
                columns: new[] { "teacher_id", "student_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonFeedbacks");

            migrationBuilder.DropTable(
                name: "LessonFeedbackTags");

            migrationBuilder.DropTable(
                name: "PraiseTemplates");

            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
