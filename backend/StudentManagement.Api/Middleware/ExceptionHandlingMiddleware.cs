using System.Net;
using System.Text.Json;
using StudentManagement.Api.DTOs.Common;

namespace StudentManagement.Api.Middleware;

// Middleware là một thành phần đứng chặn giữa luồng Request và Response trong ASP.NET Core.
// Lớp này giúp "bắt lỗi tập trung" (Global Exception Handling) cho toàn bộ ứng dụng,
// nhờ đó trong Controller ta không cần phải viết try-catch ở khắp mọi nơi.
public class ExceptionHandlingMiddleware
{
    // _next đại diện cho Middleware tiếp theo trong pipeline (hoặc Controller đích)
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Phương thức InvokeAsync sẽ được tự động gọi mỗi khi có một HTTP request đi qua
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Cho phép request tiếp tục đi tới các middleware phía sau hoặc vào Controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // Nếu có bất kỳ lỗi nào xảy ra ở Controller/Service mà chưa được bắt, nó sẽ rơi vào đây
            _logger.LogError(ex, "Đã xảy ra lỗi: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    // Chuyển đổi các Exception của C# thành HTTP Response JSON chuẩn mực cho Frontend
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Ánh xạ (Map) loại ngoại lệ thành mã HTTP Status Code tương ứng:
        // Cú pháp C# switch-expression giúp code ngắn gọn và dễ đọc
        var statusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,       // 404: Không tìm thấy dữ liệu
            BadHttpRequestException => (int)HttpStatusCode.BadRequest,  // 400: Dữ liệu gửi lên sai định dạng
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401: Chưa đăng nhập / không có quyền
            ArgumentException => (int)HttpStatusCode.BadRequest,        // 400: Tham số truyền vào không hợp lệ
            _ => (int)HttpStatusCode.InternalServerError                // 500: Lỗi hệ thống ngoài dự kiến
        };

        context.Response.StatusCode = statusCode;

        // Tránh lộ chi tiết lỗi kỹ thuật nhạy cảm (như connection string, stack trace) khi bị lỗi 500
        var message = statusCode == (int)HttpStatusCode.InternalServerError
            ? "Đã xảy ra lỗi máy chủ nội bộ. Vui lòng thử lại sau."
            : exception.Message;

        // Đóng gói thông báo lỗi vào định dạng thống nhất ApiResponse để Frontend dễ xử lý
        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Chuyển tên thuộc tính sang camelCase (chuẩn JavaScript)
        });

        return context.Response.WriteAsync(json);
    }
}
