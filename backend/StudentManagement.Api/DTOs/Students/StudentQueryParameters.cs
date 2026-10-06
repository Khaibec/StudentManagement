namespace StudentManagement.Api.DTOs.Students;

public class StudentQueryParameters
{
    public int? Id { get; set; } // Tìm kiếm chính xác theo ID học sinh
    public string? SearchTerm { get; set; }
    public int? ClassRoomId { get; set; }
    public string? Gender { get; set; }
    public string? SortBy { get; set; } = "FullName";
    public string? SortOrder { get; set; } = "asc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
