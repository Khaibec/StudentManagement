namespace StudentManagement.Api.DTOs.Students;

public class StudentDetailDto : StudentDto
{
    public List<StudentCourseDto> EnrolledCourses { get; set; } = new();
}

public class StudentCourseDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public double? Grade { get; set; }
}
