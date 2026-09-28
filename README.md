# Student Management

Ứng dụng web quản lý học sinh, lớp học, môn học và đăng ký học phần. Dự án gồm Angular làm giao diện và ASP.NET Core Web API làm máy chủ.

## Chức năng

- Đăng ký, đăng nhập và xác thực bằng JWT.
- Dashboard thống kê số học sinh, lớp học, môn học và lượt đăng ký.
- Quản lý học sinh: tìm kiếm, lọc theo lớp/giới tính, sắp xếp và phân trang.
- Quản lý lớp học, môn học và đăng ký học phần.
- Nhập/cập nhật điểm cho từng đăng ký.
- Phân quyền: mọi người dùng đã đăng nhập có thể xem dữ liệu; chỉ `Admin` được thêm, sửa, xóa dữ liệu.
- Swagger UI hỗ trợ thử nghiệm API trong môi trường Development.

## Công nghệ

| Thành phần | Công nghệ |
| --- | --- |
| Frontend | Angular 12, TypeScript, Bootstrap 5, RxJS |
| Backend | ASP.NET Core 8 Web API, Entity Framework Core 8 |
| Cơ sở dữ liệu | SQL Server |
| Bảo mật | JWT Bearer Authentication, BCrypt |
| API docs | Swagger / OpenAPI |

## Cấu trúc dự án

```text
StudentManagement/
├── backend/
│   └── StudentManagement.Api/    # ASP.NET Core Web API
├── frontend/                     # Angular application
└── StudentManagement.slnx
```

## Yêu cầu môi trường

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js (khuyến nghị 14 hoặc 16 để tương thích Angular 12) và npm
- SQL Server hoặc SQL Server Express

## Cài đặt và chạy

### 1. Cấu hình cơ sở dữ liệu

Mặc định API kết nối SQL Server cục bộ với chuỗi kết nối sau trong `backend/StudentManagement.Api/appsettings.json`:

```json
"DefaultConnection": "Server=.;Database=StudentManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Hãy sửa `DefaultConnection` nếu SQL Server của bạn dùng máy chủ, tài khoản hoặc phương thức xác thực khác.

Khi API khởi động, ứng dụng tự tạo cơ sở dữ liệu và nạp dữ liệu mẫu nếu chưa có dữ liệu.

### 2. Khởi động backend

Mở một terminal tại thư mục gốc dự án:

```powershell
cd backend/StudentManagement.Api
dotnet restore
dotnet run
```

API chạy tại `http://localhost:5000`; Swagger UI trong môi trường Development có tại `http://localhost:5000/swagger`.

### 3. Khởi động frontend

Mở một terminal khác:

```powershell
cd frontend
npm install
npm start
```

Truy cập ứng dụng tại `http://localhost:4200`.

Frontend hiện gọi API tại `http://localhost:5000`. Nếu thay đổi cổng backend, hãy cập nhật các hằng `apiUrl` trong `frontend/src/app/services/` cho phù hợp.

## Tài khoản mẫu

| Vai trò | Tên đăng nhập | Mật khẩu |
| --- | --- | --- |
| Admin | `admin` | `Admin@123` |
| User | `user` | `User@123` |

Tài khoản `Admin` có thể tạo, cập nhật và xóa dữ liệu. Tài khoản `User` chỉ có quyền xem. Bạn cũng có thể đăng ký tài khoản mới; tài khoản mới mặc định có vai trò `User`.

## API chính

Tất cả endpoint dưới đây, trừ đăng ký/đăng nhập, đều yêu cầu JWT Bearer token.

| Nhóm | Endpoint | Mô tả |
| --- | --- | --- |
| Auth | `POST /api/auth/login`, `POST /api/auth/register`, `GET /api/auth/me` | Xác thực và hồ sơ người dùng |
| Dashboard | `GET /api/dashboard/stats` | Thống kê tổng quan |
| Students | `GET/POST /api/students`, `GET/PUT/DELETE /api/students/{id}` | Quản lý học sinh |
| Classes | `GET/POST /api/classes`, `GET/PUT/DELETE /api/classes/{id}` | Quản lý lớp học |
| Courses | `GET/POST /api/courses`, `GET/PUT/DELETE /api/courses/{id}` | Quản lý môn học |
| Enrollments | `GET/POST /api/enrollments`, `PUT /api/enrollments/{id}/grade`, `DELETE /api/enrollments/{id}` | Đăng ký học và điểm |

Các thao tác `POST`, `PUT`, `DELETE` với Students, Classes, Courses và Enrollments chỉ dành cho `Admin`.

Ví dụ đăng nhập:

```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

Sau khi nhận token, gửi header sau cho các API được bảo vệ:

```http
Authorization: Bearer <token>
```

## Dữ liệu mẫu

Lần chạy đầu tiên API sẽ tạo:

- 2 người dùng (`admin`, `user`)
- 3 lớp học
- 4 môn học
- 5 học sinh
- Các bản ghi đăng ký học phần và điểm mẫu

## Lệnh hữu ích

```powershell
# Build backend
cd backend/StudentManagement.Api
dotnet build

# Build frontend production
cd frontend
npm run build

# Chạy unit test Angular
npm test
```
