using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class UpdateIncidentPage : ContentPage
{
    public UpdateIncidentPage(UpdateIncidentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}