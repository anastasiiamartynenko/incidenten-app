using System.Windows.Input;
using Incidenten.Mobile.Services;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.User;
using Incidenten.Shared.Utils;

namespace Incidenten.Mobile.ViewModels;

public class LoginViewModel : _BaseViewModel
{
    private readonly IUserApi _userApi;
    private readonly AuthService _authService;
    private readonly ValidationHelper _validationHelper = new ();

    public LoginViewModel(IUserApi userApi, AuthService authService)
    {
        _userApi = userApi;
        _authService = authService;
        LoginCommand = new Command(async () => await Login());
    }

    private string _email = String.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }
    
    
    private string _password = String.Empty;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand LoginCommand { get; }

    private async Task Login()
    {
        Error = string.Empty;

        // Validate email.
        if (!_validationHelper.IsValidEmail(Email))
        {
            Error = "Email is invalid.";
            return;
        }

        try
        {
            // Send LogIn request.
            var result = await _userApi.LogIn(new LogInRequest
            {
                Email = Email,
                Password = Password
            });

            if (result.Token != null)
            {
                // Set token.
                _authService.SetToken(result.Token);
                // Redirect to the home page.
                await Shell.Current.GoToAsync("//MainPage");
            }
            else Error = "Invalid credentials.";
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
}