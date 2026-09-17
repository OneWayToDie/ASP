using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmissionRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "NVARCHAR(150)", nullable: false),
                    contact = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    direction_id = table.Column<int>(type: "INT", nullable: true),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionRequests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    student = table.Column<int>(type: "int", nullable: false),
                    lesson = table.Column<long>(type: "BIGINT", nullable: false),
                    present = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => new { x.student, x.lesson });
                });

            migrationBuilder.CreateTable(
                name: "CandidateRequests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_user_id = table.Column<int>(type: "INT", nullable: false),
                    teacher_id = table.Column<int>(type: "INT", nullable: false),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateRequests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CompleteDisciplines",
                columns: table => new
                {
                    group = table.Column<int>(type: "int", nullable: false),
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompleteDisciplines", x => new { x.group, x.discipline });
                });

            migrationBuilder.CreateTable(
                name: "DaysOFF",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "DATE", nullable: false),
                    holiday = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaysOFF", x => x.date);
                });

            migrationBuilder.CreateTable(
                name: "DependentDisciplines",
                columns: table => new
                {
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false),
                    dependent_discipline = table.Column<short>(type: "SMALLINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentDisciplines", x => new { x.discipline, x.dependent_discipline });
                });

            migrationBuilder.CreateTable(
                name: "Directions",
                columns: table => new
                {
                    direction_id = table.Column<byte>(type: "TINYINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    direction_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directions", x => x.direction_id);
                });

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    discipline_id = table.Column<short>(type: "SMALLINT", nullable: false),
                    discipline_name = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    number_of_lessons = table.Column<byte>(type: "TINYINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.discipline_id);
                });

            migrationBuilder.CreateTable(
                name: "DisciplinesDirectionsRelation",
                columns: table => new
                {
                    direction = table.Column<byte>(type: "TINYINT", nullable: false),
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisciplinesDirectionsRelation", x => new { x.direction, x.discipline });
                });

            migrationBuilder.CreateTable(
                name: "Exams",
                columns: table => new
                {
                    student = table.Column<int>(type: "int", nullable: false),
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false),
                    date = table.Column<DateTime>(type: "DATE", nullable: true),
                    grade = table.Column<byte>(type: "TINYINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => new { x.student, x.discipline });
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    student = table.Column<int>(type: "int", nullable: false),
                    lesson = table.Column<long>(type: "BIGINT", nullable: false),
                    grade_1 = table.Column<byte>(type: "TINYINT", nullable: true),
                    grade_2 = table.Column<byte>(type: "TINYINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => new { x.student, x.lesson });
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: false),
                    group_name = table.Column<string>(type: "NCHAR(10)", nullable: true),
                    direction = table.Column<byte>(type: "TINYINT", nullable: true),
                    weekdays = table.Column<byte>(type: "TINYINT", nullable: true),
                    start_time = table.Column<TimeSpan>(type: "TIME", nullable: true),
                    start_date = table.Column<DateTime>(type: "DATE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.group_id);
                });

            migrationBuilder.CreateTable(
                name: "Holidays",
                columns: table => new
                {
                    holiday_id = table.Column<byte>(type: "TINYINT", nullable: false),
                    holiday_name = table.Column<string>(type: "NVARCHAR(150)", nullable: false),
                    duration = table.Column<byte>(type: "TINYINT", nullable: false),
                    month = table.Column<byte>(type: "TINYINT", nullable: true),
                    day = table.Column<byte>(type: "TINYINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holidays", x => x.holiday_id);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    news_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    body = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    published_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    is_published = table.Column<bool>(type: "BIT", nullable: false),
                    author_id = table.Column<int>(type: "INT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.news_id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    description = table.Column<string>(type: "NVARCHAR(1000)", nullable: true),
                    tags = table.Column<string>(type: "NVARCHAR(200)", nullable: true),
                    url = table.Column<string>(type: "NVARCHAR(300)", nullable: true),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    pinned = table.Column<bool>(type: "BIT", nullable: false),
                    cover = table.Column<byte[]>(type: "IMAGE", nullable: true),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.project_id);
                });

            migrationBuilder.CreateTable(
                name: "RequiredDisciplines",
                columns: table => new
                {
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false),
                    required_discipline = table.Column<short>(type: "SMALLINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequiredDisciplines", x => new { x.discipline, x.required_discipline });
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<byte>(type: "TINYINT", nullable: false),
                    role_name = table.Column<string>(type: "NVARCHAR(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "Salary",
                columns: table => new
                {
                    payment_id = table.Column<long>(type: "BIGINT", nullable: false),
                    teacher = table.Column<short>(type: "SMALLINT", nullable: false),
                    accrued = table.Column<decimal>(type: "SMALLMONEY", nullable: false),
                    received = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salary", x => x.payment_id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    lesson_id = table.Column<long>(type: "BIGINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group = table.Column<int>(type: "int", nullable: false),
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false),
                    teacher = table.Column<short>(type: "SMALLINT", nullable: false),
                    date = table.Column<DateTime>(type: "DATE", nullable: true),
                    time = table.Column<TimeSpan>(type: "TIME", nullable: true),
                    spent = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.lesson_id);
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    semester_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    start_date = table.Column<DateTime>(type: "DATE", nullable: true),
                    end_date = table.Column<DateTime>(type: "DATE", nullable: true),
                    is_even_week_start = table.Column<bool>(type: "BIT", nullable: false),
                    is_current = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.semester_id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    stud_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    last_name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    first_name = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    middle_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    birth_date = table.Column<DateTime>(type: "DATE", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    phone = table.Column<string>(type: "NCHAR(16)", nullable: true),
                    photo = table.Column<byte[]>(type: "IMAGE", nullable: true),
                    group = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.stud_id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    teacher_id = table.Column<short>(type: "SMALLINT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    last_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    first_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    middle_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    birth_date = table.Column<DateTime>(type: "DATE", nullable: true),
                    email = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    phone = table.Column<string>(type: "NCHAR(16)", nullable: true),
                    photo = table.Column<byte[]>(type: "IMAGE", nullable: true),
                    work_since = table.Column<DateTime>(type: "DATE", nullable: true),
                    rate = table.Column<decimal>(type: "SMALLMONEY", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.teacher_id);
                });

            migrationBuilder.CreateTable(
                name: "TeachersDisciplinesRelation",
                columns: table => new
                {
                    teacher = table.Column<short>(type: "SMALLINT", nullable: false),
                    discipline = table.Column<short>(type: "SMALLINT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachersDisciplinesRelation", x => new { x.teacher, x.discipline });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "NVARCHAR(50)", nullable: false),
                    password_hash = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    role_id = table.Column<byte>(type: "TINYINT", nullable: false),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    full_name = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    contact = table.Column<string>(type: "NVARCHAR(100)", nullable: true),
                    linked_id = table.Column<int>(type: "INT", nullable: true),
                    photo = table.Column<byte[]>(type: "IMAGE", nullable: true),
                    banner = table.Column<byte[]>(type: "IMAGE", nullable: true),
                    bio = table.Column<string>(type: "NVARCHAR(300)", nullable: true),
                    tagline = table.Column<string>(type: "NVARCHAR(80)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    vacancy_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    discipline_id = table.Column<int>(type: "INT", nullable: true),
                    is_open = table.Column<bool>(type: "BIT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.vacancy_id);
                });

            migrationBuilder.CreateTable(
                name: "VacancyApplications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_user_id = table.Column<int>(type: "INT", nullable: false),
                    vacancy_id = table.Column<int>(type: "INT", nullable: false),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyApplications", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionRequests");

            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "CandidateRequests");

            migrationBuilder.DropTable(
                name: "CompleteDisciplines");

            migrationBuilder.DropTable(
                name: "DaysOFF");

            migrationBuilder.DropTable(
                name: "DependentDisciplines");

            migrationBuilder.DropTable(
                name: "Directions");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "DisciplinesDirectionsRelation");

            migrationBuilder.DropTable(
                name: "Exams");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Holidays");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "RequiredDisciplines");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Salary");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "TeachersDisciplinesRelation");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "VacancyApplications");
        }
    }
}
