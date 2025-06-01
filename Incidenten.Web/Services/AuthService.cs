using Blazored.LocalStorage;

namespace Incidenten.Web.Services;

public class AuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly AuthState _authState;

    public AuthService(ILocalStorageService localStorage, AuthState authState)
    {
        _localStorage = localStorage;
        _authState = authState;
    }

    public async Task SetTokenAsync(string token)
    {
        await _localStorage.SetItemAsync("token", token);
        _authState.NotifyChanged();
    }
    public async Task<string?> GetTokenAsync() =>
        await _localStorage.GetItemAsync<string>("token");

    public async Task RemoveTokenAsync()
    {
        await _localStorage.RemoveItemAsync("token");
        _authState.NotifyChanged();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }
}