using StudentManagement.Api.DTOs.Auth;

namespace StudentManagement.Api.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<UserProfileDto> GetUserProfileAsync(int userId);

    // OAuth External Auth Methods
    Task<AuthResponseDto> ProcessExternalLoginAsync(string provider, string providerKey, string? email, string? displayName);
    Task<List<LinkedAccountDto>> GetLinkedAccountsAsync(int userId);
    Task<bool> LinkExternalAccountAsync(int userId, string provider, string providerKey, string? email, string? displayName);
    Task<bool> UnlinkExternalAccountAsync(int userId, string provider);
}
