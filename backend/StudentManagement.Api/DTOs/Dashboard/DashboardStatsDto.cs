using StudentManagement.Api.DTOs.Enrollments;
using StudentManagement.Api.DTOs.Students;

namespace StudentManagement.Api.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalStudents { get; set; }
    public int TotalClasses { get; set; }
    public int TotalCourses { get; set; }
    public int TotalEnrollments { get; set; }
    public List<StudentDto> RecentStudents { get; set; } = new();
    public List<EnrollmentDto> RecentEnrollments { get; set; } = new();
}
