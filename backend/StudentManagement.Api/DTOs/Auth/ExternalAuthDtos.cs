namespace StudentManagement.Api.DTOs.Auth;

public class LinkedAccountDto
{
    public string Provider { get; set; } = string.Empty; // "Google", "Facebook", "GitHub"
    public bool IsLinked { get; set; }
    public string? ProviderEmail { get; set; }
    public string? ProviderDisplayName { get; set; }
    public DateTime? LinkedAt { get; set; }
}

public class UnlinkAccountRequestDto
{
    public string Provider { get; set; } = string.Empty;
}
