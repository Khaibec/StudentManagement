# Student Management

Ứng dụng quản lý sinh viên gồm frontend Angular và REST API ASP.NET Core. Ứng dụng hỗ trợ quản lý sinh viên, lớp học, môn học và đăng ký môn học.

## Công nghệ

- Frontend: Angular 22, TypeScript, RxJS
- Backend: ASP.NET Core trên .NET 10
- Database: Microsoft SQL Server
- ORM: Entity Framework Core 10
- API documentation: Swagger / OpenAPI

## Yêu cầu môi trường

- Node.js và npm
- Angular CLI 22+
- .NET SDK 10
- Microsoft SQL Server đang chạy ở máy local

Kiểm tra các phiên bản:

```bash
node --version
npm --version
dotnet --version
```

## Cấu hình database

Mặc định API sử dụng connection string trong `StudentApi/appsettings.json`:

```text
Server=localhost;Database=studentmanagement;Trusted_Connection=True;TrustServerCertificate=True;
```

Nếu SQL Server của bạn dùng server hoặc phương thức xác thực khác, hãy cập nhật `ConnectionStrings:DefaultConnection` trong `StudentApi/appsettings.Development.json` hoặc `StudentApi/appsettings.json`.

Tạo hoặc cập nhật database bằng migration:

```bash
cd StudentApi
dotnet ef database update
```

Nếu máy chưa có Entity Framework CLI:

```bash
dotnet tool install --global dotnet-ef
```

## Chạy project

Mở hai terminal tại thư mục gốc.

### 1. Chạy backend

```bash
cd StudentApi
dotnet restore
dotnet run --launch-profile http
```

API chạy tại `http://localhost:5073`.

- Swagger UI: `http://localhost:5073/swagger`
- Health check đơn giản: `http://localhost:5073/`

### 2. Chạy frontend

```bash
cd frontend
npm install
npm start
```

Mở `http://localhost:4200/` trên trình duyệt. Frontend hiện được cấu hình gọi API tại `http://localhost:5073/api`.

## API endpoints

## Xác thực API

Trước khi gọi các API quản lý, tạo tài khoản hoặc đăng nhập để nhận JWT. Mật khẩu được băm bằng `PasswordHasher`, không được lưu dạng rõ.

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| POST | `/api/auth/register` | Đăng ký tài khoản mới và trả về access token |
| POST | `/api/auth/login` | Đăng nhập và trả về access token |
| GET | `/api/auth/me` | Lấy thông tin người dùng đang đăng nhập |

Đăng ký (mật khẩu tối thiểu 8 ký tự):

```json
POST /api/auth/register
{
  "fullName": "Nguyen Van A",
  "email": "nguyenvana@example.com",
  "password": "MatKhau123!"
}
```

Sau khi đăng ký hoặc đăng nhập, response chứa `accessToken`. Gửi token đó ở header khi gọi `/api/students`, `/api/classes`, `/api/courses` và `/api/enrollments`:

```text
Authorization: Bearer <accessToken>
```

Token mặc định hết hạn sau 60 phút. Khi deploy, đặt `Jwt__Key` bằng một secret ngẫu nhiên, đủ dài trong biến môi trường thay vì dùng khóa mẫu ở `appsettings.json`.

Các nhóm tài nguyên `students`, `classes` và `courses` đều hỗ trợ:

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| GET | `/api/{resource}` | Lấy danh sách |
| GET | `/api/{resource}/{id}` | Lấy một bản ghi |
| POST | `/api/{resource}` | Tạo bản ghi |
| PUT | `/api/{resource}/{id}` | Cập nhật bản ghi |
| DELETE | `/api/{resource}/{id}` | Xóa bản ghi |

Trong đó `{resource}` là một trong `students`, `classes`, `courses`.

Enrollment sử dụng khóa ghép gồm `studentId` và `courseId`:

| Method | Endpoint | Mô tả |
| --- | --- | --- |
| GET | `/api/enrollments` | Lấy danh sách đăng ký |
| GET | `/api/enrollments/{studentId}/{courseId}` | Lấy một đăng ký |
| POST | `/api/enrollments` | Tạo đăng ký |
| PUT | `/api/enrollments/{studentId}/{courseId}` | Cập nhật điểm |
| DELETE | `/api/enrollments/{studentId}/{courseId}` | Xóa đăng ký |

Các request `POST` và `PUT` nhận dữ liệu JSON. Chi tiết schema có trong Swagger.

## Cấu trúc project

```text
StudentManagement/
├── frontend/              # Angular application
└── StudentApi/            # ASP.NET Core Web API
    ├── Controllers/       # Các API endpoint
    ├── Data/              # DbContext
    ├── DTOs/              # Request/response models
    ├── Migrations/        # EF Core migrations
    ├── Models/            # Entity models
    └── Services/          # Business logic và truy cập dữ liệu
```

## Kiểm thử và build

Frontend:

```bash
cd frontend
npm test
npm run build
```

Backend:

```bash
cd StudentApi
dotnet build
```

## Ghi chú

- Backend chỉ cho phép CORS từ `http://localhost:4200` trong cấu hình hiện tại.
- Khi đổi cổng API, cần cập nhật `apiUrl` trong `frontend/src/app/app.ts` và cấu hình CORS trong `StudentApi/Program.cs`.
- Không nên đưa thông tin xác thực database thật vào source control.
