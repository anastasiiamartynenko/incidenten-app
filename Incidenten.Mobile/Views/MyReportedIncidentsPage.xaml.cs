using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Incidenten.Mobile.ViewModels;

namespace Incidenten.Mobile.Views;

public partial class MyReportedIncidentsPage : ContentPage
{
    private readonly MyReportedIncidentsViewModel _viewModel;
    public MyReportedIncidentsPage(MyReportedIncidentsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadData();
    }
}