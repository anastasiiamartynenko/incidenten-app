namespace Incidenten.Shared.DTO.User;

public class LogInRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LogInResponse
{
    public string? Token { get; set; }
}