
# WAREHOUSE MANAGEMENT SYSTEM
## Hệ thống quản lý kho hàng – WarehouseManagementAPI

## 1. Giới thiệu dự án

Warehouse Management System là hệ thống quản lý kho hàng được xây dựng nhằm hỗ trợ doanh nghiệp quản lý sản phẩm, danh mục, nhà cung cấp, kho hàng, hoạt động nhập kho, xuất kho và số lượng tồn kho.

Dự án được phát triển theo mô hình Backend API kết hợp Frontend Web, sử dụng ASP.NET Core Web API và SQL Server.

Hệ thống giúp:
- Quản lý tập trung thông tin hàng hóa.
- Theo dõi số lượng sản phẩm tại từng kho.
- Quản lý quy trình nhập kho và xuất kho.
- Kiểm tra và cảnh báo sản phẩm sắp hết hàng.
- Lưu lịch sử các giao dịch kho.
- Phân quyền người dùng.
- Thống kê tình hình hoạt động của kho hàng.

## 2. Công nghệ sử dụng

### Backend
- C#.
- ASP.NET Core Web API.
- .NET 10.
- Entity Framework Core.
- LINQ.
- JWT Authentication.
- Data Annotations.
- Dependency Injection.

### Frontend
- HTML5.
- CSS3.
- JavaScript.
- Fetch API.
- Responsive Web Design.

### Database
- Microsoft SQL Server Express.
- SQL Server Management Studio (SSMS).
- Entity Framework Core.

### Công cụ phát triển
- Visual Studio 2022.
- SQL Server Express.
- SQL Server Management Studio.
- Swagger / OpenAPI.
- Git (nếu sử dụng quản lý mã nguồn).

## 3. Chức năng hệ thống

### 3.1. Quản lý người dùng và phân quyền
- Đăng nhập hệ thống.
- Xác thực bằng JWT.
- Phân quyền Admin và Staff.
- Kiểm soát quyền truy cập các chức năng.

### 3.2. Quản lý danh mục sản phẩm
- Thêm danh mục.
- Xem danh sách danh mục.
- Cập nhật danh mục.
- Xóa danh mục.

### 3.3. Quản lý sản phẩm
- Thêm sản phẩm mới.
- Xem danh sách sản phẩm.
- Cập nhật thông tin sản phẩm.
- Xóa sản phẩm.
- Quản lý mã SKU.
- Quản lý giá nhập và giá bán.
- Thiết lập số lượng tồn kho tối thiểu.
- Tìm kiếm và lọc sản phẩm.

### 3.4. Quản lý nhà cung cấp
- Thêm nhà cung cấp.
- Xem danh sách nhà cung cấp.
- Cập nhật thông tin nhà cung cấp.
- Xóa nhà cung cấp.
- Quản lý thông tin liên hệ.

### 3.5. Quản lý kho hàng
- Thêm kho hàng.
- Xem danh sách kho.
- Cập nhật thông tin kho.
- Xóa kho.
- Quản lý mã kho và địa chỉ kho.

### 3.6. Quản lý nhập kho
- Tạo phiếu nhập kho.
- Chọn kho nhập.
- Chọn sản phẩm cần nhập.
- Nhập số lượng sản phẩm.
- Tự động cập nhật số lượng tồn kho.
- Lưu lịch sử nhập kho.

### 3.7. Quản lý xuất kho
- Tạo phiếu xuất kho.
- Chọn kho xuất.
- Chỉ hiển thị sản phẩm có tồn tại kho được chọn.
- Kiểm tra số lượng tồn trước khi xuất.
- Không cho phép xuất vượt quá số lượng tồn.
- Tự động giảm số lượng tồn kho.
- Lưu lịch sử xuất kho.

### 3.8. Quản lý tồn kho
- Theo dõi số lượng sản phẩm tại từng kho.
- Xem tồn kho theo sản phẩm.
- Xem tồn kho theo kho.
- Cảnh báo sản phẩm sắp hết hàng.
- Phân biệt trạng thái còn hàng, sắp hết hàng và hết hàng.

### 3.9. Lịch sử giao dịch
- Xem lịch sử nhập kho.
- Xem lịch sử xuất kho.
- Theo dõi sản phẩm được giao dịch.
- Theo dõi kho thực hiện giao dịch.
- Theo dõi số lượng và người thực hiện.

### 3.10. Dashboard
- Tổng số sản phẩm.
- Tổng số kho hàng.
- Tổng số nhà cung cấp.
- Thống kê tình trạng tồn kho.
- Cảnh báo hàng sắp hết.
- Tổng hợp thông tin hoạt động kho.

## 4. Kiến trúc hệ thống

Dự án sử dụng mô hình phân chia trách nhiệm giữa Frontend, Backend và Database.

### Frontend
Tiếp nhận thao tác từ người dùng, hiển thị dữ liệu và gửi HTTP Request đến Backend thông qua Fetch API.

### Backend
ASP.NET Core Web API tiếp nhận Request, kiểm tra dữ liệu, xác thực JWT, xử lý nghiệp vụ và truy vấn cơ sở dữ liệu.

### Database
SQL Server lưu trữ dữ liệu người dùng, sản phẩm, danh mục, nhà cung cấp, kho hàng, tồn kho và lịch sử giao dịch.

Luồng xử lý:

Frontend
    ↓
ASP.NET Core Web API
    ↓
Controllers
    ↓
Services / AppDbContext
    ↓
Entity Framework Core
    ↓
SQL Server
    ↓
Trả kết quả về Frontend

## 5. Cấu trúc thư mục dự án

WarehouseManagementAPI
│
├── Controllers
│   ├── AuthController.cs
│   ├── CategoriesController.cs
│   ├── ProductsController.cs
│   ├── SuppliersController.cs
│   ├── WarehousesController.cs
│   ├── InventoryController.cs
│   └── DashboardController.cs
│
├── Data
│   ├── AppDbContext.cs
│   └── DatabaseSeeder.cs
│
├── DTOs
│   ├── AuthDtos.cs
│   ├── ProductDtos.cs
│   ├── InventoryDtos.cs
│   └── ...
│
├── Models
│   ├── User.cs
│   ├── Product.cs
│   ├── Category.cs
│   ├── Supplier.cs
│   ├── Warehouse.cs
│   ├── Stock.cs
│   └── StockMovement.cs
│
├── Services
│   ├── InventoryService.cs
│   └── TokenService.cs
│
├── wwwroot
│   ├── index.html
│   ├── style.css
│   └── app.js
│
├── appsettings.json
├── Program.cs
└── WarehouseManagementAPI.csproj

Lưu ý: Đây là cấu trúc tổng quan. Tên file thực tế có thể thay đổi theo phiên bản source code đang sử dụng.

## 6. Thiết kế cơ sở dữ liệu

Database sử dụng Microsoft SQL Server.

Tên database:

WarehouseManagementDB

SQL Server:

localhost\SQLEXPRESS

Các bảng dữ liệu chính:

| Bảng | Chức năng |
|---|---|
| Users | Lưu tài khoản và vai trò |
| Categories | Lưu danh mục sản phẩm |
| Suppliers | Lưu nhà cung cấp |
| Warehouses | Lưu thông tin kho hàng |
| Products | Lưu thông tin sản phẩm |
| Stocks | Lưu số lượng tồn theo kho và sản phẩm |
| StockMovements | Lưu lịch sử nhập và xuất kho |

### Quan hệ dữ liệu

- Một Category có thể có nhiều Product.
- Một Supplier có thể cung cấp nhiều Product.
- Một Product có thể có số lượng tồn tại nhiều Warehouse.
- Một Warehouse có thể chứa nhiều Product.
- Một Product có nhiều giao dịch nhập và xuất.
- Một Warehouse có nhiều giao dịch nhập và xuất.
- Một User có thể thực hiện nhiều giao dịch kho.

Bảng Stocks kết hợp ProductId và WarehouseId để theo dõi số lượng sản phẩm tại từng kho.

Bảng StockMovements lưu lại thông tin các giao dịch nhập kho và xuất kho.

## 7. Yêu cầu môi trường

Trước khi chạy dự án cần cài đặt:

1. Visual Studio 2022.
2. .NET 10 SDK.
3. SQL Server Express.
4. SQL Server Management Studio (SSMS).
5. Trình duyệt Chrome hoặc Microsoft Edge.

## 8. Cấu hình kết nối SQL Server

Mở file appsettings.json.

Cấu hình ConnectionStrings:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=WarehouseManagementDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
  }
}

Lưu ý:
- Server: localhost\SQLEXPRESS.
- Database: WarehouseManagementDB.
- Authentication: Windows Authentication.
- Tài khoản Windows cần có quyền kết nối SQL Server Express.

Tên Connection String trong appsettings.json phải trùng với tên được sử dụng trong Program.cs.

## 9. Hướng dẫn cài đặt và chạy dự án

### Bước 1: Giải nén source code

Giải nén thư mục WarehouseManagementAPI vào máy tính.

Ví dụ:

D:\LTGWeb2\WarehouseManagementAPI

### Bước 2: Mở dự án

Mở file:

WarehouseManagementAPI.sln

bằng Visual Studio.

### Bước 3: Khôi phục thư viện NuGet

Mở Terminal trong Visual Studio và chạy:

dotnet restore

### Bước 4: Kiểm tra cấu hình database

Kiểm tra appsettings.json và đảm bảo:

Server=localhost\SQLEXPRESS
Database=WarehouseManagementDB

### Bước 5: Build dự án

Chọn:

Build → Build Solution

Hoặc chạy lệnh:

dotnet build

### Bước 6: Chạy dự án

Nhấn F5 trong Visual Studio.

Hoặc dùng Terminal:

dotnet run

### Bước 7: Mở giao diện Frontend

Truy cập:

https://localhost:7260/

### Bước 8: Mở Swagger

Truy cập:

https://localhost:7260/swagger

Lưu ý: Nếu cổng HTTPS được thay đổi trong launchSettings.json, sử dụng đúng cổng được hiển thị khi chạy ứng dụng.

## 10. Khởi tạo database và dữ liệu mẫu

Dự án sử dụng Entity Framework Core để kết nối SQL Server.

Khi khởi động lần đầu, chương trình thực hiện khởi tạo database và dữ liệu mẫu thông qua phần Data/DatabaseSeeder.cs nếu được cấu hình trong Program.cs.

Database cần sử dụng:

WarehouseManagementDB

Nếu database đã tồn tại, kiểm tra dữ liệu hiện có trước khi chạy lại các script bổ sung.

Có thể sử dụng SSMS để thêm hoặc kiểm tra dữ liệu mẫu.

Ví dụ:

USE WarehouseManagementDB;
GO

SELECT * FROM Categories;
SELECT * FROM Suppliers;
SELECT * FROM Warehouses;
SELECT * FROM Products;
SELECT * FROM Stocks;
SELECT * FROM StockMovements;
SELECT * FROM Users;

Không xóa database đang sử dụng nếu chưa sao lưu dữ liệu.

## 11. Tài khoản đăng nhập demo

Tài khoản được cấu hình trong dữ liệu khởi tạo:

| Vai trò | Username | Password |
|---|---|---|
| Admin | admin | Admin@12345 |
| Staff | staff01 | Staff@12345 |
| Staff | staff02 | Staff@12345 |

Lưu ý: Tài khoản chỉ sử dụng được nếu đã được tạo thành công trong database. Kiểm tra bảng Users nếu không đăng nhập được.

## 12. Các API chính

### Authentication

| Method | Endpoint | Chức năng |
|---|---|---|
| POST | /api/auth/login | Đăng nhập |
| POST | /api/auth/register | Đăng ký nếu được triển khai |

### Categories

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/categories | Lấy danh sách |
| GET | /api/categories/{id} | Chi tiết |
| POST | /api/categories | Thêm |
| PUT | /api/categories/{id} | Cập nhật |
| DELETE | /api/categories/{id} | Xóa |

### Products

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/products | Danh sách |
| GET | /api/products/{id} | Chi tiết |
| POST | /api/products | Thêm sản phẩm |
| PUT | /api/products/{id} | Cập nhật |
| DELETE | /api/products/{id} | Xóa |

### Suppliers

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/suppliers | Danh sách |
| POST | /api/suppliers | Thêm |
| PUT | /api/suppliers/{id} | Cập nhật |
| DELETE | /api/suppliers/{id} | Xóa |

### Warehouses

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/warehouses | Danh sách |
| POST | /api/warehouses | Thêm |
| PUT | /api/warehouses/{id} | Cập nhật |
| DELETE | /api/warehouses/{id} | Xóa |

### Inventory

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/inventory | Xem tồn kho |
| POST | /api/inventory/receipts | Tạo phiếu nhập |
| POST | /api/inventory/issues | Tạo phiếu xuất |
| GET | /api/inventory/movements | Lịch sử giao dịch |

### Dashboard

| Method | Endpoint | Chức năng |
|---|---|---|
| GET | /api/dashboard/summary | Thống kê tổng quan |

Lưu ý: Swagger là nguồn kiểm tra endpoint, HTTP method và request body chính xác của phiên bản đang chạy. Các API phụ thuộc vào cách triển khai trong source code.

## 13. Quy trình nghiệp vụ nhập kho

1. Người dùng đăng nhập hệ thống.
2. Chọn chức năng Nhập kho.
3. Chọn kho nhận hàng.
4. Chọn sản phẩm cần nhập.
5. Nhập số lượng.
6. Gửi yêu cầu nhập kho đến Backend.
7. Backend kiểm tra sản phẩm và kho.
8. Backend cập nhật số lượng tồn.
9. Lưu lịch sử nhập kho.
10. Trả kết quả về Frontend.

Khi nhập thành công, số lượng tồn kho tăng tương ứng với số lượng nhập.

## 14. Quy trình nghiệp vụ xuất kho

1. Người dùng đăng nhập hệ thống.
2. Chọn chức năng Xuất kho.
3. Chọn kho xuất hàng.
4. Hệ thống lọc sản phẩm theo kho đã chọn.
5. Chỉ hiển thị sản phẩm có số lượng tồn tại kho đó.
6. Người dùng chọn sản phẩm và nhập số lượng xuất.
7. Backend kiểm tra số lượng tồn.
8. Nếu đủ hàng, hệ thống giảm số lượng tồn.
9. Lưu lịch sử xuất kho.
10. Trả kết quả về Frontend.

Nếu số lượng xuất lớn hơn tồn kho hiện tại, hệ thống từ chối giao dịch.

## 15. Quy tắc tính tồn kho

Tồn kho sau giao dịch được tính theo công thức:

Tồn kho cuối = Tồn kho đầu + Số lượng nhập - Số lượng xuất

Ví dụ:

- Tồn kho ban đầu: 50 sản phẩm.
- Nhập thêm: 20 sản phẩm.
- Xuất: 15 sản phẩm.

Kết quả:

50 + 20 - 15 = 55 sản phẩm.

Mọi thay đổi tồn kho cần được ghi nhận vào lịch sử giao dịch.

## 16. Kiểm thử hệ thống

Các chức năng cần kiểm thử trước khi demo:

| STT | Chức năng | Kết quả mong đợi |
|---|---|---|
| 1 | Đăng nhập Admin | Đăng nhập thành công |
| 2 | Đăng nhập sai mật khẩu | Từ chối đăng nhập |
| 3 | Thêm sản phẩm | Lưu thành công |
| 4 | Thêm danh mục | Lưu thành công |
| 5 | Thêm nhà cung cấp | Lưu thành công |
| 6 | Thêm kho hàng | Lưu thành công |
| 7 | Nhập kho | Tồn kho tăng |
| 8 | Xuất kho | Tồn kho giảm |
| 9 | Xuất vượt tồn | Từ chối giao dịch |
| 10 | Kiểm tra lịch sử | Hiển thị giao dịch |
| 11 | Cảnh báo tồn kho | Hiển thị hàng cần chú ý |
| 12 | Phân quyền | Người dùng không đủ quyền bị từ chối |

## 17. Các lỗi thường gặp

### Lỗi kết nối SQL Server

Kiểm tra:
- SQL Server Express đang chạy.
- Server name đúng: localhost\SQLEXPRESS.
- Connection String đúng.
- Tài khoản Windows có quyền kết nối.

### Lỗi database chưa tồn tại

Kiểm tra cấu hình khởi tạo database trong Program.cs và DatabaseSeeder.cs.

Nếu sử dụng EnsureCreatedAsync, lưu ý phương thức này không tự cập nhật schema của database đã tồn tại khi model thay đổi.

### Lỗi 401 Unauthorized

Nguyên nhân:
- Chưa đăng nhập.
- Chưa gửi JWT.
- Token không hợp lệ hoặc hết hạn.

### Lỗi 403 Forbidden

Người dùng đã xác thực nhưng không đủ quyền thực hiện thao tác.

### Lỗi 500 Internal Server Error

Kiểm tra:
- Visual Studio Output.
- Exception Message.
- Inner Exception.
- Request Payload trong trình duyệt.
- Lỗi SQL Server nếu có.

## 18. Hướng dẫn demo trước giảng viên

Đề xuất trình tự trình diễn:

1. Giới thiệu giao diện hệ thống.
2. Đăng nhập bằng tài khoản Admin.
3. Trình bày Dashboard.
4. Xem danh sách danh mục, sản phẩm, nhà cung cấp và kho.
5. Chọn một sản phẩm và thực hiện nhập kho.
6. Mở SSMS để kiểm tra số lượng tồn.
7. Thực hiện xuất kho.
8. Kiểm tra số lượng tồn sau xuất.
9. Xem lịch sử giao dịch.
10. Trình bày chức năng cảnh báo hàng sắp hết.
11. Giải thích kiến trúc Backend và database.

Ví dụ demo:

Tồn kho ban đầu: 50
Nhập kho: 20
Xuất kho: 15
Tồn kho cuối: 55

## 19. Hạn chế và hướng phát triển

Các hướng phát triển có thể xem xét:
- Quản lý đơn đặt hàng.
- Quản lý khách hàng.
- In phiếu nhập kho và xuất kho.
- Xuất báo cáo Excel và PDF.
- Quét mã vạch hoặc QR Code.
- Quản lý nhiều chi nhánh.
- Gửi cảnh báo tồn kho qua email.
- Quản lý và phân quyền người dùng chi tiết hơn.
- Triển khai hệ thống lên máy chủ để truy cập từ xa.

Các chức năng trên chỉ được xem là hướng phát triển nếu chưa được triển khai và kiểm thử trong phiên bản hiện tại.

## 20. Kết luận

Warehouse Management System được xây dựng nhằm ứng dụng kiến thức về lập trình Backend, phát triển API, thiết kế cơ sở dữ liệu và xử lý nghiệp vụ quản lý kho hàng.

Thông qua việc kết hợp ASP.NET Core Web API, Entity Framework Core, SQL Server và Frontend Web, hệ thống hỗ trợ quản lý hàng hóa, nhà cung cấp, kho hàng, nhập kho, xuất kho và theo dõi tồn kho.

Dự án giúp nhóm thực hành quy trình phân tích, thiết kế, xây dựng, kiểm thử và triển khai một hệ thống Backend có tính ứng dụng thực tế.

---

**Tên đề tài:** Thiết kế Backend cho hệ thống quản lý kho hàng.

**Tên dự án:** WarehouseManagementAPI.

**Công nghệ chính:** ASP.NET Core Web API, C#, Entity Framework Core, SQL Server, HTML, CSS và JavaScript.
