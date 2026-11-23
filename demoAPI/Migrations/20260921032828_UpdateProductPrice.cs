using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace demoAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Điện thoại" },
                    { 2, "Laptop" },
                    { 3, "Phụ kiện" },
                    { 4, "Máy tính bảng" }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "", "Samsung Việt Nam", "0901234567" },
                    { 2, "Hồ Chí Minh", "", "Apple Việt Nam", "0902345678" },
                    { 3, "Đà Nẵng", "", "Dell Việt Nam", "0903456789" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Name", "Price", "Quantity", "SKU", "SupplierId" },
                values: new object[,]
                {
                    { 1, 1, "Samsung Galaxy S25", 22000000m, 20, "SS-S25-001", 1 },
                    { 2, 1, "iPhone 16", 24000000m, 15, "IP16-001", 2 },
                    { 3, 2, "Dell Inspiron 15", 18000000m, 10, "DELL-IN15-001", 3 },
                    { 4, 2, "MacBook Air M3", 27000000m, 8, "MBA-M3-001", 2 },
                    { 5, 3, "Chuột Logitech M331", 500000m, 50, "LOG-M331-001", 3 },
                    { 6, 4, "iPad Air M2", 16000000m, 12, "IPAD-AIR-M2", 2 }
                });
        }
    }
}
