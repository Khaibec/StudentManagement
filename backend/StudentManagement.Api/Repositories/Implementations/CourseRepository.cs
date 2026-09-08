using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;

namespace StudentManagement.Api.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Course>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .OrderBy(c => c.Code)
            .ToListAsync();
    }

    public async Task<Course?> GetByIdAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course> CreateAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
        return course;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Courses.FindAsync(id);
        if (item == null) return false;

        _context.Courses.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
    {
        return await _context.Courses.AnyAsync(c =>
            c.Code.ToLower() == code.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<int> CountAsync()
    {
        return await _context.Courses.CountAsync();
    }
}
