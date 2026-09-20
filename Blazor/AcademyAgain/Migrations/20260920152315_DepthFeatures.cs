using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class DepthFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "duration_min",
                table: "Schedule",
                type: "TINYINT",
                nullable: false,
                defaultValue: (byte)90);

            migrationBuilder.AddColumn<short>(
                name: "semester_id",
                table: "Schedule",
                type: "SMALLINT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SessionRules",
                columns: table => new
                {
                    session_rule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    min_attendance_pct = table.Column<byte>(type: "TINYINT", nullable: false),
                    min_average_grade = table.Column<decimal>(type: "DECIMAL(3,2)", nullable: false),
                    required_makeups = table.Column<byte>(type: "TINYINT", nullable: false),
                    is_active = table.Column<bool>(type: "BIT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRules", x => x.session_rule_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionRules");

            migrationBuilder.DropColumn(
                name: "duration_min",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "semester_id",
                table: "Schedule");
        }
    }
}
