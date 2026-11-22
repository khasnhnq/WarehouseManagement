using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public class WarehousesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WarehouseResponse>>> GetAll() =>
        Ok(await db.Warehouses.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new WarehouseResponse(x.Id, x.Code, x.Name, x.Address)).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseResponse>> GetById(int id)
    {
        var x = await db.Warehouses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return x is null ? NotFound() : Ok(new WarehouseResponse(x.Id, x.Code, x.Name, x.Address));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<WarehouseResponse>> Create(WarehouseRequest request)
    {
        var code = request.Code.Trim();
        if (await db.Warehouses.AnyAsync(x => x.Code == code)) return Conflict(new { message = "Mã kho đã tồn tại." });
        var x = new Warehouse { Code = code, Name = request.Name.Trim(), Address = request.Address };
        db.Warehouses.Add(x); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = x.Id }, new WarehouseResponse(x.Id, x.Code, x.Name, x.Address));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, WarehouseRequest request)
    {
        var x = await db.Warehouses.FindAsync(id); if (x is null) return NotFound();
        var code = request.Code.Trim();
        if (await db.Warehouses.AnyAsync(w => w.Id != id && w.Code == code)) return Conflict(new { message = "Mã kho đã tồn tại." });
        x.Code = code; x.Name = request.Name.Trim(); x.Address = request.Address;
        await db.SaveChangesAsync(); return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var x = await db.Warehouses.FindAsync(id); if (x is null) return NotFound();
        if (await db.Stocks.AnyAsync(s => s.WarehouseId == id) || await db.StockMovements.AnyAsync(m => m.WarehouseId == id))
            return Conflict(new { message = "Kho đang có tồn kho hoặc lịch sử giao dịch, không thể xóa." });
        db.Warehouses.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
