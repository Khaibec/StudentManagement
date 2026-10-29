using StudentManagement.Api.DTOs.Auth;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // 1. Tìm user theo username
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new BadHttpRequestException("Tên đăng nhập hoặc mật khẩu không chính xác.");
        }

        // 2. Kiểm tra mật khẩu đã hash với BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new BadHttpRequestException("Tên đăng nhập hoặc mật khẩu không chính xác.");
        }

        // 3. Tạo JWT token
        var (token, expiresAt) = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // 1. Kiểm tra username đã tồn tại chưa
        bool exists = await _userRepository.ExistsByUsernameAsync(request.Username);
        if (exists)
        {
            throw new BadHttpRequestException("Tên đăng nhập đã được sử dụng. Vui lòng chọn tên khác.");
        }

        // 2. Hash mật khẩu và tạo entity User mới (mặc định Role là User)
        var newUser = new User
        {
            Username = request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName.Trim(),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(newUser);

        // 3. Tạo JWT token cho user mới
        var (token, expiresAt) = _tokenService.CreateToken(createdUser);

        return new AuthResponseDto
        {
            Token = token,
            Username = createdUser.Username,
            FullName = createdUser.FullName,
            Role = createdUser.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserProfileDto> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin người dùng.");
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            HasPassword = !string.IsNullOrWhiteSpace(user.PasswordHash),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<AuthResponseDto> ProcessExternalLoginAsync(string provider, string providerKey, string? email, string? displayName)
    {
        // 1. Tìm kiếm UserExternalAccount theo (Provider, ProviderKey)
        var externalAccount = await _userRepository.GetExternalAccountAsync(provider, providerKey);
        User user;

        if (externalAccount != null)
        {
            // Đã liên kết trước đó -> Đăng nhập bằng User tương ứng
            user = externalAccount.User;
        }
        else
        {
            // Chưa có liên kết với ProviderKey này. Kiểm tra xem có User nào trùng email không?
            User? existingUserByEmail = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                existingUserByEmail = await _userRepository.GetByEmailAsync(email);
            }

            if (existingUserByEmail != null)
            {
                // Auto-link: Tự động gắn tài khoản OAuth vào User có sẵn
                user = existingUserByEmail;
                await _userRepository.AddExternalAccountAsync(new UserExternalAccount
                {
                    UserId = user.Id,
                    Provider = provider,
                    ProviderKey = providerKey,
                    ProviderEmail = email,
                    ProviderDisplayName = displayName,
                    LinkedAt = DateTime.UtcNow
                });
            }
            else
            {
                // Người dùng hoàn toàn mới -> Tự sinh username và tạo User
                var username = await GenerateUniqueUsernameAsync(email, displayName, provider);
                var newUser = new User
                {
                    Username = username,
                    FullName = !string.IsNullOrWhiteSpace(displayName) ? displayName : username,
                    Email = email,
                    PasswordHash = string.Empty, // Người dùng tạo qua OAuth chưa có mật khẩu
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };

                user = await _userRepository.CreateAsync(newUser);

                // Thêm bản ghi liên kết OAuth
                await _userRepository.AddExternalAccountAsync(new UserExternalAccount
                {
                    UserId = user.Id,
                    Provider = provider,
                    ProviderKey = providerKey,
                    ProviderEmail = email,
                    ProviderDisplayName = displayName,
                    LinkedAt = DateTime.UtcNow
                });
            }
        }

        // Tạo JWT token cho User
        var (token, expiresAt) = _tokenService.CreateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = expiresAt
        };
    }

    public async Task<List<LinkedAccountDto>> GetLinkedAccountsAsync(int userId)
    {
        var linked = await _userRepository.GetExternalAccountsByUserIdAsync(userId);
        var supportedProviders = new[] { "Google", "Facebook", "GitHub" };

        var result = new List<LinkedAccountDto>();
        foreach (var p in supportedProviders)
        {
            var match = linked.FirstOrDefault(e => e.Provider.Equals(p, StringComparison.OrdinalIgnoreCase));
            result.Add(new LinkedAccountDto
            {
                Provider = p,
                IsLinked = match != null,
                ProviderEmail = match?.ProviderEmail,
                ProviderDisplayName = match?.ProviderDisplayName,
                LinkedAt = match?.LinkedAt
            });
        }

        return result;
    }

    public async Task<bool> LinkExternalAccountAsync(int userId, string provider, string providerKey, string? email, string? displayName)
    {
        // 1. Kiểm tra xem cặp (Provider, ProviderKey) này đã bị người dùng khác liên kết chưa
        var existing = await _userRepository.GetExternalAccountAsync(provider, providerKey);
        if (existing != null)
        {
            if (existing.UserId == userId)
            {
                return true; // Đã liên kết với chính user này rồi
            }
            throw new BadHttpRequestException($"Tài khoản {provider} này đã được liên kết với một tài khoản khác trong hệ thống.");
        }

        // 2. Kiểm tra xem User hiện tại đã liên kết Provider này chưa
        var userAccounts = await _userRepository.GetExternalAccountsByUserIdAsync(userId);
        if (userAccounts.Any(a => a.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // 3. Tạo mới bản ghi liên kết
        await _userRepository.AddExternalAccountAsync(new UserExternalAccount
        {
            UserId = userId,
            Provider = provider,
            ProviderKey = providerKey,
            ProviderEmail = email,
            ProviderDisplayName = displayName,
            LinkedAt = DateTime.UtcNow
        });

        // 4. Nếu user chưa có Email và bên ngoài có cung cấp Email, cập nhật luôn cho user
        var user = await _userRepository.GetByIdAsync(userId);
        if (user != null && string.IsNullOrWhiteSpace(user.Email) && !string.IsNullOrWhiteSpace(email))
        {
            user.Email = email;
            await _userRepository.UpdateAsync(user);
        }

        return true;
    }

    public async Task<bool> UnlinkExternalAccountAsync(int userId, string provider)
    {
        var user = await _userRepository.GetByIdWithAccountsAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin tài khoản người dùng.");
        }

        var targetAccount = user.ExternalAccounts.FirstOrDefault(e => e.Provider.Equals(provider, StringComparison.OrdinalIgnoreCase));
        if (targetAccount == null)
        {
            throw new BadHttpRequestException($"Tài khoản của bạn hiện chưa liên kết với {provider}.");
        }

        // Quy tắc an toàn: Không cho phép hủy liên kết tài khoản OAuth duy nhất nếu người dùng chưa thiết lập mật khẩu
        bool hasPassword = !string.IsNullOrWhiteSpace(user.PasswordHash);
        if (!hasPassword && user.ExternalAccounts.Count <= 1)
        {
            throw new BadHttpRequestException("Không thể hủy liên kết tài khoản duy nhất khi bạn chưa thiết lập mật khẩu đăng nhập! Vui lòng giữ ít nhất một phương thức đăng nhập.");
        }

        return await _userRepository.RemoveExternalAccountAsync(userId, provider);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string? email, string? displayName, string provider)
    {
        string baseUsername;
        if (!string.IsNullOrWhiteSpace(email))
        {
            baseUsername = email.Split('@')[0].ToLower();
        }
        else if (!string.IsNullOrWhiteSpace(displayName))
        {
            baseUsername = displayName.Replace(" ", "").ToLower();
        }
        else
        {
            baseUsername = $"{provider.ToLower()}_{Guid.NewGuid().ToString("N")[..6]}";
        }

        // Đảm bảo chỉ chứa ký tự chữ và số
        baseUsername = System.Text.RegularExpressions.Regex.Replace(baseUsername, @"[^a-z0-9_]", "");
        if (string.IsNullOrWhiteSpace(baseUsername)) baseUsername = $"{provider.ToLower()}_user";

        var username = baseUsername;
        int counter = 1;
        while (await _userRepository.ExistsByUsernameAsync(username))
        {
            username = $"{baseUsername}{counter}";
            counter++;
        }

        return username;
    }
}
