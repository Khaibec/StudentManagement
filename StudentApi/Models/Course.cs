using System;

namespace StudentApi.Models;

public class Course
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Credits { get; set; }

    public List<Enrollment> Enrollments { get; set; } = new();
}
