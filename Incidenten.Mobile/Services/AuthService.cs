namespace Incidenten.Mobile.Services;

public class AuthService
{
    public event Action? OnChange;
    public void NotifyAuthChanged() => OnChange?.Invoke();

    public string? Token => Preferences.Get("token", null);
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public void SetToken(string token)
    {
        Preferences.Set("token", token);
        NotifyAuthChanged();
    }
    public void RemoveToken()
    {
        Preferences.Remove("token");
        NotifyAuthChanged();
    }
}