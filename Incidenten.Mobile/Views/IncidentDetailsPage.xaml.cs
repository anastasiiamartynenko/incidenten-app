using Incidenten.Mobile.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Incidenten.Mobile.Views;

[QueryProperty(nameof(IncidentId), "id")]
[QueryProperty(nameof(ShowActionButtonString), "showAction")]
public partial class IncidentDetailsPage : ContentPage
{
    public IncidentDetailsPage(IncidentDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    
    private bool _showActionButton;

    public string ShowActionButtonString
    {
        set
        {
            if (bool.TryParse(value, out var result))
            {
                _showActionButton = result;
                if (BindingContext is IncidentDetailsViewModel viewModel)
                    viewModel.ShowActionButton = _showActionButton;
            }
        }
    }

    public string IncidentId
    {
        set
        {
            if (BindingContext is IncidentDetailsViewModel viewModel && Guid.TryParse(value, out var parsed))
            {
                viewModel.LocationChanged += location =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IncidentMap.Pins.Clear();
                        IncidentMap.Pins.Add(new Pin
                        {
                            Label = "Incident location",
                            Location = location,
                            Type = PinType.Place,
                        });
                        
                        IncidentMap.MoveToRegion(
                            MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1))
                        );
                    });
                };
                
                viewModel.LoadData(parsed);
            }
        }
    }
}