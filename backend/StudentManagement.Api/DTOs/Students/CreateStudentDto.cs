using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Api.DTOs.Students;

public class CreateStudentDto
{
    [Required(ErrorMessage = "Mã học sinh không được để trống")]
    [MaxLength(20, ErrorMessage = "Mã học sinh tối đa 20 ký tự")]
    public string StudentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ và tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ và tên tối đa 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Giới tính không được để trống")]
    public string Gender { get; set; } = "Nam";

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
    [MaxLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
    public string? PhoneNumber { get; set; }

    [MaxLength(250, ErrorMessage = "Địa chỉ tối đa 250 ký tự")]
    public string? Address { get; set; }

    public int? ClassRoomId { get; set; }
}
