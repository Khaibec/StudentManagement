using StudentManagement.Api.Entities;

namespace StudentManagement.Api.Repositories.Interfaces;

public interface IClassRepository
{
    Task<IEnumerable<ClassRoom>> GetAllAsync();
    Task<ClassRoom?> GetByIdAsync(int id);
    Task<ClassRoom> CreateAsync(ClassRoom classRoom);
    Task<ClassRoom> UpdateAsync(ClassRoom classRoom);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<int> CountAsync();
}
