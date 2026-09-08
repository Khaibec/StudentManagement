using StudentManagement.Api.DTOs.Enrollments;

namespace StudentManagement.Api.Services.Interfaces;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsAsync(int? studentId = null, int? courseId = null);
    Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentDto dto);
    Task<EnrollmentDto> UpdateGradeAsync(int id, UpdateGradeDto dto);
    Task<bool> CancelEnrollmentAsync(int id);
}
