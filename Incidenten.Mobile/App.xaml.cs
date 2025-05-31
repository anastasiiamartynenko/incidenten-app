using Incidenten.Mobile.Services;
using Incidenten.Mobile.Views;

namespace Incidenten.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		Routing.RegisterRoute("LoginPage", typeof(LoginPage));
		Routing.RegisterRoute("SignupPage", typeof(SignupPage));
		Routing.RegisterRoute("UserPage", typeof(UserPage));
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}