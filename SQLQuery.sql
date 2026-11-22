
USE [WarehouseManagementDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- =============================================
-- KIEM TRA CAC BANG
-- =============================================
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Suppliers', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Warehouses', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Products', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Stocks', N'U') IS NULL
   OR OBJECT_ID(N'dbo.StockMovements', N'U') IS NULL
   OR OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    RAISERROR(N'Khong tim thay du cac bang. Hay chay project truoc.', 16, 1);
    RETURN;
END;

BEGIN TRY
    BEGIN TRANSACTION;

    -- =============================================
    -- 1. THEM 16 DANH MUC
    -- =============================================

    DECLARE @CategorySeeds TABLE
    (
        Name NVARCHAR(100),
        Description NVARCHAR(500)
    );

    INSERT INTO @CategorySeeds VALUES
    (N'Thiết bị điện tử', N'Máy tính, màn hình và thiết bị điện tử.'),
    (N'Văn phòng phẩm', N'Giấy, bút và dụng cụ văn phòng.'),
    (N'Phụ kiện', N'Phụ kiện thiết bị công nghệ.'),
    (N'Linh kiện máy tính', N'RAM, SSD và linh kiện.'),
    (N'Thiết bị mạng', N'Router, switch và thiết bị mạng.'),
    (N'Thiết bị ngoại vi', N'Chuột, bàn phím, webcam và máy in.'),
    (N'Điện thoại và phụ kiện', N'Điện thoại và phụ kiện di động.'),
    (N'Thiết bị gia dụng', N'Thiết bị điện gia dụng.'),
    (N'Dụng cụ kho vận', N'Thiết bị vận chuyển và kiểm kê.'),
    (N'Thiết bị bảo hộ', N'Trang bị bảo hộ lao động.'),
    (N'Vật tư đóng gói', N'Thùng carton, màng bọc.'),
    (N'Thiết bị an ninh', N'Camera và thiết bị giám sát.'),
    (N'Thiết bị lưu trữ dữ liệu', N'Ổ cứng, USB và thiết bị lưu trữ.'),
    (N'Thiết bị chiếu sáng', N'Đèn LED và phụ kiện.'),
    (N'Vật tư vệ sinh', N'Vật tư vệ sinh kho.'),
    (N'Thiết bị văn phòng chuyên dụng', N'Máy photocopy và máy hủy giấy.');

    INSERT INTO dbo.Categories (Name, Description, CreatedAt)
    SELECT s.Name, s.Description, SYSUTCDATETIME()
    FROM @CategorySeeds s
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Categories c
        WHERE c.Name = s.Name
    );


    -- =============================================
    -- 2. THEM 20 NHA CUNG CAP
    -- =============================================

    DECLARE @SupplierSeeds TABLE
    (
        Seq INT PRIMARY KEY,
        Name NVARCHAR(150),
        ContactName NVARCHAR(100),
        Phone NVARCHAR(50),
        Email NVARCHAR(120),
        Address NVARCHAR(300)
    );

    INSERT INTO @SupplierSeeds VALUES
    (1, N'Công ty Công nghệ Sao Việt', N'Trần Minh Quân', N'0905001001', N'saoviet@example.com', N'Hải Châu, Đà Nẵng'),
    (2, N'Phân phối An Phát', N'Lê Hoàng Nam', N'0905001002', N'anphat@example.com', N'Thanh Khê, Đà Nẵng'),
    (3, N'Thiết bị Minh Long', N'Phạm Thu Hà', N'0905001003', N'minhlong@example.com', N'Cẩm Lệ, Đà Nẵng'),
    (4, N'Văn phòng phẩm Hòa Bình', N'Nguyễn Thị Mai', N'0905001004', N'hoabinh@example.com', N'Hải Châu, Đà Nẵng'),
    (5, N'Công nghệ Đại Phát', N'Đỗ Quốc Bảo', N'0905001005', N'daiphat@example.com', N'Liên Chiểu, Đà Nẵng'),
    (6, N'Điện tử Nam Việt', N'Võ Thành Đạt', N'0905001006', N'namviet@example.com', N'Sơn Trà, Đà Nẵng'),
    (7, N'Linh kiện Thành Công', N'Bùi Ngọc Anh', N'0905001007', N'thanhcong@example.com', N'Đà Nẵng'),
    (8, N'Mạng và Camera ABC', N'Hoàng Gia Huy', N'0905001008', N'abcnetwork@example.com', N'Huế'),
    (9, N'Đóng gói Tiến Đạt', N'Phan Văn Đức', N'0905001009', N'tiendat@example.com', N'Quảng Nam'),
    (10, N'Thiết bị công nghiệp Vạn Lợi', N'Lý Thanh Tùng', N'0905001010', N'vanloi@example.com', N'Hà Nội'),
    (11, N'Điện máy Đông Á', N'Ngô Thảo Vy', N'0905001011', N'donga@example.com', N'TP. Hồ Chí Minh'),
    (12, N'Kho vận Miền Trung', N'Đặng Minh Khoa', N'0905001012', N'khovan@example.com', N'Đà Nẵng'),
    (13, N'Công nghệ Phương Nam', N'Nguyễn Hữu Phúc', N'0905001013', N'phuongnam@example.com', N'Đà Nẵng'),
    (14, N'Thiết bị số Hoàng Gia', N'Trần Bảo Ngọc', N'0905001014', N'hoanggia@example.com', N'Hà Nội'),
    (15, N'Điện máy Thành Đạt', N'Phạm Minh Đức', N'0905001015', N'thanhdat@example.com', N'Quảng Nam'),
    (16, N'Văn phòng phẩm Trường Sơn', N'Lê Thị Hương', N'0905001016', N'truongson@example.com', N'Huế'),
    (17, N'Kho vận Đại Thành', N'Vũ Quốc Anh', N'0905001017', N'daithanh@example.com', N'Bình Dương'),
    (18, N'An ninh Sao Bắc Đẩu', N'Đỗ Hoàng Long', N'0905001018', N'saobacdau@example.com', N'TP. Hồ Chí Minh'),
    (19, N'Bao bì Việt Tiến', N'Nguyễn Thành Trung', N'0905001019', N'viettien@example.com', N'Đà Nẵng'),
    (20, N'Vật tư Công nghiệp Hưng Thịnh', N'Bùi Văn Sơn', N'0905001020', N'hungthinh@example.com', N'Cần Thơ');

    INSERT INTO dbo.Suppliers
    (Name, ContactName, Phone, Email, Address, CreatedAt)
    SELECT
        s.Name, s.ContactName, s.Phone,
        s.Email, s.Address, SYSUTCDATETIME()
    FROM @SupplierSeeds s
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Suppliers existing
        WHERE existing.Name = s.Name
    );


    -- =============================================
    -- 3. THEM 8 KHO HANG
    -- =============================================

    DECLARE @WarehouseSeeds TABLE
    (
        Code NVARCHAR(30),
        Name NVARCHAR(150),
        Address NVARCHAR(300)
    );

    INSERT INTO @WarehouseSeeds VALUES
    (N'WH-HQ', N'Kho trung tâm', N'Hải Châu, Đà Nẵng'),
    (N'WH-HN', N'Kho miền Bắc', N'Long Biên, Hà Nội'),
    (N'WH-HUE', N'Kho miền Trung', N'Phú Bài, Huế'),
    (N'WH-HCM', N'Kho miền Nam', N'Thủ Đức, TP. Hồ Chí Minh'),
    (N'WH-QN', N'Kho Quảng Nam', N'Điện Bàn, Quảng Nam'),
    (N'WH-DN2', N'Kho Đà Nẵng - Liên Chiểu', N'Hòa Khánh, Đà Nẵng'),
    (N'WH-CT', N'Kho miền Tây', N'Ninh Kiều, Cần Thơ'),
    (N'WH-BD', N'Kho Bình Dương', N'Thuận An, Bình Dương');

    INSERT INTO dbo.Warehouses
    (Code, Name, Address, CreatedAt)
    SELECT
        s.Code, s.Name, s.Address, SYSUTCDATETIME()
    FROM @WarehouseSeeds s
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Warehouses w
        WHERE w.Code = s.Code
    );


    -- =============================================
    -- 4. TAO 20 MAU SAN PHAM
    -- =============================================

    DECLARE @ProductTemplates TABLE
    (
        TemplateNo INT PRIMARY KEY,
        Name NVARCHAR(200),
        Unit NVARCHAR(30),
        CategoryName NVARCHAR(100),
        BaseCost DECIMAL(18,2)
    );

    INSERT INTO @ProductTemplates VALUES
    (1, N'Laptop văn phòng', N'cái', N'Thiết bị điện tử', 12000000),
    (2, N'Màn hình LED 24 inch', N'cái', N'Thiết bị điện tử', 2100000),
    (3, N'Giấy in A4 70gsm', N'ram', N'Văn phòng phẩm', 65000),
    (4, N'Bút bi xanh', N'hộp', N'Văn phòng phẩm', 42000),
    (5, N'Hub USB-C', N'cái', N'Phụ kiện', 320000),
    (6, N'Cáp HDMI 2 mét', N'sợi', N'Phụ kiện', 45000),
    (7, N'SSD 512GB', N'cái', N'Linh kiện máy tính', 850000),
    (8, N'RAM DDR4 16GB', N'thanh', N'Linh kiện máy tính', 690000),
    (9, N'Router Wi-Fi 6', N'cái', N'Thiết bị mạng', 920000),
    (10, N'Switch mạng 8 cổng', N'cái', N'Thiết bị mạng', 480000),
    (11, N'Chuột không dây', N'cái', N'Thiết bị ngoại vi', 180000),
    (12, N'Bàn phím cơ', N'cái', N'Thiết bị ngoại vi', 420000),
    (13, N'Điện thoại Android 128GB', N'cái', N'Điện thoại và phụ kiện', 3900000),
    (14, N'Củ sạc nhanh 30W', N'cái', N'Điện thoại và phụ kiện', 145000),
    (15, N'Ấm siêu tốc', N'cái', N'Thiết bị gia dụng', 185000),
    (16, N'Xe nâng tay 2 tấn', N'cái', N'Dụng cụ kho vận', 4200000),
    (17, N'Găng tay bảo hộ', N'đôi', N'Thiết bị bảo hộ', 12000),
    (18, N'Thùng carton', N'cái', N'Vật tư đóng gói', 8500),
    (19, N'Camera IP', N'cái', N'Thiết bị an ninh', 530000),
    (20, N'Đèn LED nhà xưởng', N'cái', N'Thiết bị chiếu sáng', 320000);


    -- =============================================
    -- 5. TAO 120 SO THU TU
    -- Khong dung CTE de tranh loi Numbers
    -- =============================================

    DECLARE @Numbers TABLE (N INT PRIMARY KEY);
    DECLARE @i INT = 1;

    WHILE @i <= 120
    BEGIN
        INSERT INTO @Numbers (N) VALUES (@i);
        SET @i = @i + 1;
    END;


    -- =============================================
    -- 6. THEM 120 SAN PHAM
    -- =============================================

    INSERT INTO dbo.Products
    (
        SKU,
        Name,
        Unit,
        CostPrice,
        SellingPrice,
        MinStockLevel,
        IsActive,
        CreatedAt,
        CategoryId,
        SupplierId
    )
    SELECT
        CONCAT(N'SQL-DEMO-', RIGHT(CONCAT(N'0000', n.N), 4)),
        CONCAT(t.Name, N' - mẫu ', ((n.N - 1) / 20) + 1),
        t.Unit,
        CAST(t.BaseCost + (n.N % 5) * 10000 AS DECIMAL(18,2)),
        CAST((t.BaseCost + (n.N % 5) * 10000) * 1.22 AS DECIMAL(18,2)),
        CAST(5 + (n.N % 16) AS DECIMAL(18,3)),
        1,
        DATEADD(DAY, -(n.N % 60), SYSUTCDATETIME()),
        c.Id,
        supplier.Id
    FROM @Numbers n
    INNER JOIN @ProductTemplates t
        ON t.TemplateNo = ((n.N - 1) % 20) + 1
    INNER JOIN dbo.Categories c
        ON c.Name = t.CategoryName
    INNER JOIN @SupplierSeeds ss
        ON ss.Seq = ((n.N - 1) % 20) + 1
    CROSS APPLY
    (
        SELECT TOP (1) su.Id
        FROM dbo.Suppliers su
        WHERE su.Name = ss.Name
        ORDER BY su.Id
    ) supplier
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Products p
        WHERE p.SKU = CONCAT(N'SQL-DEMO-', RIGHT(CONCAT(N'0000', n.N), 4))
    );


    -- =============================================
    -- 7. TAO TON KHO CHO 120 SAN PHAM
    -- Tối đa 120 x 8 = 960 dòng
    -- =============================================

    INSERT INTO dbo.Stocks
    (ProductId, WarehouseId, Quantity, UpdatedAt)
    SELECT
        p.Id,
        w.Id,
        CASE
            WHEN (p.Id + w.Id * 3) % 11 = 0
                THEN
                    CASE
                        WHEN p.MinStockLevel > 1
                        THEN p.MinStockLevel - 1
                        ELSE 1
                    END
            ELSE 25 + ((p.Id * 7 + w.Id * 13) % 176)
        END,
        SYSUTCDATETIME()
    FROM dbo.Products p
    CROSS JOIN dbo.Warehouses w
    WHERE p.SKU LIKE N'SQL-DEMO-%'
      AND w.Code IN
      (
          N'WH-HQ', N'WH-HN', N'WH-HUE', N'WH-HCM',
          N'WH-QN', N'WH-DN2', N'WH-CT', N'WH-BD'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.Stocks st
          WHERE st.ProductId = p.Id
            AND st.WarehouseId = w.Id
      );


    -- =============================================
    -- 8. TAO GIAO DICH NHAP DAU KY
    -- =============================================

    DECLARE @AdminId INT;

    SELECT TOP (1) @AdminId = Id
    FROM dbo.Users
    WHERE Role = N'Admin'
    ORDER BY Id;

    INSERT INTO dbo.StockMovements
    (
        MovementType,
        ProductId,
        WarehouseId,
        Quantity,
        ReferenceNo,
        Notes,
        CreatedByUserId,
        CreatedAt
    )
    SELECT
        N'IMPORT',
        st.ProductId,
        st.WarehouseId,
        st.Quantity,
        CONCAT(N'SQL-OPEN-', p.SKU, N'-', w.Code),
        N'Số dư đầu kỳ dữ liệu mẫu.',
        @AdminId,
        DATEADD(DAY, -10, SYSUTCDATETIME())
    FROM dbo.Stocks st
    INNER JOIN dbo.Products p
        ON p.Id = st.ProductId
    INNER JOIN dbo.Warehouses w
        ON w.Id = st.WarehouseId
    WHERE p.SKU LIKE N'SQL-DEMO-%'
      AND w.Code IN
      (
          N'WH-HQ', N'WH-HN', N'WH-HUE', N'WH-HCM',
          N'WH-QN', N'WH-DN2', N'WH-CT', N'WH-BD'
      )
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.StockMovements m
          WHERE m.ReferenceNo =
              CONCAT(N'SQL-OPEN-', p.SKU, N'-', w.Code)
      );


    -- =============================================
    -- 9. TAO 24 GIAO DICH XUAT MAU
    -- Dong thoi tru so luong ton
    -- =============================================

    DECLARE @ExportRows TABLE
    (
        ProductId INT,
        WarehouseId INT,
        Quantity DECIMAL(18,3),
        ReferenceNo NVARCHAR(200) PRIMARY KEY,
        Notes NVARCHAR(500)
    );

    INSERT INTO @ExportRows
    (
        ProductId,
        WarehouseId,
        Quantity,
        ReferenceNo,
        Notes
    )
    SELECT TOP (24)
        st.ProductId,
        st.WarehouseId,
        CAST(
            1 + (ROW_NUMBER() OVER (ORDER BY p.SKU, w.Code) % 3)
            AS DECIMAL(18,3)
        ),
        CONCAT(N'SQL-EXPORT-', p.SKU, N'-', w.Code),
        N'Giao dịch xuất mẫu tạo bằng SSMS.'
    FROM dbo.Stocks st
    INNER JOIN dbo.Products p
        ON p.Id = st.ProductId
    INNER JOIN dbo.Warehouses w
        ON w.Id = st.WarehouseId
    WHERE p.SKU LIKE N'SQL-DEMO-%'
      AND st.Quantity > 3
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.StockMovements m
          WHERE m.ReferenceNo =
              CONCAT(N'SQL-EXPORT-', p.SKU, N'-', w.Code)
      )
    ORDER BY p.SKU, w.Code;


    -- Tru ton kho
    UPDATE st
    SET
        st.Quantity = st.Quantity - ex.Quantity,
        st.UpdatedAt = SYSUTCDATETIME()
    FROM dbo.Stocks st
    INNER JOIN @ExportRows ex
        ON ex.ProductId = st.ProductId
       AND ex.WarehouseId = st.WarehouseId
    WHERE st.Quantity >= ex.Quantity;


    -- Luu lich su xuat kho
    INSERT INTO dbo.StockMovements
    (
        MovementType,
        ProductId,
        WarehouseId,
        Quantity,
        ReferenceNo,
        Notes,
        CreatedByUserId,
        CreatedAt
    )
    SELECT
        N'EXPORT',
        ex.ProductId,
        ex.WarehouseId,
        ex.Quantity,
        ex.ReferenceNo,
        ex.Notes,
        @AdminId,
        SYSUTCDATETIME()
    FROM @ExportRows ex;

    COMMIT TRANSACTION;

    PRINT N'DA BO SUNG DU LIEU MAU THANH CONG!';

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();

    RAISERROR(N'Loi SQL: %s', 16, 1, @ErrorMessage);
    RETURN;

END CATCH;


-- =============================================
-- 10. KIEM TRA SO LUONG DU LIEU
-- =============================================

SELECT N'Categories' AS TableName, COUNT(*) AS Total
FROM dbo.Categories

UNION ALL

SELECT N'Suppliers', COUNT(*)
FROM dbo.Suppliers

UNION ALL

SELECT N'Warehouses', COUNT(*)
FROM dbo.Warehouses

UNION ALL

SELECT N'Products', COUNT(*)
FROM dbo.Products

UNION ALL

SELECT N'Stocks', COUNT(*)
FROM dbo.Stocks

UNION ALL

SELECT N'StockMovements', COUNT(*)
FROM dbo.StockMovements

UNION ALL

SELECT N'Users', COUNT(*)
FROM dbo.Users;


-- =============================================
-- 11. KIEM TRA 30 SAN PHAM VA TON KHO
-- =============================================

SELECT TOP (30)
    p.SKU,
    p.Name AS ProductName,
    w.Code AS WarehouseCode,
    w.Name AS WarehouseName,
    st.Quantity,
    p.MinStockLevel,
    CASE
        WHEN st.Quantity <= p.MinStockLevel
            THEN N'Sắp hết hàng'
        ELSE N'Còn hàng'
    END AS StockStatus
FROM dbo.Stocks st
INNER JOIN dbo.Products p
    ON p.Id = st.ProductId
INNER JOIN dbo.Warehouses w
    ON w.Id = st.WarehouseId
WHERE p.SKU LIKE N'SQL-DEMO-%'
ORDER BY p.SKU, w.Code;

GO
