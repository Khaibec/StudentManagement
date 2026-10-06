using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentManagement.Api.Data;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Repositories.Implementations;
using StudentManagement.Api.Services.Interfaces;
using StudentManagement.Api.Services.Implementations;
using StudentManagement.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext với SQL Server
// DI (Dependency Injection) sẽ tự động tạo và truyền ApplicationDbContext vào Repository/Service khi cần
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình CORS (Cross-Origin Resource Sharing)
// Mặc định trình duyệt sẽ chặn frontend (port 4200) gọi API của backend (port 5000) do khác domain/cổng.
// Chính sách này thông báo cho backend tin tưởng và chấp nhận request từ Angular (http://localhost:4200).
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 3. Cấu hình JWT Authentication (Xác thực người dùng qua mã token)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "StudentManagementSystemSecretKeyForJwtAuthentication2026!#*";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentManagementApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "StudentManagementClient";

builder.Services.AddAuthentication(options =>
{
    // Chỉ định cơ chế xác thực mặc định là JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Các tiêu chuẩn để backend kiểm tra token gửi lên có hợp lệ hay không:
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Kiểm tra token có đúng do hệ thống này phát hành (Issuer) không
        ValidateAudience = true, // Kiểm tra token có gửi đúng cho client được chỉ định (Audience) không
        ValidateLifetime = true, // Kiểm tra token còn hạn sử dụng hay đã hết hạn
        ValidateIssuerSigningKey = true, // Kiểm tra chữ ký bí mật của token
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), // Khóa bí mật dùng để giải mã / xác thực
        ClockSkew = TimeSpan.Zero // Mặc định .NET cho phép chênh lệch thời gian 5 phút. Đặt TimeSpan.Zero để token hết hạn chính xác từng giây.
    };
});

builder.Services.AddAuthorization();

// Đăng ký các tầng Repositories và Services vào DI Container
// AddScoped: Mỗi request HTTP gửi lên sẽ tạo ra một instance riêng biệt của service, 
// dùng chung suốt request đó và tự giải phóng khi request kết thúc.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// 4. Cấu hình Controllers
builder.Services.AddControllers();

// 5. Cấu hình Swagger kèm nút nhập Token JWT (Authorize)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student Management API",
        Version = "v1",
        Description = "Dự án Quản lý học sinh - ASP.NET Core Web API + Angular"
    });

    // Thêm định nghĩa bảo mật JWT vào Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT Bearer token theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. Tự động khởi tạo Database và Seed dữ liệu khi ứng dụng chạy
// Vì ApplicationDbContext được đăng ký là Scoped service (gắn với từng request),
// trong Program.cs chưa có request nào nên ta phải tự tạo một "Scope" thủ công để lấy DbContext ra dùng.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context); // Tự động tạo bảng và nạp dữ liệu mẫu ban đầu
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo và seed dữ liệu CSDL.");
    }
}

// 7. Cấu hình HTTP request pipeline (Thứ tự các Middleware cực kỳ quan trọng!)

// ExceptionHandlingMiddleware phải đặt ở ĐẦU TIÊN để bọc toàn bộ pipeline và bắt mọi lỗi xảy ra ở các bước sau
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Management API v1");
    });
}

app.UseHttpsRedirection();

// UseCors phải đặt trước UseAuthentication và UseAuthorization để xử lý các preflight OPTIONS request của trình duyệt
app.UseCors("AllowAngularApp");

// Quy tắc: UseAuthentication (Xác định bạn là ai?) phải chạy TRƯỚC UseAuthorization (Bạn có quyền làm gì?)
app.UseAuthentication();
app.UseAuthorization();

// Điều hướng request đến đúng Action trong Controller
app.MapControllers();

app.Run();
