namespace StudentManagement.Api.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = "User"; // "Admin" hoặc "User"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Danh sách tài khoản liên kết mạng xã hội (OAuth)
    public ICollection<UserExternalAccount> ExternalAccounts { get; set; } = new List<UserExternalAccount>();
}
