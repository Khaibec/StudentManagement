using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class ClassService
{
    private readonly ApplicationDBContext _context;

    public ClassService(ApplicationDBContext context) => _context = context;

    public async Task<List<ClassDto>> GetAllAsync() => await _context.Classes.AsNoTracking()
        .Select(item => new ClassDto { Id = item.Id, Name = item.Name }).ToListAsync();

    public async Task<ClassDto?> GetByIdAsync(int id)
    {
        var item = await _context.Classes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? null : new ClassDto { Id = item.Id, Name = item.Name };
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto)
    {
        var item = new Class { Name = dto.Name };
        _context.Classes.Add(item);
        await _context.SaveChangesAsync();
        return new ClassDto { Id = item.Id, Name = item.Name };
    }

    public async Task<bool> UpdateAsync(int id, UpdateClassDto dto)
    {
        var item = await _context.Classes.FindAsync(id);
        if (item == null) return false;
        item.Name = dto.Name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Classes.FindAsync(id);
        if (item == null) return false;
        _context.Classes.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
}