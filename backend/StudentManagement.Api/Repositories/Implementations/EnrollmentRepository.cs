using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;

namespace StudentManagement.Api.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync(int? studentId = null, int? courseId = null)
    {
        var q = _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .AsQueryable();

        if (studentId.HasValue && studentId.Value > 0)
        {
            q = q.Where(e => e.StudentId == studentId.Value);
        }

        if (courseId.HasValue && courseId.Value > 0)
        {
            q = q.Where(e => e.CourseId == courseId.Value);
        }

        return await q.OrderByDescending(e => e.EnrollmentDate).ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(int studentId, int courseId)
    {
        return await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<Enrollment> CreateAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Enrollments.FindAsync(id);
        if (item == null) return false;

        _context.Enrollments.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _context.Enrollments.CountAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetRecentAsync(int count)
    {
        return await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrollmentDate)
            .Take(count)
            .ToListAsync();
    }
}
