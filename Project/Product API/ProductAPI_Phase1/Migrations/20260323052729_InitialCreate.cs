using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductAPI_Phase1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products_tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Prod_Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProdImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products_tbl", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products_tbl",
                columns: new[] { "Id", "Category", "CostPrice", "CreatedAt", "CreatedBy", "Description", "Price", "ProdImageUrl", "Prod_Name", "ProductId", "Stock", "SupplierId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Electronics", 950.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "High performance laptop with 16GB RAM and 512GB SSD", 1299.99m, "https://example.com/laptop.jpg", "Laptop Dell XPS 15", 12, 25, 101, null, null },
                    { 2, "Electronics", 60.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Ergonomic wireless mouse with precision scrolling", 99.99m, "https://example.com/mouse.jpg", "Wireless Mouse Logitech MX Master 3", 15, 150, 102, null, null },
                    { 3, "Furniture", 890.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Premium ergonomic office chair with lumbar support", 1495.00m, "https://example.com/chair.jpg", "Herman Miller Aeron Chair", 18, 15, 103, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products_tbl");
        }
    }
}
