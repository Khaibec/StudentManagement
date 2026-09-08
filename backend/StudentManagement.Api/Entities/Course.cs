using System.Text.Json.Serialization;

namespace StudentManagement.Api.Entities;

public class Course
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // Mã khóa học/môn học: MATH101, CS101
    public string Title { get; set; } = string.Empty; // Tên môn học
    public int Credits { get; set; } = 3; // Số tín chỉ
    public string? Description { get; set; }

    // Quan hệ N-N với Student thông qua bảng trung gian Enrollment
    [JsonIgnore]
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
