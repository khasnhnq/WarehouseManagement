using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll() =>
        Ok(await db.Categories.AsNoTracking().OrderBy(x => x.Id)
            .Select(x => new CategoryResponse(x.Id, x.Name, x.Description, x.CreatedAt)).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var item = await db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item is null ? NotFound() : Ok(new CategoryResponse(item.Id, item.Name, item.Description, item.CreatedAt));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(x => x.Name == name)) return Conflict(new { message = "Tên danh mục đã tồn tại." });
        var item = new Category { Name = name, Description = request.Description };
        db.Categories.Add(item);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, new CategoryResponse(item.Id, item.Name, item.Description, item.CreatedAt));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryRequest request)
    {
        var item = await db.Categories.FindAsync(id);
        if (item is null) return NotFound();
        var name = request.Name.Trim();
        if (await db.Categories.AnyAsync(x => x.Id != id && x.Name == name)) return Conflict(new { message = "Tên danh mục đã tồn tại." });
        item.Name = name; item.Description = request.Description;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Categories.FindAsync(id);
        if (item is null) return NotFound();
        if (await db.Products.AnyAsync(x => x.CategoryId == id)) return Conflict(new { message = "Danh mục đang được sản phẩm sử dụng." });
        db.Categories.Remove(item); await db.SaveChangesAsync(); return NoContent();
    }
}
