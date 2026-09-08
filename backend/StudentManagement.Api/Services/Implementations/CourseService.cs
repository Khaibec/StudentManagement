using StudentManagement.Api.DTOs.Courses;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
    {
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(c => new CourseDto
        {
            Id = c.Id,
            Code = c.Code,
            Title = c.Title,
            Credits = c.Credits,
            Description = c.Description,
            EnrolledCount = c.Enrollments.Count
        });
    }

    public async Task<CourseDto> GetCourseByIdAsync(int id)
    {
        var c = await _courseRepository.GetByIdAsync(id);
        if (c == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy môn học với mã ID = {id}");
        }

        return new CourseDto
        {
            Id = c.Id,
            Code = c.Code,
            Title = c.Title,
            Credits = c.Credits,
            Description = c.Description,
            EnrolledCount = c.Enrollments.Count
        };
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseDto dto)
    {
        if (await _courseRepository.ExistsByCodeAsync(dto.Code.Trim()))
        {
            throw new BadHttpRequestException($"Mã môn học '{dto.Code}' đã tồn tại trong hệ thống.");
        }

        var course = new Course
        {
            Code = dto.Code.Trim().ToUpper(),
            Title = dto.Title.Trim(),
            Credits = dto.Credits,
            Description = dto.Description?.Trim()
        };

        var created = await _courseRepository.CreateAsync(course);

        return new CourseDto
        {
            Id = created.Id,
            Code = created.Code,
            Title = created.Title,
            Credits = created.Credits,
            Description = created.Description,
            EnrolledCount = 0
        };
    }

    public async Task<CourseDto> UpdateCourseAsync(int id, UpdateCourseDto dto)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy môn học với mã ID = {id}");
        }

        if (await _courseRepository.ExistsByCodeAsync(dto.Code.Trim(), id))
        {
            throw new BadHttpRequestException($"Mã môn học '{dto.Code}' đã tồn tại trong hệ thống.");
        }

        course.Code = dto.Code.Trim().ToUpper();
        course.Title = dto.Title.Trim();
        course.Credits = dto.Credits;
        course.Description = dto.Description?.Trim();

        var updated = await _courseRepository.UpdateAsync(course);

        return new CourseDto
        {
            Id = updated.Id,
            Code = updated.Code,
            Title = updated.Title,
            Credits = updated.Credits,
            Description = updated.Description,
            EnrolledCount = updated.Enrollments.Count
        };
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        var existing = await _courseRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy môn học với mã ID = {id}");
        }

        return await _courseRepository.DeleteAsync(id);
    }
}
