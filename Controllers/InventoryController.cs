using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.DTOs.Common;
using WarehouseManagementAPI.Models;
using WarehouseManagementAPI.Services;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Roles = AppRoles.AdminOrStaff)]
public class InventoryController(InventoryService inventory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StockResponse>>> GetStocks(
        [FromQuery] int? warehouseId, [FromQuery] int? productId)
        => Ok(await inventory.GetStocksAsync(warehouseId, productId));

    [HttpGet("movements")]
    public async Task<ActionResult<PagedResult<StockMovementResponse>>> GetMovements(
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        => Ok(await inventory.GetMovementsAsync(pageNumber, pageSize));

    [HttpPost("receipts")]
    public async Task<ActionResult<StockMovementResponse>> Receive(StockMovementRequest request)
    {
        try
        {
            var result = await inventory.ReceiveAsync(request, GetCurrentUserId());
            return Ok(new { message = "Nhập kho thành công.", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("issues")]
    public async Task<ActionResult<StockMovementResponse>> Issue(StockMovementRequest request)
    {
        try
        {
            var result = await inventory.IssueAsync(request, GetCurrentUserId());
            return Ok(new { message = "Xuất kho thành công.", data = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int? GetCurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
