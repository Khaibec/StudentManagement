using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Api.DTOs.Auth;
using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
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
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(ApiResponse<UserProfileDto>.Fail("Token không hợp lệ hoặc đã hết hạn."));
        }

        try
        {
            var profile = await _authService.GetUserProfileAsync(userId.Value);
            return Ok(ApiResponse<UserProfileDto>.Ok(profile, "Lấy thông tin tài khoản thành công."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<UserProfileDto>.Fail(ex.Message));
        }
    }

    /// <summary>
    /// Bắt đầu luồng đăng nhập với OAuth Provider (Google, Facebook, GitHub)
    /// </summary>
    [HttpGet("external-login")]
    public IActionResult ExternalLogin([FromQuery] string provider)
    {
        var normalizedProvider = NormalizeProviderName(provider);
        if (string.IsNullOrEmpty(normalizedProvider))
        {
            return BadRequest(ApiResponse<object>.Fail("Nhà cung cấp OAuth không được hỗ trợ. Vui lòng chọn Google, Facebook hoặc GitHub."));
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalCallback))
        };
        properties.Items["action"] = "login";
        properties.Items["provider"] = normalizedProvider;

        return Challenge(properties, normalizedProvider);
    }

    /// <summary>
    /// Bắt đầu luồng liên kết tài khoản OAuth khi người dùng ĐÃ ĐĂNG NHẬP
    /// </summary>
    [HttpGet("link-external")]
    public IActionResult LinkExternal([FromQuery] string provider, [FromQuery] string? accessToken)
    {
        var normalizedProvider = NormalizeProviderName(provider);
        if (string.IsNullOrEmpty(normalizedProvider))
        {
            return Redirect("http://localhost:4200/profile?linkError=" + Uri.EscapeDataString("Nhà cung cấp OAuth không hợp lệ."));
        }

        int? userId = null;
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            userId = ValidateAndExtractUserIdFromToken(accessToken);
        }
        else
        {
            userId = GetCurrentUserId();
        }

        if (!userId.HasValue)
        {
            return Redirect("http://localhost:4200/profile?linkError=" + Uri.EscapeDataString("Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để liên kết."));
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalCallback))
        };
        properties.Items["action"] = "link";
        properties.Items["userId"] = userId.Value.ToString();
        properties.Items["provider"] = normalizedProvider;

        return Challenge(properties, normalizedProvider);
    }

    /// <summary>
    /// Điểm tiếp nhận phản hồi (Callback) từ Provider OAuth sau khi người dùng đồng ý cấp quyền
    /// </summary>
    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalCallback()
    {
        var authResult = await HttpContext.AuthenticateAsync("ExternalAuth");
        if (!authResult.Succeeded || authResult.Principal == null)
        {
            var error = authResult.Failure?.Message ?? "Xác thực tài khoản bên thứ ba thất bại.";
            return Redirect($"http://localhost:4200/auth/callback?error={Uri.EscapeDataString(error)}");
        }

        var action = (authResult.Properties?.Items.TryGetValue("action", out var act) == true && !string.IsNullOrEmpty(act)) ? act : "login";
        var provider = (authResult.Properties?.Items.TryGetValue("provider", out var prov) == true && !string.IsNullOrEmpty(prov)) ? prov : "Google";

        // Trích xuất các Claims định danh người dùng từ Provider
        var providerKey = authResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? authResult.Principal.FindFirstValue("sub")
                          ?? authResult.Principal.FindFirstValue("id");

        var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
        var name = authResult.Principal.FindFirstValue(ClaimTypes.Name)
                   ?? authResult.Principal.FindFirstValue("name");

        if (string.IsNullOrEmpty(providerKey))
        {
            await HttpContext.SignOutAsync("ExternalAuth");
            return Redirect("http://localhost:4200/auth/callback?error=" + Uri.EscapeDataString("Không thể lấy thông tin định danh từ nhà cung cấp OAuth."));
        }

        try
        {
            if (action == "link")
            {
                // Thao tác LIÊN KẾT TÀI KHOẢN
                if (authResult.Properties?.Items.TryGetValue("userId", out var uidStr) != true || !int.TryParse(uidStr, out int userId))
                {
                    await HttpContext.SignOutAsync("ExternalAuth");
                    return Redirect("http://localhost:4200/profile?linkError=" + Uri.EscapeDataString("Không xác định được tài khoản cần liên kết."));
                }

                await _authService.LinkExternalAccountAsync(userId, provider, providerKey, email, name);
                await HttpContext.SignOutAsync("ExternalAuth");
                return Redirect($"http://localhost:4200/profile?linkSuccess=true&provider={Uri.EscapeDataString(provider)}");
            }
            else
            {
                // Thao tác ĐĂNG NHẬP
                var loginResponse = await _authService.ProcessExternalLoginAsync(provider, providerKey, email, name);
                await HttpContext.SignOutAsync("ExternalAuth");

                var redirectUrl = $"http://localhost:4200/auth/callback" +
                                  $"?token={Uri.EscapeDataString(loginResponse.Token)}" +
                                  $"&username={Uri.EscapeDataString(loginResponse.Username)}" +
                                  $"&fullName={Uri.EscapeDataString(loginResponse.FullName)}" +
                                  $"&role={Uri.EscapeDataString(loginResponse.Role)}";

                return Redirect(redirectUrl);
            }
        }
        catch (Exception ex)
        {
            await HttpContext.SignOutAsync("ExternalAuth");
            if (action == "link")
            {
                return Redirect($"http://localhost:4200/profile?linkError={Uri.EscapeDataString(ex.Message)}");
            }
            return Redirect($"http://localhost:4200/auth/callback?error={Uri.EscapeDataString(ex.Message)}");
        }
    }

    /// <summary>
    /// Lấy danh sách trạng thái liên kết các bên thứ 3 (Google, Facebook, GitHub) của tài khoản hiện tại
    /// </summary>
    [Authorize]
    [HttpGet("external-accounts")]
    public async Task<ActionResult<ApiResponse<List<LinkedAccountDto>>>> GetExternalAccounts()
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        var accounts = await _authService.GetLinkedAccountsAsync(userId.Value);
        return Ok(ApiResponse<List<LinkedAccountDto>>.Ok(accounts, "Lấy danh sách tài khoản liên kết thành công."));
    }

    /// <summary>
    /// Hủy liên kết tài khoản bên thứ 3 (Google, Facebook hoặc GitHub)
    /// </summary>
    [Authorize]
    [HttpDelete("external-accounts/{provider}")]
    public async Task<ActionResult<ApiResponse<bool>>> UnlinkExternalAccount(string provider)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var success = await _authService.UnlinkExternalAccountAsync(userId.Value, provider);
            return Ok(ApiResponse<bool>.Ok(success, $"Đã hủy liên kết tài khoản {provider} thành công."));
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ApiResponse<bool>.Fail(ex.Message));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.Fail(ex.Message));
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

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int id) ? id : null;
    }

    private static string? NormalizeProviderName(string provider)
    {
        return provider.ToLower() switch
        {
            "google" => "Google",
            "facebook" => "Facebook",
            "github" => "GitHub",
            _ => null
        };
    }

    private int? ValidateAndExtractUserIdFromToken(string token)
    {
        try
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "StudentManagementSystemSecretKeyForJwtAuthentication2026!#*";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "StudentManagementApi";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "StudentManagementClient";

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            }, out _);

            var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idClaim, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}
