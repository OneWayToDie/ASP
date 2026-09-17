using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class EmailVerificationAndNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Users",
                type: "NVARCHAR(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "Users",
                type: "NVARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                table: "Users",
                type: "NVARCHAR(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "middle_name",
                table: "Users",
                type: "NVARCHAR(50)",
                nullable: true);

            // Backfill the three name columns from the legacy single full_name
            // before it is dropped. last = 1st word, first = 2nd word,
            // middle = everything after the 2nd word.
            migrationBuilder.Sql(@"
UPDATE u SET
    last_name  = CASE WHEN b.i1 = 0 THEN b.s ELSE LEFT(b.s, b.i1 - 1) END,
    first_name = CASE WHEN d.i2 = 0 THEN d.r1 ELSE LEFT(d.r1, d.i2 - 1) END,
    middle_name = NULLIF(CASE WHEN d.i2 = 0 THEN N'' ELSE LTRIM(SUBSTRING(d.r1, d.i2 + 1, LEN(d.r1))) END, N'')
FROM [Users] u
CROSS APPLY (SELECT s = LTRIM(RTRIM(ISNULL(u.full_name, N'')))) a
CROSS APPLY (SELECT s = a.s, i1 = CHARINDEX(N' ', a.s)) b
CROSS APPLY (SELECT r1 = CASE WHEN b.i1 = 0 THEN N'' ELSE LTRIM(SUBSTRING(b.s, b.i1 + 1, LEN(b.s))) END) c
CROSS APPLY (SELECT r1 = c.r1, i2 = CHARINDEX(N' ', c.r1)) d;
");

            migrationBuilder.DropColumn(
                name: "full_name",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "EmailVerificationCodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    purpose = table.Column<byte>(type: "TINYINT", nullable: false),
                    code_hash = table.Column<string>(type: "NVARCHAR(200)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    consumed = table.Column<bool>(type: "BIT", nullable: false),
                    attempts = table.Column<byte>(type: "TINYINT", nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: true),
                    pending_username = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    pending_password_hash = table.Column<string>(type: "NVARCHAR(200)", nullable: true),
                    pending_role_id = table.Column<byte>(type: "TINYINT", nullable: true),
                    pending_last_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    pending_first_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    pending_middle_name = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    pending_phone = table.Column<string>(type: "NVARCHAR(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerificationCodes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailVerificationCodes_email_purpose_consumed",
                table: "EmailVerificationCodes",
                columns: new[] { "email", "purpose", "consumed" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerificationCodes");

            migrationBuilder.DropIndex(
                name: "IX_Users_email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "Users",
                type: "NVARCHAR(150)",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE [Users]
SET [full_name] = NULLIF(LTRIM(RTRIM(
        ISNULL([last_name], N'') + N' ' +
        ISNULL([first_name], N'') + N' ' +
        ISNULL([middle_name], N''))), N'');
");

            migrationBuilder.DropColumn(
                name: "first_name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "last_name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "middle_name",
                table: "Users");
        }
    }
}
