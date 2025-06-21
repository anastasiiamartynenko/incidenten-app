namespace Incidenten.Mobile.Services;

public class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler innerHandler)
        : base(innerHandler) { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Request:");
        Console.WriteLine(request.Method + " " + request.RequestUri);

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync();
            Console.WriteLine("Request Body: " + content);
        }

        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine("Response:");
        Console.WriteLine($"Status: {response.StatusCode}");

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Response Body: " + responseBody);

        return response;
    }
}
