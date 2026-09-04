namespace StudentApi.Models;

public class Student
{
    public int Id { get; set; }

    public string StudentCode { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; } = string.Empty;

    public int ClassId { get; set; }

    public Class Class { get; set; } = null!;

    public List<Enrollment> Enrollments { get; set; } = new();
}
