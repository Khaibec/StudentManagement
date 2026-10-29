using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;

namespace StudentManagement.Api.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByIdWithAccountsAsync(int id)
    {
        return await _context.Users
            .Include(u => u.ExternalAccounts)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<UserExternalAccount?> GetExternalAccountAsync(string provider, string providerKey)
    {
        return await _context.UserExternalAccounts
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Provider.ToLower() == provider.ToLower() && e.ProviderKey == providerKey);
    }

    public async Task<List<UserExternalAccount>> GetExternalAccountsByUserIdAsync(int userId)
    {
        return await _context.UserExternalAccounts
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserExternalAccount> AddExternalAccountAsync(UserExternalAccount account)
    {
        await _context.UserExternalAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> RemoveExternalAccountAsync(int userId, string provider)
    {
        var item = await _context.UserExternalAccounts
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Provider.ToLower() == provider.ToLower());

        if (item == null) return false;

        _context.UserExternalAccounts.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}
