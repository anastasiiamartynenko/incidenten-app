using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Incidenten.Domain;

namespace Incidenten.Mobile.Components;

public partial class IncidentsCollection : ContentView
{
    public IncidentsCollection()
    {
        InitializeComponent();
    }
    
    /**
     * A helper function to set the items source
     */
    public void SetItemsSource(IEnumerable<Incident> items)
    {
        IncidentCollectionView.ItemsSource = items;
    }
    
    public static readonly BindableProperty ShowActionButtonProperty =
        BindableProperty.Create(nameof(ShowActionButton), typeof(bool), typeof(IncidentsCollection), false);

    public bool ShowActionButton
    {
        get => (bool)GetValue(ShowActionButtonProperty);
        set => SetValue(ShowActionButtonProperty, value);
    }

    public static readonly BindableProperty PickUpCommandProperty =
        BindableProperty.Create(nameof(PickUpCommand), typeof(ICommand), typeof(IncidentsCollection));

    public ICommand? PickUpCommand
    {
        get => (ICommand?)GetValue(PickUpCommandProperty);
        set => SetValue(PickUpCommandProperty, value);
    }

    public static readonly BindableProperty CompleteCommandProperty =
        BindableProperty.Create(nameof(CompleteCommand), typeof(ICommand), typeof(IncidentsCollection));

    public ICommand? CompleteCommand
    {
        get => (ICommand?)GetValue(CompleteCommandProperty);
        set => SetValue(CompleteCommandProperty, value);
    }
}