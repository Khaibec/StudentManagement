using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Auth;

public class ChangePasswordDto
{
    // Mật khẩu hiện tại (có thể null nếu tài khoản tạo qua OAuth chưa có mật khẩu)
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    [MaxLength(100, ErrorMessage = "Mật khẩu mới tối đa 100 ký tự")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare(nameof(NewPassword), ErrorMessage = "Xác nhận mật khẩu không khớp với mật khẩu mới")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
