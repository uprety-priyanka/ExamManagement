using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Server.Migrations
{
    public partial class UpdateRollNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RollNumber",
                table: "UserDetailExtension",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RollNumber",
                table: "UserDetailExtension");
        }
    }
}
