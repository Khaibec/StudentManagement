using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Classes;

public class CreateClassDto
{
    [Required(ErrorMessage = "Tên lớp không được để trống")]
    [MaxLength(50, ErrorMessage = "Tên lớp tối đa 50 ký tự")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Mô tả tối đa 200 ký tự")]
    public string? Description { get; set; }
}
