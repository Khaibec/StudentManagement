namespace StudentManagement.Api.Entities;

public class Enrollment
{
    public int Id { get; set; }

    // Liên kết tới Học sinh
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    // Liên kết tới Khóa học
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public double? Grade { get; set; } // Điểm tổng kết (thang 10), có thể null khi mới đăng ký
}
