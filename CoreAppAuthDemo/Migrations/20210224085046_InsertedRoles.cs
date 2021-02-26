using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreAppAuthDemo.Migrations
{
    public partial class InsertedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b6abd2fd-a694-480c-9d2c-6f4636bfd767", "6e925194-fc1d-49a9-a4f7-291f19ddd7ca", "Operator", "OPERATOR" },
                    { "d26b6f36-59b3-4bc1-b993-fd95d30b710c", "940a62d9-15ce-4669-b93e-b2908cac58a5", "Planner", "PLANNER" },
                    { "faa7d2b3-12b2-4b20-9e5d-18eb19add65c", "c7217594-197b-431a-9087-dd2c7f009692", "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Age", "Name", "Position" },
                values: new object[,]
                {
                    { new Guid("e310a6cb-6677-4aa6-93c7-2763956f7a97"), 26, "Deepak Brijwasi", "Software Developer" },
                    { new Guid("398d10fe-4b8d-4606-8e9c-bd2c78d4e001"), 25, "Prince kumar Sahoo", "Software Developer" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b6abd2fd-a694-480c-9d2c-6f4636bfd767");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d26b6f36-59b3-4bc1-b993-fd95d30b710c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "faa7d2b3-12b2-4b20-9e5d-18eb19add65c");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("398d10fe-4b8d-4606-8e9c-bd2c78d4e001"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("e310a6cb-6677-4aa6-93c7-2763956f7a97"));
        }
    }
}
