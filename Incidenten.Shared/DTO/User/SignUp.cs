namespace Incidenten.Shared.DTO.User;

public class SignUpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool SendNotifications { get; set; } = true;
}

public class SignUpResponse
{
    public string? Token { get; set; }
}