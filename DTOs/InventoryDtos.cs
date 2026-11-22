using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementAPI.DTOs;

public class StockMovementRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(1, int.MaxValue)] public int WarehouseId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public decimal Quantity { get; set; }
    [MaxLength(50)] public string? ReferenceNo { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}

public record StockResponse(
    int ProductId, string SKU, string ProductName,
    int WarehouseId, string WarehouseName, string Unit,
    decimal Quantity, decimal MinStockLevel, bool IsLowStock, DateTime UpdatedAt);

public record StockMovementResponse(
    int Id, string MovementType, int ProductId, string SKU, string ProductName,
    int WarehouseId, string WarehouseName, decimal Quantity,
    string? ReferenceNo, string? Notes, string? CreatedBy, DateTime CreatedAt);

public record DashboardSummary(
    int TotalProducts, int TotalCategories, int TotalSuppliers,
    int TotalWarehouses, int TotalUsers, decimal TotalStockQuantity,
    int LowStockLines, int TotalMovements);
