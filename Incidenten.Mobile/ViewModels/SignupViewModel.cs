using System.Windows.Input;
using Incidenten.Mobile.Services;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.User;
using Incidenten.Shared.Utils;

namespace Incidenten.Mobile.ViewModels;

public class SignupViewModel : _BaseViewModel
{
    private readonly IUserApi _userApi;
    private readonly AuthService _authService;
    private readonly ValidationHelper _validationHelper = new ();

    public SignupViewModel(IUserApi userApi, AuthService authService)
    {
        _userApi = userApi;
        _authService = authService;
        SignupCommand = new Command(async () => await SignUp());
    }
    
    /* Fields */
    // Fullname
    private string _fullName = String.Empty;
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    // Email
    private string _email = String.Empty;
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    // Password
    private string _password = String.Empty;
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    
    /* Methods */
    /**
     * Sign up the user.
     */
    private async Task SignUp()
    {
        Error = String.Empty;
        
        // Validate full name.
        if (!_validationHelper.IsNotBlank(FullName))
        {
            Error = "Fullname is blank.";
            return;
        }
        
        // Validate email.
        if (!_validationHelper.IsValidEmail(Email))
        {
            Error = "Email is not valid";
            return;
        }
        
        // Validate password.
        if (!_validationHelper.IsValidPassword(Password))
        {
            Error = "Password is not valid";
            return;
        }

        try
        {
            // Send SignUp request.
            var response = await _userApi.SignUp(new SignUpRequest
            {
                Email = Email,
                Password = Password,
                FullName = FullName
            });

            if (response.Token != null)
            {
                // Set token.
                _authService.SetToken(response.Token);
                // Redirect to the home page.
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
    
    /* Commands */
    public ICommand SignupCommand { get; }
}