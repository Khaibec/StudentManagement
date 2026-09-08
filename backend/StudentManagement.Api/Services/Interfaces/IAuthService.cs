using StudentManagement.Api.DTOs.Auth;

namespace StudentManagement.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<UserProfileDto> GetUserProfileAsync(int userId);
}
