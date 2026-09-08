using StudentManagement.Api.DTOs.Dashboard;
using StudentManagement.Api.DTOs.Enrollments;
using StudentManagement.Api.DTOs.Students;
using StudentManagement.Api.Repositories.Interfaces;
using StudentManagement.Api.Services.Interfaces;

namespace StudentManagement.Api.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassRepository _classRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public DashboardService(
        IStudentRepository studentRepository,
        IClassRepository classRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _studentRepository = studentRepository;
        _classRepository = classRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var totalStudents = await _studentRepository.CountAsync();
        var totalClasses = await _classRepository.CountAsync();
        var totalCourses = await _courseRepository.CountAsync();
        var totalEnrollments = await _enrollmentRepository.CountAsync();

        var recentStudents = await _studentRepository.GetRecentAsync(5);
        var recentEnrollments = await _enrollmentRepository.GetRecentAsync(5);

        return new DashboardStatsDto
        {
            TotalStudents = totalStudents,
            TotalClasses = totalClasses,
            TotalCourses = totalCourses,
            TotalEnrollments = totalEnrollments,
            RecentStudents = recentStudents.Select(s => new StudentDto
            {
                Id = s.Id,
                StudentCode = s.StudentCode,
                FullName = s.FullName,
                DateOfBirth = s.DateOfBirth,
                Gender = s.Gender,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address,
                ClassRoomId = s.ClassRoomId,
                ClassRoomName = s.ClassRoom?.Name,
                EnrolledCoursesCount = s.Enrollments.Count
            }).ToList(),
            RecentEnrollments = recentEnrollments.Select(e => new EnrollmentDto
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
            }).ToList()
        };
    }
}
