using System.Text.Json.Serialization;

namespace StudentManagement.Api.Entities;

public class Student
{
    public int Id { get; set; }
    public string StudentCode { get; set; } = string.Empty; // Mã học sinh duy nhất: SV001, SV002
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = "Nam"; // "Nam", "Nữ", "Khác"
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    // Khóa ngoại liên kết tới Lớp học (Quan hệ 1-N)
    public int? ClassRoomId { get; set; }
    public ClassRoom? ClassRoom { get; set; }

    // Quan hệ N-N với Course thông qua bảng trung gian Enrollment
    [JsonIgnore]
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
