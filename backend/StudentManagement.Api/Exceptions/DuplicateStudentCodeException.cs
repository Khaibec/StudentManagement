namespace StudentManagement.Api.Exceptions;

/// <summary>
/// Ngoại lệ nghiệp vụ ném ra khi mã học sinh bị trùng lặp trong hệ thống.
/// Kế thừa BadHttpRequestException để ExceptionHandlingMiddleware tự động ánh xạ sang HTTP 400 Bad Request.
/// </summary>
public class DuplicateStudentCodeException : BadHttpRequestException
{
    public string StudentCode { get; }

    public DuplicateStudentCodeException(string studentCode)
        : base($"Mã học sinh '{studentCode}' đã tồn tại trong hệ thống.")
    {
        StudentCode = studentCode;
    }
}
