using Incidenten.Mobile.Services;
using Incidenten.Mobile.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Incidenten.Mobile.Views;

public partial class CreateIncidentPage : ContentPage
{
    private readonly PermissionsService _permissionsService = new ();
    public CreateIncidentPage(CreateIncidentViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Execute the MapClicked command every time the map is clicked.
        IncidentMap.MapClicked += (_, e) => viewModel.MapClickedCommand.Execute(e.Location);
        
        // Watch the location changes and update the map view.
        viewModel.LocationChanged += location =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Clear the previously set pin.
                IncidentMap.Pins.Clear();
                if (location != null)
                {
                    // Add the new pin on the map with the updated location.
                    IncidentMap.Pins.Add(new Pin
                    {
                        Label = "Incident location",
                        Location = location,
                        Type = PinType.Place,
                    });
                }
            });
        };
    }
}