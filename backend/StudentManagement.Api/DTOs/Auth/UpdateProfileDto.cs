using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Auth;

public class UpdateProfileDto
{
    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string? Email { get; set; }
}
