using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Data;

public static class DatabaseSeeder
{
    public static async Task InitializeAsync(
        AppDbContext db,
        IServiceProvider services,
        IConfiguration configuration)
    {
        await db.Database.EnsureCreatedAsync();

        var hasher = services.GetRequiredService<IPasswordHasher<AppUser>>();

        var adminUsername = configuration["SeedAdmin:Username"] ?? "admin";
        if (!await db.Users.AnyAsync(x => x.Username == adminUsername))
        {
            var admin = new AppUser
            {
                Username = adminUsername,
                Email = configuration["SeedAdmin:Email"] ?? "admin@warehouse.local",
                Role = AppRoles.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var password = configuration["SeedAdmin:Password"] ?? "Admin@12345";
            admin.PasswordHash = hasher.HashPassword(admin, password);
            db.Users.Add(admin);
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { Name = "Thiết bị điện tử", Description = "Máy tính và thiết bị điện tử" },
                new Category { Name = "Văn phòng phẩm", Description = "Dụng cụ văn phòng" },
                new Category { Name = "Phụ kiện", Description = "Phụ kiện và linh kiện" });
        }

        if (!await db.Suppliers.AnyAsync())
        {
            db.Suppliers.Add(new Supplier
            {
                Name = "Nhà cung cấp mẫu",
                ContactName = "Nguyễn Văn A",
                Phone = "0900000000",
                Email = "supplier@example.com",
                Address = "Đà Nẵng"
            });
        }

        if (!await db.Warehouses.AnyAsync())
        {
            db.Warehouses.Add(new Warehouse
            {
                Code = "WH-HQ",
                Name = "Kho trung tâm",
                Address = "Đà Nẵng"
            });
        }

        await db.SaveChangesAsync();

        if (!await db.Products.AnyAsync())
        {
            var category = await db.Categories.OrderBy(x => x.Id).FirstAsync();
            var supplier = await db.Suppliers.OrderBy(x => x.Id).FirstAsync();
            db.Products.AddRange(
                new Product
                {
                    SKU = "SP-001", Name = "Laptop Demo", Unit = "cái",
                    CostPrice = 12000000, SellingPrice = 14500000,
                    MinStockLevel = 5, CategoryId = category.Id, SupplierId = supplier.Id
                },
                new Product
                {
                    SKU = "SP-002", Name = "Chuột máy tính Demo", Unit = "cái",
                    CostPrice = 120000, SellingPrice = 180000,
                    MinStockLevel = 10, CategoryId = category.Id, SupplierId = supplier.Id
                });
            await db.SaveChangesAsync();
        }

        if (!await db.Stocks.AnyAsync())
        {
            var warehouse = await db.Warehouses.OrderBy(x => x.Id).FirstAsync();
            var products = await db.Products.OrderBy(x => x.Id).ToListAsync();
            foreach (var product in products)
            {
                db.Stocks.Add(new Stock
                {
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    Quantity = product.SKU == "SP-001" ? 20 : 50,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await db.SaveChangesAsync();
        }
    }
}
