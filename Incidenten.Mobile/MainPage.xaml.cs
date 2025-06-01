using Incidenten.Mobile.Services;
using Incidenten.Shared.Api;

namespace Incidenten.Mobile;

public partial class MainPage : ContentPage
{
	private readonly AuthService _authService;

	public MainPage(AuthService authService)
	{
		InitializeComponent();
		_authService = authService;
	}

	private async void OnPlusClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("CreateIncidentPage");
	}
	
	private async void OnAccountClicked(object sender, EventArgs e)
	{
		var action = _authService.IsAuthenticated
			? await DisplayActionSheet(
				"Account", 
				"Cancel", 
				null, 
				"My account", 
				"Log out")
			: await DisplayActionSheet(
				"Account", 
				"Cancel", 
				null, 
				"Log in", 
				"Sign up");

		switch (action)
		{
			case "Log in":
				await Shell.Current.GoToAsync("LoginPage");
				break;
			case "Sign up":
				await Shell.Current.GoToAsync("SignupPage");
				break;
			case "My account":
				await Shell.Current.GoToAsync("UserPage");
				break;
			case "Log out":
				_authService.RemoveToken();
				await Shell.Current.GoToAsync("//MainPage");
				break;
		}
	}
}
