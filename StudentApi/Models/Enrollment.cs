using System;

namespace StudentApi.Models;

public class Enrollment
{
    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public double? Grade { get; set; }

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;
}
