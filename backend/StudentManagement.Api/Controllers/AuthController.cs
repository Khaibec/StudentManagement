using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Api.DTOs.Auth;
using StudentManagement.Api.DTOs.Common;
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
    /// Đăng nhập hệ thống bằng username và password
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Đăng nhập thành công!"));
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Đăng ký tài khoản người dùng mới (Role mặc định: User)
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Đăng ký tài khoản thành công!"));
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Lấy thông tin tài khoản hiện tại từ JWT Token (Yêu cầu đăng nhập)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(ApiResponse<UserProfileDto>.Fail("Token không hợp lệ hoặc đã hết hạn."));
        }

        try
        {
            var profile = await _authService.GetUserProfileAsync(userId);
            return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Lấy thông tin tài khoản thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserProfileDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Endpoint kiểm thử phân quyền: Chỉ dành cho tài khoản có Role = Admin
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only-test")]
    public IActionResult AdminOnlyTest()
    {
        return Ok(ApiResponse<string>.Ok("Xin chào Admin! Bạn có toàn quyền quản trị hệ thống."));
    }
}
