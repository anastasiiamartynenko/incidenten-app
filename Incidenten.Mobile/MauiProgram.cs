using Incidenten.Mobile.Services;
using Incidenten.Mobile.ViewModels;
using Incidenten.Shared.Api;
using Microsoft.Extensions.Logging;
using Refit;

namespace Incidenten.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		#if ANDROID
			var apiBase = "http://10.0.2.2:5000";
		#else
			var apiBase = "http://localhost:5000";
		#endif
		
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		
		// Add auth service.
		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddSingleton<AuthTokenInjector>();

		builder.Services.AddRefitClient<ITestApi>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(apiBase);
			});
		builder.Services.AddRefitClient<IUserApi>()
			.ConfigureHttpClient(client =>
			{
					client.BaseAddress = new Uri(apiBase);
			})
			.AddHttpMessageHandler<AuthTokenInjector>();
		
		// Add ViewModels.
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<SignupViewModel>();
		builder.Services.AddTransient<UserViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
