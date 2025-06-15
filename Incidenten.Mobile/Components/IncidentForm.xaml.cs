using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Incidenten.Mobile.Interfaces;
using Microsoft.Maui.Controls.Maps;

namespace Incidenten.Mobile.Components;

public partial class IncidentForm : ContentView
{
    public IncidentForm()
    {
        InitializeComponent();
        BindingContextChanged += OnBindingContextChanged;
    }
    
    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is IMapAware vm)
        {
            // Make sure the MapClickedCommand is executed each time the map is clicked.
            IncidentMap.MapClicked += (_, args) => vm.MapClickedCommand.Execute(args.Location);
            
            // Watch the location changes.
            vm.LocationChanged += loc =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Remove all the previous pins on the map.
                    IncidentMap.Pins.Clear();
                    if (loc != null)
                    {
                        // Add the new pin with the provided location.
                        IncidentMap.Pins.Add(new Pin
                        {
                            Label = "Location",
                            Location = loc,
                            Type = PinType.Place
                        });
                    }
                });
            };
        }
    }
}