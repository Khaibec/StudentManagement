using StudentManagement.Api.DTOs.Enrollments;
using StudentManagement.Api.Entities;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _studentRepository = studentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsAsync(int? studentId = null, int? courseId = null)
    {
        var items = await _enrollmentRepository.GetAllAsync(studentId, courseId);
        return items.Select(e => new EnrollmentDto
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentCode = e.Student?.StudentCode ?? string.Empty,
            StudentName = e.Student?.FullName ?? string.Empty,
            CourseId = e.CourseId,
            CourseCode = e.Course?.Code ?? string.Empty,
            CourseTitle = e.Course?.Title ?? string.Empty,
            Credits = e.Course?.Credits ?? 0,
            EnrollmentDate = e.EnrollmentDate,
            Grade = e.Grade
        });
    }

    public async Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentDto dto)
    {
        var student = await _studentRepository.GetByIdAsync(dto.StudentId);
        if (student == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy học sinh với ID = {dto.StudentId}");
        }

        var course = await _courseRepository.GetByIdAsync(dto.CourseId);
        if (course == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy môn học với ID = {dto.CourseId}");
        }

        if (await _enrollmentRepository.ExistsAsync(dto.StudentId, dto.CourseId))
        {
            throw new BadHttpRequestException($"Học sinh '{student.FullName}' đã đăng ký môn '{course.Title}' trước đó rồi.");
        }

        var enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            EnrollmentDate = DateTime.UtcNow,
            Grade = null
        };

        var created = await _enrollmentRepository.CreateAsync(enrollment);
        var refreshed = await _enrollmentRepository.GetByIdAsync(created.Id);

        return new EnrollmentDto
        {
            Id = created.Id,
            StudentId = created.StudentId,
            StudentCode = refreshed?.Student?.StudentCode ?? string.Empty,
            StudentName = refreshed?.Student?.FullName ?? string.Empty,
            CourseId = created.CourseId,
            CourseCode = refreshed?.Course?.Code ?? string.Empty,
            CourseTitle = refreshed?.Course?.Title ?? string.Empty,
            Credits = refreshed?.Course?.Credits ?? 0,
            EnrollmentDate = created.EnrollmentDate,
            Grade = created.Grade
        };
    }

    public async Task<EnrollmentDto> UpdateGradeAsync(int id, UpdateGradeDto dto)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);
        if (enrollment == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy bản ghi đăng ký với ID = {id}");
        }

        enrollment.Grade = dto.Grade;
        var updated = await _enrollmentRepository.UpdateAsync(enrollment);

        return new EnrollmentDto
        {
            Id = updated.Id,
            StudentId = updated.StudentId,
            StudentCode = updated.Student?.StudentCode ?? string.Empty,
            StudentName = updated.Student?.FullName ?? string.Empty,
            CourseId = updated.CourseId,
            CourseCode = updated.Course?.Code ?? string.Empty,
            CourseTitle = updated.Course?.Title ?? string.Empty,
            Credits = updated.Course?.Credits ?? 0,
            EnrollmentDate = updated.EnrollmentDate,
            Grade = updated.Grade
        };
    }

    public async Task<bool> CancelEnrollmentAsync(int id)
    {
        var existing = await _enrollmentRepository.GetByIdAsync(id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy bản ghi đăng ký với ID = {id}");
        }

        return await _enrollmentRepository.DeleteAsync(id);
    }
}
