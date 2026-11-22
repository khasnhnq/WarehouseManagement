using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>().HasIndex(x => x.Username).IsUnique();
        modelBuilder.Entity<AppUser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<AppUser>().Property(x => x.Username).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<AppUser>().Property(x => x.Email).HasMaxLength(120).IsRequired();
        modelBuilder.Entity<AppUser>().Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        modelBuilder.Entity<AppUser>().Property(x => x.Role).HasMaxLength(20).IsRequired();

        modelBuilder.Entity<Category>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<Category>().Property(x => x.Name).HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Supplier>().Property(x => x.Name).HasMaxLength(150).IsRequired();
        modelBuilder.Entity<Warehouse>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Warehouse>().Property(x => x.Code).HasMaxLength(30).IsRequired();
        modelBuilder.Entity<Warehouse>().Property(x => x.Name).HasMaxLength(150).IsRequired();

        modelBuilder.Entity<Product>().HasIndex(x => x.SKU).IsUnique();
        modelBuilder.Entity<Product>().Property(x => x.SKU).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<Product>().Property(x => x.Name).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<Product>().Property(x => x.Unit).HasMaxLength(30).IsRequired();
        modelBuilder.Entity<Product>().Property(x => x.CostPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(x => x.SellingPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(x => x.MinStockLevel).HasPrecision(18, 3);
        modelBuilder.Entity<Product>()
            .HasOne(x => x.Category).WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Product>()
            .HasOne(x => x.Supplier).WithMany(x => x.Products)
            .HasForeignKey(x => x.SupplierId).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Stock>().HasIndex(x => new { x.ProductId, x.WarehouseId }).IsUnique();
        modelBuilder.Entity<Stock>().Property(x => x.Quantity).HasPrecision(18, 3);
        modelBuilder.Entity<Stock>()
            .HasOne(x => x.Product).WithMany(x => x.Stocks)
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Stock>()
            .HasOne(x => x.Warehouse).WithMany(x => x.Stocks)
            .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>().Property(x => x.MovementType).HasMaxLength(10).IsRequired();
        modelBuilder.Entity<StockMovement>().Property(x => x.Quantity).HasPrecision(18, 3);
        modelBuilder.Entity<StockMovement>()
            .HasOne(x => x.Product).WithMany(x => x.StockMovements)
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StockMovement>()
            .HasOne(x => x.Warehouse).WithMany()
            .HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StockMovement>()
            .HasOne(x => x.CreatedByUser).WithMany()
            .HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
    }
}
