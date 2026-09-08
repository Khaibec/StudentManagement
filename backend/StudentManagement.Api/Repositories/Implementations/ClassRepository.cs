using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;

namespace StudentManagement.Api.Repositories.Implementations;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDbContext _context;

    public ClassRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClassRoom>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Students)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<ClassRoom?> GetByIdAsync(int id)
    {
        return await _context.Classes
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ClassRoom> CreateAsync(ClassRoom classRoom)
    {
        await _context.Classes.AddAsync(classRoom);
        await _context.SaveChangesAsync();
        return classRoom;
    }

    public async Task<ClassRoom> UpdateAsync(ClassRoom classRoom)
    {
        _context.Classes.Update(classRoom);
        await _context.SaveChangesAsync();
        return classRoom;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Classes.FindAsync(id);
        if (item == null) return false;

        _context.Classes.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        return await _context.Classes.AnyAsync(c =>
            c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<int> CountAsync()
    {
        return await _context.Classes.CountAsync();
    }
}
