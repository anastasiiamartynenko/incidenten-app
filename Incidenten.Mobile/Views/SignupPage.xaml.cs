using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class SignupPage : ContentPage
{
    public SignupPage(SignupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}