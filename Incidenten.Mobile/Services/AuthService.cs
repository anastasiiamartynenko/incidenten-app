namespace Incidenten.Mobile.Services;

public class AuthService
{
    public string? Token => Preferences.Get("token", null);
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    
    public void SetToken(string token) => Preferences.Set("token", token);
    public void RemoveToken() => Preferences.Remove("token");
}