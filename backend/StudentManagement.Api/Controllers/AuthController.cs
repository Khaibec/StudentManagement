using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Auth;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng nhập hệ thống (dùng tài khoản admin/Admin@123 hoặc user/User@123)
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Đăng ký tài khoản người dùng mới (mặc định Role là User)
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(GetMe), new { }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy thông tin tài khoản hiện tại từ Token JWT (Yêu cầu đăng nhập)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
        }

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "Không tìm thấy người dùng." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Endpoint kiểm tra phân quyền (Chỉ tài khoản có Role = Admin mới được truy cập)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only-test")]
    public IActionResult AdminOnlyTest()
    {
        var username = User.Identity?.Name ?? "Admin";
        return Ok(new
        {
            message = $"Xin chào {username}! Bạn có quyền Quản trị viên (Admin) để truy cập tính năng này.",
            timestamp = DateTime.UtcNow
        });
    }
}
