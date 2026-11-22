using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.DTOs.Common;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetAll([FromQuery] ProductQueryParameters parameters)
    {
        var pageNumber = Math.Max(parameters.PageNumber, 1);
        var pageSize = Math.Clamp(parameters.PageSize, 1, 100);
        var query = db.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Supplier).AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.Trim();
            query = query.Where(x => x.Name.Contains(term) || x.SKU.Contains(term));
        }
        if (parameters.CategoryId.HasValue) query = query.Where(x => x.CategoryId == parameters.CategoryId.Value);

        query = parameters.SortBy?.Trim().ToLowerInvariant() switch
        {
            "id" => parameters.IsDescending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            "name" => parameters.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "sku" => parameters.IsDescending ? query.OrderByDescending(x => x.SKU) : query.OrderBy(x => x.SKU),
            "price" => parameters.IsDescending ? query.OrderByDescending(x => x.SellingPrice) : query.OrderBy(x => x.SellingPrice),
            "createdat" => parameters.IsDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => parameters.IsDescending ? query.OrderByDescending(x => x.SKU) : query.OrderBy(x => x.SKU)
        };

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(x => new ProductResponse(
                x.Id, x.SKU, x.Name, x.Unit, x.CostPrice, x.SellingPrice, x.MinStockLevel,
                x.IsActive, x.CategoryId, x.Category!.Name, x.SupplierId,
                x.Supplier == null ? null : x.Supplier.Name, x.CreatedAt))
            .ToListAsync();
        return Ok(new PagedResult<ProductResponse>(items, totalCount, pageNumber, pageSize));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var x = await db.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (x is null) return NotFound();
        return Ok(ToResponse(x));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(ProductRequest request)
    {
        if (await db.Products.AnyAsync(x => x.SKU == request.SKU.Trim())) return Conflict(new { message = "SKU đã tồn tại." });
        if (!await db.Categories.AnyAsync(x => x.Id == request.CategoryId)) return BadRequest(new { message = "CategoryId không tồn tại." });
        if (request.SupplierId.HasValue && !await db.Suppliers.AnyAsync(x => x.Id == request.SupplierId.Value)) return BadRequest(new { message = "SupplierId không tồn tại." });

        var x = new Product
        {
            SKU = request.SKU.Trim(), Name = request.Name.Trim(), Unit = request.Unit.Trim(),
            CostPrice = request.CostPrice, SellingPrice = request.SellingPrice,
            MinStockLevel = request.MinStockLevel, IsActive = request.IsActive,
            CategoryId = request.CategoryId, SupplierId = request.SupplierId
        };
        db.Products.Add(x); await db.SaveChangesAsync();
        await db.Entry(x).Reference(p => p.Category).LoadAsync();
        await db.Entry(x).Reference(p => p.Supplier).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = x.Id }, ToResponse(x));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var x = await db.Products.FindAsync(id); if (x is null) return NotFound();
        var sku = request.SKU.Trim();
        if (await db.Products.AnyAsync(p => p.Id != id && p.SKU == sku)) return Conflict(new { message = "SKU đã tồn tại." });
        if (!await db.Categories.AnyAsync(c => c.Id == request.CategoryId)) return BadRequest(new { message = "CategoryId không tồn tại." });
        if (request.SupplierId.HasValue && !await db.Suppliers.AnyAsync(s => s.Id == request.SupplierId.Value)) return BadRequest(new { message = "SupplierId không tồn tại." });

        x.SKU = sku; x.Name = request.Name.Trim(); x.Unit = request.Unit.Trim();
        x.CostPrice = request.CostPrice; x.SellingPrice = request.SellingPrice;
        x.MinStockLevel = request.MinStockLevel; x.IsActive = request.IsActive;
        x.CategoryId = request.CategoryId; x.SupplierId = request.SupplierId;
        await db.SaveChangesAsync(); return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var x = await db.Products.FindAsync(id); if (x is null) return NotFound();
        if (await db.Stocks.AnyAsync(s => s.ProductId == id) || await db.StockMovements.AnyAsync(m => m.ProductId == id))
            return Conflict(new { message = "Sản phẩm đã có tồn kho hoặc lịch sử giao dịch. Hãy ngừng kinh doanh thay vì xóa." });
        db.Products.Remove(x); await db.SaveChangesAsync(); return NoContent();
    }

    private static ProductResponse ToResponse(Product x) => new(
        x.Id, x.SKU, x.Name, x.Unit, x.CostPrice, x.SellingPrice, x.MinStockLevel,
        x.IsActive, x.CategoryId, x.Category?.Name ?? "", x.SupplierId,
        x.Supplier?.Name, x.CreatedAt);
}
