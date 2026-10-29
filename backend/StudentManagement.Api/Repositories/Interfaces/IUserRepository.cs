using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByIdWithAccountsAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);

    Task<UserExternalAccount?> GetExternalAccountAsync(string provider, string providerKey);
    Task<List<UserExternalAccount>> GetExternalAccountsByUserIdAsync(int userId);
    Task<UserExternalAccount> AddExternalAccountAsync(UserExternalAccount account);
    Task<bool> RemoveExternalAccountAsync(int userId, string provider);
}
