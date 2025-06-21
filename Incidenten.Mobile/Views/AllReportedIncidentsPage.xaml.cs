using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class AllReportedIncidentsPage : ContentPage
{
    private readonly AllIncidentsViewModel _viewModel;
    
    public AllReportedIncidentsPage(AllIncidentsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadData();
        IncidentList.SetItemsSource(_viewModel.Incidents);
    }
}