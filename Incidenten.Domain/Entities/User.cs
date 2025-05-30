using Incidenten.Domain.Enums;

namespace Incidenten.Domain;

public class User : _Base
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool SendNotifications { get; set; } = true;

    public UserRole Role { get; set; } = UserRole.Citizen;
}
