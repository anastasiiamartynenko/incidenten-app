using Incidenten.Shared.Api;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Incidenten.Web;
using MudBlazor.Services;
using Newtonsoft.Json;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrEmpty(apiBaseUrl))
    throw new Exception("API base URL is not configured!");

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var settings = new RefitSettings
{
    ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // fixes self-referencing loops
        PreserveReferencesHandling = PreserveReferencesHandling.Arrays
    })
};

builder.Services.AddRefitClient<ITestApi>(settings)
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiBaseUrl));

// TODO: uncomment if needed. Most likely it is not.
// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddMudServices();

await builder.Build().RunAsync();
