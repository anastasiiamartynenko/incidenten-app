using Incidenten.Shared.Api;

namespace Incidenten.Mobile;

public partial class MainPage : ContentPage
{
	public readonly ITestApi _testApi;

	public MainPage(ITestApi testApi)
	{
		InitializeComponent();
		_testApi = testApi;
	}

	public async void OnSendClicked(object sender, EventArgs e)
	{
		try
		{
			var input = InputEntry.Text;

			var response = await _testApi.SendTest(new TestRequest
			{
				testString = input
			});

			ResultLabel.Text = $"Response: {response.Result}";
		}
		catch (Exception ex)
		{
			ResultLabel.Text = $"Error: {ex.Message}";
		}
	}
}
