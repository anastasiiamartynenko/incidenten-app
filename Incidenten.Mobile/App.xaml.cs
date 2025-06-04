using Incidenten.Mobile.Services;
using Incidenten.Mobile.Views;

namespace Incidenten.Mobile;

public partial class App : Application
{
	private readonly IServiceProvider _serviceProvider;
	private readonly AuthService _authService;
	public App(IServiceProvider serviceProvider, AuthService authService)
	{
		InitializeComponent();
		_serviceProvider = serviceProvider;
		_authService = authService;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell(_serviceProvider, _authService));
	}
}