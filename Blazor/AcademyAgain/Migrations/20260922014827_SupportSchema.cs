using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcademyAgain.Migrations
{
    /// <inheritdoc />
    public partial class SupportSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupportBans",
                columns: table => new
                {
                    ban_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chat_id = table.Column<int>(type: "INT", nullable: false),
                    user_id = table.Column<int>(type: "INT", nullable: false),
                    moderator_user_id = table.Column<int>(type: "INT", nullable: true),
                    reason = table.Column<string>(type: "NVARCHAR(300)", nullable: true),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportBans", x => x.ban_id);
                });

            migrationBuilder.CreateTable(
                name: "SupportChats",
                columns: table => new
                {
                    chat_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "INT", nullable: false),
                    status = table.Column<byte>(type: "TINYINT", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    updated_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportChats", x => x.chat_id);
                });

            migrationBuilder.CreateTable(
                name: "SupportMessages",
                columns: table => new
                {
                    msg_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    chat_id = table.Column<int>(type: "INT", nullable: false),
                    author_user_id = table.Column<int>(type: "INT", nullable: false),
                    body = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    created_at = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportMessages", x => x.msg_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportChats_user_id",
                table: "SupportChats",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_SupportMessages_chat_id",
                table: "SupportMessages",
                column: "chat_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportBans");

            migrationBuilder.DropTable(
                name: "SupportChats");

            migrationBuilder.DropTable(
                name: "SupportMessages");
        }
    }
}
