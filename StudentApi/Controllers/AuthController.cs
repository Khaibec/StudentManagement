using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentApi.DTOs;
using StudentApi.Services;

namespace StudentApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService) => _authService = authService;

    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var (response, error) = await _authService.RegisterAsync(dto);
        if (error is not null)
            return Conflict(new { message = error });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);
        return response is null
            ? Unauthorized(new { message = "Email hoặc mật khẩu không đúng." })
            : Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(id, out var userId))
            return Unauthorized();

        var user = await _authService.GetUserAsync(userId);
        return user is null ? Unauthorized() : Ok(user);
    }
}
