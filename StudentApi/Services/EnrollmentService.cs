using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class EnrollmentService
{
    private readonly ApplicationDBContext _context;

    public EnrollmentService(ApplicationDBContext context) => _context = context;

    public async Task<List<EnrollmentDto>> GetAllAsync() => await _context.Enrollments.AsNoTracking()
        .Select(item => new EnrollmentDto { StudentId = item.StudentId, CourseId = item.CourseId, Grade = item.Grade })
        .ToListAsync();

    public async Task<EnrollmentDto?> GetByIdAsync(int studentId, int courseId)
    {
        var item = await _context.Enrollments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.CourseId == courseId);
        return item == null ? null : ToDto(item);
    }

    public async Task<EnrollmentDto?> CreateAsync(CreateEnrollmentDto dto)
    {
        if (await _context.Enrollments.FindAsync(dto.StudentId, dto.CourseId) != null) return null;

        var item = new Enrollment { StudentId = dto.StudentId, CourseId = dto.CourseId, Grade = dto.Grade };
        _context.Enrollments.Add(item);
        await _context.SaveChangesAsync();
        return ToDto(item);
    }

    public async Task<bool> UpdateAsync(int studentId, int courseId, UpdateEnrollmentDto dto)
    {
        var item = await _context.Enrollments.FindAsync(studentId, courseId);
        if (item == null) return false;
        item.Grade = dto.Grade;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int studentId, int courseId)
    {
        var item = await _context.Enrollments.FindAsync(studentId, courseId);
        if (item == null) return false;
        _context.Enrollments.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    private static EnrollmentDto ToDto(Enrollment item) => new() { StudentId = item.StudentId, CourseId = item.CourseId, Grade = item.Grade };
}