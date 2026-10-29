namespace StudentManagement.Api.DTOs.Auth;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool HasPassword { get; set; }
    public DateTime CreatedAt { get; set; }
}
