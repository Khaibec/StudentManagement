using StudentManagement.Api.DTOs.Courses;

namespace StudentManagement.Api.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto> GetCourseByIdAsync(int id);
    Task<CourseDto> CreateCourseAsync(CreateCourseDto dto);
    Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto);
    Task<bool> DeleteCourseAsync(int id);
}
