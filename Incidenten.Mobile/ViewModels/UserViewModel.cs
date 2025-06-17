using System.Windows.Input;
using Incidenten.Domain.Enums;
using Incidenten.Mobile.Services;
using Incidenten.Shared.Api;

namespace Incidenten.Mobile.ViewModels;

public class UserViewModel : _BaseViewModel
{
    private readonly IUserApi _userApi;
    private readonly AuthService _authService;

    public UserViewModel(IUserApi userApi, AuthService authService)
    {
        _userApi = userApi;
        _authService = authService;
        LoadData();

        ToggleNotificationsCommand = new Command(async () => await ToggleNotifications());
    }
    
    /* Fields */
    // Full name
    private string _fullName = string.Empty;
    public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
    // Email
    private string _email = string.Empty;
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    // Role of the user
    private UserRole _role = UserRole.Anonym;
    public UserRole Role { get => _role; set => SetProperty(ref _role, value); }
    // Toggle notifications button label
    private string _toggleNotificationsLabel = string.Empty;
    public string ToggleNotificationsLabel { get => _toggleNotificationsLabel; set => SetProperty(ref _toggleNotificationsLabel, value); }
    
    /* Methods */
    /**
     * Load the user data from the backend.
     */
    public async Task ToggleNotifications()
    {
        Error = string.Empty;
        try
        {
            await _userApi.ToggleNotifications();
            var newLabel = ToggleNotificationsLabel == "Turn on" ? "Turn off" : "Turn on";
            ToggleNotificationsLabel = newLabel;
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    public async void LoadData()
    {
        try
        {
            // Load the user data and store it in the fields.
            var user = await _userApi.GetMe();
            FullName = user.FullName;
            Email = user.Email;
            Role = user.Role;
            ToggleNotificationsLabel = user.SendNotifications ? "Turn off" : "Turn on";
        }
        catch (Exception ex)
        {
            Error = "An error occured" + ex.Message;
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    /*
     * Log out the user.
     */
    public void Logout()
    {
        // Remove the token from the local storage.
        _authService.RemoveToken();
        // Redirect the user to the main page.
        Shell.Current.GoToAsync("//MainPage");
    }
    
    /* Commands */
    public ICommand ToggleNotificationsCommand { get; set; }
}