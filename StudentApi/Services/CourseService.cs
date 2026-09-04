using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class CourseService
{
    private readonly ApplicationDBContext _context;

    public CourseService(ApplicationDBContext context) => _context = context;

    public async Task<List<CourseDto>> GetAllAsync() => await _context.Courses.AsNoTracking()
        .Select(item => new CourseDto { Id = item.Id, Code = item.Code, Name = item.Name, Credits = item.Credits })
        .ToListAsync();

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var item = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return item == null ? null : ToDto(item);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        var item = new Course { Code = dto.Code, Name = dto.Name, Credits = dto.Credits };
        _context.Courses.Add(item);
        await _context.SaveChangesAsync();
        return ToDto(item);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCourseDto dto)
    {
        var item = await _context.Courses.FindAsync(id);
        if (item == null) return false;
        item.Code = dto.Code;
        item.Name = dto.Name;
        item.Credits = dto.Credits;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Courses.FindAsync(id);
        if (item == null) return false;
        _context.Courses.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CourseDto ToDto(Course item) => new() { Id = item.Id, Code = item.Code, Name = item.Name, Credits = item.Credits };
}