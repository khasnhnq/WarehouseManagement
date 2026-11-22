using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public class SuppliersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SupplierResponse>>> GetAll() =>
        Ok(await db.Suppliers.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new SupplierResponse(x.Id, x.Name, x.ContactName, x.Phone, x.Email, x.Address)).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierResponse>> GetById(int id)
    {
        var x = await db.Suppliers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return x is null ? NotFound() : Ok(new SupplierResponse(x.Id, x.Name, x.ContactName, x.Phone, x.Email, x.Address));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> Create(SupplierRequest request)
    {
        var x = new Supplier { Name = request.Name.Trim(), ContactName = request.ContactName, Phone = request.Phone, Email = request.Email, Address = request.Address };
        db.Suppliers.Add(x); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = x.Id }, new SupplierResponse(x.Id, x.Name, x.ContactName, x.Phone, x.Email, x.Address));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SupplierRequest request)
    {
        var x = await db.Suppliers.FindAsync(id); if (x is null) return NotFound();
        x.Name = request.Name.Trim(); x.ContactName = request.ContactName; x.Phone = request.Phone; x.Email = request.Email; x.Address = request.Address;
        await db.SaveChangesAsync(); return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var x = await db.Suppliers.FindAsync(id); if (x is null) return NotFound();
        if (await db.Products.AnyAsync(p => p.SupplierId == id)) return Conflict(new { message = "Nhà cung cấp đang được sản phẩm tham chiếu." });
        db.Suppliers.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }
}
