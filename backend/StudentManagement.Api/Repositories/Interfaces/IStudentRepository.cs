using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<(IEnumerable<Student> Items, int TotalCount)> GetPagedAsync(StudentQueryParameters query);
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByIdWithDetailsAsync(int id);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string studentCode, int? excludeId = null);
    Task<int> CountAsync();
    Task<IEnumerable<Student>> GetRecentAsync(int count);
}
