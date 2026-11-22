namespace WarehouseManagementAPI.Models;

public class StockMovement
{
    public int Id { get; set; }
    public string MovementType { get; set; } = string.Empty; // IMPORT hoặc EXPORT
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public decimal Quantity { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
    public int? CreatedByUserId { get; set; }
    public AppUser? CreatedByUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
