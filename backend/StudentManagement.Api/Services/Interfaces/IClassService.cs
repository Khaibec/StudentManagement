using StudentManagement.Api.DTOs.Classes;

namespace StudentManagement.Api.Services.Interfaces;

public interface IClassService
{
    Task<IEnumerable<ClassDto>> GetAllClassesAsync();
    Task<ClassDto> GetClassByIdAsync(int id);
    Task<ClassDto> CreateClassAsync(CreateClassDto dto);
    Task<ClassDto> UpdateClassAsync(int id, UpdateClassDto dto);
    Task<bool> DeleteClassAsync(int id);
}
