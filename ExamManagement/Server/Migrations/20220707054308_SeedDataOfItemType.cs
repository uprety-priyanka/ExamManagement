using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamManagement.Server.Migrations
{
    public partial class SeedDataOfItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ItemTypeCategory",
                columns: new[] { "Id", "CategoryName", "NormalizedCategoryName" },
                values: new object[] { 1, "Semester", "SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 1, 1, "FIRST SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 2, 1, "SECOND SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 3, 1, "THIRD SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 4, 1, "FOURTH SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 5, 1, "FIFTH SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 6, 1, "SIXTH SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 7, 1, "SEVENTH SEMESTER" });

            migrationBuilder.InsertData(
                table: "ItemType",
                columns: new[] { "Id", "ItemTypeCategoryId", "Name" },
                values: new object[] { 8, 1, "EIGHTH SEMESTER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ItemType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ItemTypeCategory",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
