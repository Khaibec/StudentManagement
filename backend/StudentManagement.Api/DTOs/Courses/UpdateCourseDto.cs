using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Courses;

public class UpdateCourseDto
{
    [Required(ErrorMessage = "Mã môn học không được để trống")]
    [MaxLength(20, ErrorMessage = "Mã môn học tối đa 20 ký tự")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên môn học không được để trống")]
    [MaxLength(150, ErrorMessage = "Tên môn học tối đa 150 ký tự")]
    public string Title { get; set; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
    public int Credits { get; set; } = 3;

    [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    public string? Description { get; set; }
}
