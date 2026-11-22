
using System.Data;
using Microsoft.EntityFrameworkCore;
using WarehouseManagementAPI.Data;
using WarehouseManagementAPI.DTOs;
using WarehouseManagementAPI.DTOs.Common;
using WarehouseManagementAPI.Models;

namespace WarehouseManagementAPI.Services;

public class InventoryService(AppDbContext db)
{
    public async Task<StockMovementResponse> ReceiveAsync(
        StockMovementRequest request,
        int? userId)
        => await MoveStockAsync(request, userId, "IMPORT");

    public async Task<StockMovementResponse> IssueAsync(
        StockMovementRequest request,
        int? userId)
        => await MoveStockAsync(request, userId, "EXPORT");

    // =============================================
    // NHAP KHO / XUAT KHO
    // =============================================
    private async Task<StockMovementResponse> MoveStockAsync(
        StockMovementRequest request,
        int? userId,
        string movementType)
    {
        // Lấy Execution Strategy của SQL Server
        var strategy = db.Database.CreateExecutionStrategy();

        // Thực hiện toàn bộ Transaction bên trong Strategy
        return await strategy.ExecuteAsync(async () =>
        {
            // Xóa trạng thái tracking của lần thử trước
            db.ChangeTracker.Clear();

            // Bắt đầu Transaction
            await using var transaction =
                await db.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            // 1. Kiểm tra sản phẩm
            var product = await db.Products
                .FirstOrDefaultAsync(x =>
                    x.Id == request.ProductId &&
                    x.IsActive);

            if (product == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy sản phẩm hoặc sản phẩm đã ngừng kinh doanh.");
            }

            // 2. Kiểm tra kho hàng
            var warehouse = await db.Warehouses
                .FirstOrDefaultAsync(x =>
                    x.Id == request.WarehouseId);

            if (warehouse == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy kho hàng.");
            }

            // 3. Tìm tồn kho của sản phẩm tại kho được chọn
            var stock = await db.Stocks
                .SingleOrDefaultAsync(x =>
                    x.ProductId == request.ProductId &&
                    x.WarehouseId == request.WarehouseId);

            // 4. Nếu chưa có tồn kho
            if (stock == null)
            {
                // Không cho xuất sản phẩm chưa có tồn kho
                if (movementType == "EXPORT")
                {
                    throw new InvalidOperationException(
                        "Sản phẩm chưa có tồn kho tại kho này.");
                }

                // Tạo dòng tồn kho mới khi nhập lần đầu
                stock = new Stock
                {
                    ProductId = request.ProductId,
                    WarehouseId = request.WarehouseId,
                    Quantity = 0,
                    UpdatedAt = DateTime.UtcNow
                };

                db.Stocks.Add(stock);
            }

            // 5. Kiểm tra số lượng trước khi xuất
            if (movementType == "EXPORT" &&
                stock.Quantity < request.Quantity)
            {
                throw new InvalidOperationException(
                    $"Không đủ tồn kho. Tồn hiện tại: {stock.Quantity} {product.Unit}.");
            }

            // 6. Cập nhật số lượng tồn kho
            if (movementType == "IMPORT")
            {
                stock.Quantity += request.Quantity;
            }
            else
            {
                stock.Quantity -= request.Quantity;
            }

            stock.UpdatedAt = DateTime.UtcNow;

            // 7. Tạo lịch sử nhập / xuất
            var movement = new StockMovement
            {
                MovementType = movementType,
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                Quantity = request.Quantity,
                ReferenceNo = request.ReferenceNo,
                Notes = request.Notes,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            db.StockMovements.Add(movement);

            // 8. Lưu thay đổi vào Database
            await db.SaveChangesAsync();

            // 9. Lấy tên người thực hiện
            var username = userId.HasValue
                ? await db.Users
                    .Where(x => x.Id == userId.Value)
                    .Select(x => x.Username)
                    .FirstOrDefaultAsync()
                : null;

            // 10. Tạo kết quả trả về
            var response = new StockMovementResponse(
                movement.Id,
                movement.MovementType,
                product.Id,
                product.SKU,
                product.Name,
                warehouse.Id,
                warehouse.Name,
                movement.Quantity,
                movement.ReferenceNo,
                movement.Notes,
                username,
                movement.CreatedAt
            );

            // 11. Xác nhận Transaction
            await transaction.CommitAsync();

            return response;
        });
    }

    // =============================================
    // LAY DANH SACH TON KHO
    // =============================================
    public async Task<List<StockResponse>> GetStocksAsync(
        int? warehouseId,
        int? productId)
    {
        var query = db.Stocks
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(x =>
                x.WarehouseId == warehouseId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(x =>
                x.ProductId == productId.Value);
        }

        return await query
            .OrderBy(x => x.Warehouse!.Name)
            .ThenBy(x => x.Product!.Name)
            .Select(x => new StockResponse(
                x.ProductId,
                x.Product!.SKU,
                x.Product.Name,
                x.WarehouseId,
                x.Warehouse!.Name,
                x.Product.Unit,
                x.Quantity,
                x.Product.MinStockLevel,
                x.Quantity <= x.Product.MinStockLevel,
                x.UpdatedAt
            ))
            .ToListAsync();
    }

    // =============================================
    // LAY LICH SU NHAP XUAT
    // =============================================
    public async Task<PagedResult<StockMovementResponse>>
        GetMovementsAsync(int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.StockMovements
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.CreatedByUser)
            .OrderByDescending(x => x.CreatedAt);

        var count = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new StockMovementResponse(
                x.Id,
                x.MovementType,
                x.ProductId,
                x.Product!.SKU,
                x.Product.Name,
                x.WarehouseId,
                x.Warehouse!.Name,
                x.Quantity,
                x.ReferenceNo,
                x.Notes,
                x.CreatedByUser == null
                    ? null
                    : x.CreatedByUser.Username,
                x.CreatedAt
            ))
            .ToListAsync();

        return new PagedResult<StockMovementResponse>(
            items,
            count,
            pageNumber,
            pageSize
        );
    }
}
