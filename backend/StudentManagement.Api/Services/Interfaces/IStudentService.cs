using StudentManagement.Api.DTOs.Common;
using StudentManagement.Api.DTOs.Students;

namespace StudentManagement.Api.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResult<StudentDto>> GetStudentsAsync(StudentQueryParameters query);
    Task<StudentDetailDto> GetStudentByIdAsync(int id);
    Task<StudentDto> CreateStudentAsync(CreateStudentDto dto);
    Task<StudentDto> UpdateStudentAsync(int id, UpdateStudentDto dto);
    Task<bool> DeleteStudentAsync(int id);
}
