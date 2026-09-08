using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Services.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
