using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Enrollments;

public class UpdateGradeDto
{
    [Range(0.0, 10.0, ErrorMessage = "Điểm số phải từ 0 đến 10")]
    public double? Grade { get; set; }
}
