using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incidenten.Mobile.ViewModels;
using Microsoft.Maui.Controls.Maps;

namespace Incidenten.Mobile.Views;

public partial class AllReportedIncidentsPage : ContentPage
{
    private readonly AllIncidentsViewModel _viewModel;
    
    public AllReportedIncidentsPage(AllIncidentsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        OnBindingContextChanged(this, EventArgs.Empty);
        BindingContextChanged += OnBindingContextChanged;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadData();
        IncidentList.SetItemsSource(_viewModel.Incidents);
    }

    protected void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (BindingContext is AllIncidentsViewModel viewModel)
        {
            viewModel.OnMapUpdateRequested += () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Clear all the previous pins from the map.
                    IncidentsMap.Pins.Clear();

                    foreach (var incident in viewModel.Incidents)
                    {
                        if (incident.Location != null)
                        {
                            // Add every incident's location as a pin on the map.
                            IncidentsMap.Pins.Add(new Pin
                            {
                                Label = incident.Name,
                                Location = new Location
                                {
                                    Latitude = incident.Location.Latitude,
                                    Longitude = incident.Location.Longitude
                                },
                                Type = PinType.Place
                            });
                        }
                    }
                });
            };
        }
    }
}