using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
}
