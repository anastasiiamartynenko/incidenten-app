using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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