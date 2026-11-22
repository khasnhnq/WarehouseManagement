using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementAPI.DTOs;

public class ProductRequest
{
    [Required, MaxLength(50)] public string SKU { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Unit { get; set; } = "cái";
    [Range(typeof(decimal), "0", "999999999999")] public decimal CostPrice { get; set; }
    [Range(typeof(decimal), "0", "999999999999")] public decimal SellingPrice { get; set; }
    [Range(typeof(decimal), "0", "999999999999")] public decimal MinStockLevel { get; set; }
    [Range(1, int.MaxValue)] public int CategoryId { get; set; }
    public int? SupplierId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ProductQueryParameters
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public string? SortBy { get; set; } = "sku";
    public bool IsDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public record ProductResponse(
    int Id, string SKU, string Name, string Unit,
    decimal CostPrice, decimal SellingPrice, decimal MinStockLevel,
    bool IsActive, int CategoryId, string CategoryName,
    int? SupplierId, string? SupplierName, DateTime CreatedAt);
