using Incidenten.Mobile.Services;
using Incidenten.Mobile.Views;

namespace Incidenten.Mobile;

public partial class AppShell : Shell
{
	private readonly IServiceProvider _serviceProvider;
	private readonly AuthService _authState;
	public AppShell(IServiceProvider serviceProvider, AuthService authState)
	{
		InitializeComponent();
		_serviceProvider = serviceProvider;
		_authState = authState;

		_authState.OnChange += AuthChanged;

		RegisterRoutes();
		AddConditionalMenuItems();
	}

	private void RegisterRoutes()
	{
		Routing.RegisterRoute("MyReportedIncidentsPage", typeof(MyReportedIncidentsPage));
		Routing.RegisterRoute("MainPage", typeof(MainPage));
		Routing.RegisterRoute("LoginPage", typeof(LoginPage));
		Routing.RegisterRoute("SignupPage", typeof(SignupPage));
		Routing.RegisterRoute("UserPage", typeof(UserPage));
		Routing.RegisterRoute("CreateIncidentPage", typeof(CreateIncidentPage));
	}

	private void AuthChanged()
	{
		MainThread.BeginInvokeOnMainThread(async () =>
		{
			await RefreshMenuAsync();
		});
	}

	private async Task RefreshMenuAsync()
	{
		var authService = _serviceProvider.GetRequiredService<AuthService>();
		
		var toRemove = Items
			.OfType<FlyoutItem>()
			.Where(i => i.Route is "MyReportedIncidentsPage")
			.ToList();

		foreach (var item in toRemove)
		{
			Items.Remove(item);
		}

		var toAdd = new List<FlyoutItem>();
		
		if (authService.IsAuthenticated)
		{
			var content = new ShellContent
			{
				Title = "My reported incidents",
				ContentTemplate = new DataTemplate(typeof(MyReportedIncidentsPage)),
				Route = "MyReportedIncidentsPage"
			};

			var myIncidents = new FlyoutItem
			{
				Title = "My reported incidents",
				Route = "MyReportedIncidentsPage",
				Items = { content }
			};
			toAdd.Add(myIncidents);
		}

		foreach (var item in toAdd)
		{
			Items.Add(item);
		}
	}

	private async void AddConditionalMenuItems()
	{
		var authService = _serviceProvider.GetRequiredService<AuthService>();
		var isAuthenticated = authService.IsAuthenticated;

		if (isAuthenticated)
		{
			var myIncidentsItem = new ShellContent
			{
				Title = "My reported incidents",
				ContentTemplate = new DataTemplate(typeof(MyReportedIncidentsPage)),
				Route = "MyReportedIncidentsPage"
			};

			if (!Items.Contains(myIncidentsItem)) Items.Add(myIncidentsItem); 
		}
	}
}
