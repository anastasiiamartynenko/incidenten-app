using Incidenten.Shared.Api;
using Microsoft.Extensions.Logging;
using Refit;

namespace Incidenten.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		#if ANDROID
			var apiBase = "http://10.0.2.2:5059";
		#else
			var apiBase = "http://localhost:5059";
		#endif
		
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddRefitClient<ITestApi>()
			.ConfigureHttpClient(client =>
			{
					client.BaseAddress = new Uri(apiBase);
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
