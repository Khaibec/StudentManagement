using Microsoft.EntityFrameworkCore;
using StudentApi.Data;
using StudentApi.DTOs;
using StudentApi.Models;

namespace StudentApi.Services;

public class StudentService
{
    private readonly ApplicationDBContext _context;

    public StudentService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Select(student => ToDto(student))
            .ToListAsync();
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return student == null ? null : ToDto(student);
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
    {
        var student = new Student
        {
            StudentCode = dto.StudentCode,
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Email = dto.Email,
            ClassId = dto.ClassId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return ToDto(student);
    }

    public async Task<bool> UpdateAsync(int id, UpdateStudentDto dto)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        student.StudentCode = dto.StudentCode;
        student.FullName = dto.FullName;
        student.DateOfBirth = dto.DateOfBirth;
        student.Email = dto.Email;
        student.ClassId = dto.ClassId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return false;

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return true;
    }

    private static StudentDto ToDto(Student student) => new()
    {
        Id = student.Id,
        StudentCode = student.StudentCode,
        FullName = student.FullName,
        DateOfBirth = student.DateOfBirth,
        Email = student.Email,
        ClassId = student.ClassId
    };
}