# Student Management System

Ứng dụng web quản lý học sinh, lớp học, môn học và điểm số theo mô hình Client-Server (SPA + RESTful API).

---

## 🚀 Công nghệ sử dụng

- **Frontend:** Angular 12, Bootstrap 5, RxJS
- **Backend:** ASP.NET Core 8 Web API, Entity Framework Core 8
- **Database:** SQL Server (tự động migration & nạp dữ liệu mẫu khi chạy)
- **Bảo mật:** JWT Bearer Token, BCrypt
- **API Docs:** Swagger UI

---

## ✨ Tính năng chính

- **Xác thực & Phân quyền:** Đăng ký, đăng nhập JWT. Phân quyền: `Admin` (toàn quyền thêm/sửa/xóa) và `User` (chỉ xem).
- **Dashboard:** Thống kê nhanh tổng số học sinh, lớp học, môn học và lượt đăng ký.
- **Quản lý học sinh:** Tìm kiếm, lọc (lớp, giới tính), sắp xếp và phân trang linh hoạt.
- **Quản lý học vụ:** Danh mục lớp học, môn học, đăng ký tín chỉ và cập nhật điểm số.

---

## 🛠️ Cài đặt & Khởi chạy

### Yêu cầu môi trường
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (khuyến nghị v14 hoặc v16) & npm
- SQL Server (Local / Express)

### 1. Khởi động Backend
> Cấu hình chuỗi kết nối tại `backend/StudentManagement.Api/appsettings.json` (mặc định trỏ đến SQL Server cục bộ).

```powershell
cd backend/StudentManagement.Api
dotnet run
```
- API chạy tại: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

### 2. Khởi động Frontend
```powershell
cd frontend
npm install
npm start
```
- Ứng dụng chạy tại: `http://localhost:4200`

---

## 👥 Tài khoản mặc định

| Vai trò | Tài khoản | Mật khẩu | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `Admin@123` | Toàn quyền (Thêm, sửa, xóa) |
| **User** | `user` | `User@123` | Xem dữ liệu |

---

## 📡 API Endpoints chính

> Hầu hết API (ngoại trừ Auth) yêu cầu header: `Authorization: Bearer <token>`.

| Nhóm | Endpoint | Chức năng |
| :--- | :--- | :--- |
| **Auth** | `POST /api/auth/login`, `POST /api/auth/register` | Xác thực người dùng |
| **Dashboard** | `GET /api/dashboard/stats` | Dữ liệu thống kê |
| **Students** | `GET/POST /api/students`, `GET/PUT/DELETE /api/students/{id}` | Quản lý học sinh |
| **Classes** | `GET/POST /api/classes`, `GET/PUT/DELETE /api/classes/{id}` | Quản lý lớp học |
| **Courses** | `GET/POST /api/courses`, `GET/PUT/DELETE /api/courses/{id}` | Quản lý môn học |
| **Enrollments** | `GET/POST /api/enrollments`, `PUT /api/enrollments/{id}/grade` | Đăng ký & cập nhật điểm |
