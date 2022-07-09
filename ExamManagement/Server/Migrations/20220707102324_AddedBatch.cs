using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Server.Migrations
{
    public partial class AddedBatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Batch",
                table: "UserDetailExtension",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Batch",
                table: "UserDetailExtension");
        }
    }
}
