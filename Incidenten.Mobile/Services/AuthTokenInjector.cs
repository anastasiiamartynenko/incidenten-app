using System.Net.Http.Headers;
using Incidenten.Domain;

namespace Incidenten.Mobile.Services;

public class AuthTokenInjector : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthTokenInjector(AuthService authService)
    {
        _authService = authService;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var token = _authService.Token;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        return base.SendAsync(request, cancellationToken);
    }
}