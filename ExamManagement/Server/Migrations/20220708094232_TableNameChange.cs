using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Server.Migrations
{
    public partial class TableNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetailExtensionAbove");

            migrationBuilder.CreateTable(
                name: "UserDetailExtensionStudentTemporary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Semester = table.Column<int>(type: "INTEGER", nullable: false),
                    ExamYear = table.Column<int>(type: "INTEGER", nullable: false),
                    UserDetailExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetailExtensionStudentTemporary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetailExtensionStudentTemporary_UserDetailExtension_UserDetailExtensionId",
                        column: x => x.UserDetailExtensionId,
                        principalTable: "UserDetailExtension",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailExtensionStudentTemporary_UserDetailExtensionId",
                table: "UserDetailExtensionStudentTemporary",
                column: "UserDetailExtensionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDetailExtensionStudentTemporary");

            migrationBuilder.CreateTable(
                name: "UserDetailExtensionAbove",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserDetailExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExamYear = table.Column<int>(type: "INTEGER", nullable: false),
                    Semester = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetailExtensionAbove", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetailExtensionAbove_UserDetail_UserDetailExtensionId",
                        column: x => x.UserDetailExtensionId,
                        principalTable: "UserDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetailExtensionAbove_UserDetailExtensionId",
                table: "UserDetailExtensionAbove",
                column: "UserDetailExtensionId");
        }
    }
}
