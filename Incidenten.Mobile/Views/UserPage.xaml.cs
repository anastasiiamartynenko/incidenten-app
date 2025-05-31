using Incidenten.Mobile.Services;
using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class UserPage : ContentPage
{
    private readonly UserViewModel _userViewModel;
   
    public UserPage(UserViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _userViewModel = viewModel;
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        _userViewModel.Logout();
    }
}