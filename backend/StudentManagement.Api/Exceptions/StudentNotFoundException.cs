namespace StudentManagement.Api.Exceptions;

/// <summary>
/// Ngoại lệ ném ra khi không tìm thấy học sinh theo ID được chỉ định.
/// Kế thừa KeyNotFoundException để ExceptionHandlingMiddleware tự động ánh xạ sang HTTP 404 Not Found.
/// </summary>
public class StudentNotFoundException : KeyNotFoundException
{
    public int StudentId { get; }

    public StudentNotFoundException(int studentId)
        : base($"Không tìm thấy học sinh với ID = {studentId}")
    {
        StudentId = studentId;
    }

    public StudentNotFoundException(string message)
        : base(message)
    {
    }
}
