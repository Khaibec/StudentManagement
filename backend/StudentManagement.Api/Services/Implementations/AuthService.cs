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
        if (user == null)
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
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
