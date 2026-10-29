namespace StudentManagement.Api.Entities;

public class UserExternalAccount
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Provider { get; set; } = string.Empty; // "Google", "Facebook", "GitHub"
    public string ProviderKey { get; set; } = string.Empty; // Unique Subject / User ID từ OAuth Provider
    public string? ProviderDisplayName { get; set; }
    public string? ProviderEmail { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
}
