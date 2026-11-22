using System.ComponentModel.DataAnnotations;

namespace WarehouseManagementAPI.DTOs;

public class CategoryRequest
{
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}
public record CategoryResponse(int Id, string Name, string? Description, DateTime CreatedAt);

public class SupplierRequest
{
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(100)] public string? ContactName { get; set; }
    [Phone, MaxLength(30)] public string? Phone { get; set; }
    [EmailAddress, MaxLength(120)] public string? Email { get; set; }
    [MaxLength(300)] public string? Address { get; set; }
}
public record SupplierResponse(int Id, string Name, string? ContactName, string? Phone, string? Email, string? Address);

public class WarehouseRequest
{
    [Required, MaxLength(30)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string Name { get; set; } = string.Empty;
    [MaxLength(300)] public string? Address { get; set; }
}
public record WarehouseResponse(int Id, string Code, string Name, string? Address);
