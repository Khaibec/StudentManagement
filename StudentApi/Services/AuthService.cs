using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class AuthService
{
    private readonly ApplicationDBContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(ApplicationDBContext context, IConfiguration configuration,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<(AuthResponseDto? Response, string? Error)> RegisterAsync(RegisterDto dto)
    {
        var email = NormalizeEmail(dto.Email);
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return (null, "Email đã được sử dụng.");

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return (CreateAuthResponse(user), null);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == NormalizeEmail(dto.Email));
        if (user is null)
            return null;

        var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verification == PasswordVerificationResult.Failed)
            return null;

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            await _context.SaveChangesAsync();
        }

        return CreateAuthResponse(user);
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        return await _context.Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto(u.Id, u.FullName, u.Email, u.Role))
            .SingleOrDefaultAsync();
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        var jwt = _configuration.GetSection("Jwt");
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpiryMinutes"));
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"], audience: jwt["Audience"], claims: claims,
            notBefore: DateTime.UtcNow, expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new AuthResponseDto(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc,
            new UserDto(user.Id, user.FullName, user.Email, user.Role));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
