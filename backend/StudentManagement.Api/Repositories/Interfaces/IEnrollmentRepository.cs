using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllAsync(int? studentId = null, int? courseId = null);
    Task<Enrollment?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int studentId, int courseId);
    Task<Enrollment> CreateAsync(Enrollment enrollment);
    Task<Enrollment> UpdateAsync(Enrollment enrollment);
    Task<bool> DeleteAsync(int id);
    Task<int> CountAsync();
    Task<IEnumerable<Enrollment>> GetRecentAsync(int count);
}
