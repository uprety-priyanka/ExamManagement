using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Server.Migrations
{
    public partial class AddedItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SemesterTypeId",
                table: "Course",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ItemTypeCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: false),
                    NormalizedCategoryName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypeCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ItemTypeCategoryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemType_ItemTypeCategory_ItemTypeCategoryId",
                        column: x => x.ItemTypeCategoryId,
                        principalTable: "ItemTypeCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_SemesterTypeId",
                table: "Course",
                column: "SemesterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemType_ItemTypeCategoryId",
                table: "ItemType",
                column: "ItemTypeCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_ItemType_SemesterTypeId",
                table: "Course",
                column: "SemesterTypeId",
                principalTable: "ItemType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_ItemType_SemesterTypeId",
                table: "Course");

            migrationBuilder.DropTable(
                name: "ItemType");

            migrationBuilder.DropTable(
                name: "ItemTypeCategory");

            migrationBuilder.DropIndex(
                name: "IX_Course_SemesterTypeId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SemesterTypeId",
                table: "Course");
        }
    }
}
