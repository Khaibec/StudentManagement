namespace StudentManagement.Api.DTOs.Students;

public class StudentDto
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public int? ClassRoomId { get; set; }
    public string? ClassRoomName { get; set; }
    public int EnrolledCoursesCount { get; set; }
}
