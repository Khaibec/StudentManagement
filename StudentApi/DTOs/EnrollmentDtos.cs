namespace StudentApi.DTOs;

public class EnrollmentDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double? Grade { get; set; }
}

public class CreateEnrollmentDto
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double? Grade { get; set; }
}

public class UpdateEnrollmentDto
{
    public double? Grade { get; set; }
}