using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class CreateIncidentPage : ContentPage
{
    public CreateIncidentPage(CreateIncidentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}