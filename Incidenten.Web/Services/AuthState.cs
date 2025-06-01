namespace Incidenten.Web.Services;

public class AuthState
{
    public event Action? OnChange;
    public void NotifyChanged() => OnChange?.Invoke();
}