namespace StudentManagement.Api.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string? Description { get; set; }
    public int EnrolledCount { get; set; }
}
