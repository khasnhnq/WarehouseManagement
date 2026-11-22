using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = AppRoles.AdminOrStaff)]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummary>> GetSummary()
    {
        var result = new DashboardSummary(
            await db.Products.CountAsync(x => x.IsActive),
            await db.Categories.CountAsync(),
            await db.Suppliers.CountAsync(),
            await db.Warehouses.CountAsync(),
            await db.Users.CountAsync(x => x.IsActive),
            await db.Stocks.SumAsync(x => (decimal?)x.Quantity) ?? 0,
            await db.Stocks.CountAsync(x => x.Quantity <= x.Product!.MinStockLevel),
            await db.StockMovements.CountAsync());
        return Ok(result);
    }
}
