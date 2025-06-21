using System.Text.Json;
using System.Text.Json.Serialization;
using Incidenten.Mobile.Services;
using Incidenten.Mobile.ViewModels;
using Incidenten.Mobile.Views;
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
			})
			.UseMauiMaps();
		
		
		// Add auth service.
		builder.Services.AddSingleton<AuthService>();
		builder.Services.AddTransient<AuthTokenInjector>();

		builder.Services.AddRefitClient<ITestApi>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(apiBase);
			})
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new LoggingHandler(new HttpClientHandler());
			});;
		builder.Services.AddRefitClient<IUserApi>()
			.ConfigureHttpClient(client =>
			{
					client.BaseAddress = new Uri(apiBase);
			})
			.AddHttpMessageHandler<AuthTokenInjector>()
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new LoggingHandler(new HttpClientHandler());
			});;
		builder.Services
			.AddRefitClient<IIncidentApi>()
			.ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBase))
			.AddHttpMessageHandler<AuthTokenInjector>()
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new LoggingHandler(new HttpClientHandler());
			});
		builder.Services.AddRefitClient<IIncidentStatusApi>(new RefitSettings
			{
				ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
				{
					// Disable camelCase and uses PascalCase
					PropertyNamingPolicy = null,
				})
			})
			.ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBase))
			.AddHttpMessageHandler<AuthTokenInjector>()
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new LoggingHandler(new HttpClientHandler());
			});
		
		// Add ViewModels.
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<SignupViewModel>();
		builder.Services.AddTransient<UserViewModel>();

		builder.Services.AddTransient<CreateIncidentViewModel>();
		builder.Services.AddTransient<UpdateIncidentViewModel>();
		builder.Services.AddTransient<UpdateIncidentPage>();
		builder.Services.AddTransient<MyReportedIncidentsViewModel>();
		builder.Services.AddTransient<AllIncidentsViewModel>();
		builder.Services.AddTransient<IncidentDetailsViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
