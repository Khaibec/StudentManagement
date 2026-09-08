using StudentManagement.Api.DTOs.Auth;

namespace StudentManagement.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<UserResponseDto?> GetCurrentUserAsync(int userId);
}
