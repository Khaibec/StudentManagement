using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();
    Task<Course?> GetByIdAsync(int id);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
    Task<int> CountAsync();
}
