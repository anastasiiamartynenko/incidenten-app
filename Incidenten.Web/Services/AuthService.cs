using Blazored.LocalStorage;

namespace Incidenten.Web.Services;

public class AuthService
{
    private readonly ILocalStorageService _localStorage;

    public AuthService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    public async Task SetTokenAsync(string token) => 
        await _localStorage.SetItemAsync("token", token);
    public async Task<string?> GetTokenAsync() =>
        await _localStorage.GetItemAsync<string>("token");
    public async Task RemoveTokenAsync() =>
        await _localStorage.RemoveItemAsync("token");

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");
        return !string.IsNullOrEmpty(token);
    }
}