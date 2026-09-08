using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Enrollments;

public class CreateEnrollmentDto
{
    [Required(ErrorMessage = "Vui lòng chọn học sinh")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn khóa học/môn học")]
    public int CourseId { get; set; }
}
