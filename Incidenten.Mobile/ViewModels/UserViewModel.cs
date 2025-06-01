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
    }
    
    private string _fullName = string.Empty;

    public string FullName
    {
        get => _fullName;
        set 
        {
            _fullName = value;
            OnPropertyChanged();
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }
    
    private UserRole _role = UserRole.Anonym;

    public UserRole Role
    {
        get => _role;
        set
        {
            _role = value;
            OnPropertyChanged();
        }
    }

    public async void LoadData()
    {
        try
        {
            var user = await _userApi.GetMe();
            FullName = user.FullName;
            Email = user.Email;
            Role = user.Role;
        }
        catch (Exception ex)
        {
            Error = "An error occured" + ex.Message;
            await Shell.Current.GoToAsync("//MainPage");
        }
    }

    public void Logout()
    {
        _authService.RemoveToken();
        Shell.Current.GoToAsync("//MainPage");
    }
}