using Microsoft.EntityFrameworkCore;
using StudentManagement.Api.Data;
using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;

namespace StudentManagement.Api.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(StudentQueryParameters query)
    {
        var q = _context.Students
            .Include(s => s.ClassRoom)
            .Include(s => s.Enrollments)
            .AsQueryable();

        // 1. Tìm kiếm theo Từ khóa (Tên, Mã SV, Email)
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.Trim().ToLower();
            q = q.Where(s => s.FullName.ToLower().Contains(term) ||
                             s.StudentCode.ToLower().Contains(term) ||
                             (s.Email != null && s.Email.ToLower().Contains(term)));
        }

        // 2. Lọc theo Lớp học
        if (query.ClassRoomId.HasValue && query.ClassRoomId.Value > 0)
        {
            q = q.Where(s => s.ClassRoomId == query.ClassRoomId.Value);
        }

        // 3. Lọc theo Giới tính
        if (!string.IsNullOrWhiteSpace(query.Gender))
        {
            var gender = query.Gender.Trim().ToLower();
            q = q.Where(s => s.Gender.ToLower() == gender);
        }

        var totalCount = await q.CountAsync();

        // 4. Sắp xếp (Sorting)
        bool isDesc = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        q = (query.SortBy?.ToLower()) switch
        {
            "studentcode" => isDesc ? q.OrderByDescending(s => s.StudentCode) : q.OrderBy(s => s.StudentCode),
            "dateofbirth" => isDesc ? q.OrderByDescending(s => s.DateOfBirth) : q.OrderBy(s => s.DateOfBirth),
            _ => isDesc ? q.OrderByDescending(s => s.FullName) : q.OrderBy(s => s.FullName)
        };

        // 5. Phân trang (Pagination)
        int page = query.PageNumber > 0 ? query.PageNumber : 1;
        int size = query.PageSize > 0 ? query.PageSize : 10;
        var items = await q.Skip((page - 1) * size).Take(size).ToListAsync();

        return (items, totalCount);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.ClassRoom)
            .Include(s => s.Enrollments)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Students
            .Include(s => s.ClassRoom)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _context.Students.FindAsync(id);
        if (item == null) return false;

        _context.Students.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string studentCode, int? excludeId = null)
    {
        return await _context.Students.AnyAsync(s =>
            s.StudentCode.ToLower() == studentCode.ToLower() && (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<int> CountAsync()
    {
        return await _context.Students.CountAsync();
    }

    public async Task<IEnumerable<Student>> GetRecentAsync(int count)
    {
        return await _context.Students
            .Include(s => s.ClassRoom)
            .OrderByDescending(s => s.Id)
            .Take(count)
            .ToListAsync();
    }
}
