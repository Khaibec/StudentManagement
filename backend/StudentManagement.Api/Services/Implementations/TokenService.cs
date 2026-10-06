using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

// TokenService chịu trách nhiệm tạo chuỗi mã JWT (JSON Web Token) sau khi người dùng đăng nhập thành công.
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        // 1. Đọc cấu hình JWT từ file appsettings.json
        var keyString = _configuration["Jwt:Key"] ?? "StudentManagementSystemSecretKeyForJwtAuthentication2026!#*";
        var issuer = _configuration["Jwt:Issuer"] ?? "StudentManagementApi";
        var audience = _configuration["Jwt:Audience"] ?? "StudentManagementClient";
        var durationMinutes = int.TryParse(_configuration["Jwt:DurationInMinutes"], out var minutes) ? minutes : 1440;

        // 2. Tạo khóa bí mật đối xứng (Symmetric Key) và chỉ định thuật toán mã hóa HMAC-SHA256 để ký token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Thời điểm token hết hiệu lực
        var expiresAt = DateTime.UtcNow.AddMinutes(durationMinutes);

        // 3. Chuẩn bị Claims: Các mẩu thông tin định danh của người dùng được nhúng bên trong token
        // Frontend hoặc Backend có thể giải mã token để đọc các thông tin này mà không cần truy vấn lại Database.
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID của user
            new(ClaimTypes.Name, user.Username),                // Tên đăng nhập
            new(ClaimTypes.Role, user.Role),                    // Vai trò: Admin hoặc User (dùng để phân quyền)
            new("fullName", user.FullName)                      // Họ và tên hiển thị
        };

        // 4. Định nghĩa cấu hình đầy đủ của Token (Payload, Thời hạn, Chữ ký...)
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        // 5. Khởi tạo TokenHandler để sinh chuỗi ký tự Token theo chuẩn JWT (Header.Payload.Signature)
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Trả về chuỗi token dưới dạng chuỗi string và thời điểm hết hạn
        return (tokenHandler.WriteToken(token), expiresAt);
    }
}
