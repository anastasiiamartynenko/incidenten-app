using Incidenten.Domain.Enums;

namespace Incidenten.Mobile.Services;

public class AuthService
{
    public event Action? OnChange;
    public void NotifyAuthChanged() => OnChange?.Invoke();

    public string? Token => Preferences.Get("token", null);
    public string? Role => Preferences.Get("role", null);
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    public bool IsEmployeeOrOfficial => !string.IsNullOrEmpty(Role) &&
                                        (Role == "Employee" || Role == "Official" || Role == "2" || Role == "3");

    public void SetToken(string token)
    {
        Preferences.Set("token", token);
        NotifyAuthChanged();
    }

    public void SetRole(UserRole role)
    {
        Preferences.Set("role", role.ToString());
        NotifyAuthChanged();
    }
    
    public void RemoveToken()
    {
        Preferences.Remove("token");
        Preferences.Remove("role");
        NotifyAuthChanged();
    }
}