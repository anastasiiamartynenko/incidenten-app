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
		Routing.RegisterRoute("AllReportedIncidentsPage", typeof(AllReportedIncidentsPage));
		Routing.RegisterRoute("MainPage", typeof(MainPage));
		Routing.RegisterRoute("LoginPage", typeof(LoginPage));
		Routing.RegisterRoute("SignupPage", typeof(SignupPage));
		Routing.RegisterRoute("UserPage", typeof(UserPage));
		Routing.RegisterRoute("CreateIncidentPage", typeof(CreateIncidentPage));
		Routing.RegisterRoute("IncidentDetailsPage", typeof(IncidentDetailsPage));
		Routing.RegisterRoute("UpdateIncidentPage", typeof(UpdateIncidentPage));
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
		
		Items.Clear();

		var toAdd = new List<FlyoutItem>();
		
		var mainContent = new ShellContent
		{
			Title = "Main page",
			ContentTemplate = new DataTemplate(typeof(MainPage)),
			Route = "MainPage"
		};

		var main = new FlyoutItem
		{
			Title = "Main page",
			Route = "MainPage",
			Items = { mainContent }
		};
		
		var existing0 = Items.FirstOrDefault(i => i.Route == "MainPage");
		if (existing0 != null) Items.Remove(existing0);
			
		toAdd.Add(main);
		
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

			var content2 = new ShellContent
			{
				Title = "All reported incidents",
				ContentTemplate = new DataTemplate(typeof(AllReportedIncidentsPage)),
				Route = "AllReportedIncidentsPage"
			};
			var allIncidents = new FlyoutItem
			{
				Title = "All reported incidents",
				Route = "AllReportedIncidentsPage",
				Items = { content2 }
			};
			
			var existing = Items.FirstOrDefault(i => i.Route == "MyReportedIncidentsPage");
			if (existing != null) Items.Remove(existing);

			var existing2 = Items.FirstOrDefault(i => i.Route == "AllReportedIncidentsPage");
			if (existing2 != null) Items.Remove(existing2);
			
			toAdd.Add(myIncidents);
			if (authService.IsEmployeeOrOfficial)
			{
				toAdd.Add(allIncidents);
			}
		}

		foreach (var item in toAdd)
		{
			Items.Add(item);
		}
	}

	public async void AddConditionalMenuItems()
	{
		var authService = _serviceProvider.GetRequiredService<AuthService>();
		var isAuthenticated = authService.IsAuthenticated;
		var isEmployeeOrOfficial = authService.IsEmployeeOrOfficial;
		
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

		if (isEmployeeOrOfficial)
		{
			var allIncidentsItem = new ShellContent
			{
				Title = "All reported incidents",
				ContentTemplate = new DataTemplate(typeof(AllReportedIncidentsPage)),
				Route = "AllReportedIncidentsPage"
			};
			
			if (!Items.Contains(allIncidentsItem)) Items.Add(allIncidentsItem);
		}
	}
}
