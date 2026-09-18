using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProvaPub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Numbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Clayton Will" },
                    { 2, "Verna Paucek" },
                    { 3, "Ismael Beier" },
                    { 4, "Karla Bashirian" },
                    { 5, "Brooke Langosh" },
                    { 6, "Maria Watsica" },
                    { 7, "Rudy Ledner" },
                    { 8, "Gabriel Goodwin" },
                    { 9, "Kenny Hoppe" },
                    { 10, "Jennifer Kreiger" },
                    { 11, "Perry Prosacco" },
                    { 12, "Gladys Schaefer" },
                    { 13, "Tiffany Prohaska" },
                    { 14, "Elizabeth Pfannerstill" },
                    { 15, "Danielle Harris" },
                    { 16, "Guadalupe Hauck" },
                    { 17, "Wayne Huels" },
                    { 18, "Barbara Dach" },
                    { 19, "Jill Stehr" },
                    { 20, "Angel Schaefer" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Refined Cotton Computer" },
                    { 2, "Intelligent Wooden Hat" },
                    { 3, "Licensed Granite Gloves" },
                    { 4, "Licensed Cotton Chips" },
                    { 5, "Fantastic Frozen Gloves" },
                    { 6, "Ergonomic Concrete Salad" },
                    { 7, "Rustic Concrete Sausages" },
                    { 8, "Sleek Fresh Chicken" },
                    { 9, "Fantastic Wooden Bacon" },
                    { 10, "Gorgeous Steel Car" },
                    { 11, "Awesome Granite Salad" },
                    { 12, "Handcrafted Plastic Salad" },
                    { 13, "Sleek Soft Chair" },
                    { 14, "Sleek Fresh Sausages" },
                    { 15, "Practical Concrete Hat" },
                    { 16, "Incredible Granite Ball" },
                    { 17, "Sleek Fresh Gloves" },
                    { 18, "Ergonomic Wooden Towels" },
                    { 19, "Rustic Wooden Pizza" },
                    { 20, "Tasty Rubber Gloves" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Numbers_Number",
                table: "Numbers",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Numbers");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
