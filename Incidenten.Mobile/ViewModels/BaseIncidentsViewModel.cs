using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;

namespace Incidenten.Mobile.ViewModels;

public abstract class BaseIncidentsViewModel : _BaseViewModel
{
    private readonly IIncidentApi _incidentApi;
    
    public BaseIncidentsViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;
        LoadData();
    }
    
    /* Fields */
    public ObservableCollection<Incident> Incidents { get; set; } = new();
    private bool _showActionButton;
    public bool ShowActionButton { get => _showActionButton; set => SetProperty(ref _showActionButton, value); }

    
    /* Methods */
    protected abstract Task<ObservableCollection<Incident>> FetchIncidents(GetIncidentsFilter? filters = null);

    /**
     * Load the incidents.
     */
    public async Task LoadData(GetIncidentsFilter? filters = null)
    {
        Error = string.Empty;

        try
        {
            var result = await FetchIncidents(filters);
            Incidents.Clear();
            foreach (var incident in result)
            {
                Incidents.Add(incident);
            }
            OnPropertyChanged(nameof(Incidents));
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    /* Commands */
    public ICommand GoToDetailsCommand => new Command<Guid>(async (id) =>
    {
        // Redirect to the incident details page.
        await Shell.Current.GoToAsync($"IncidentDetailsPage?id={id}&showAction={ShowActionButton}");
    });
}