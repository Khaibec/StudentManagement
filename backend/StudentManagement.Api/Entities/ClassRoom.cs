using System.Text.Json.Serialization;

namespace StudentManagement.Api.Entities;

public class ClassRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ví dụ: "10A1", "12B3"
    public string? Description { get; set; }

    // Quan hệ 1-N: Một lớp có nhiều học sinh
    [JsonIgnore]
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
