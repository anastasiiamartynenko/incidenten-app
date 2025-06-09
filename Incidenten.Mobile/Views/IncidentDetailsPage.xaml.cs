using Incidenten.Mobile.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Incidenten.Mobile.Views;

[QueryProperty(nameof(IncidentId), "id")]
public partial class IncidentDetailsPage : ContentPage
{
    public IncidentDetailsPage(IncidentDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
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