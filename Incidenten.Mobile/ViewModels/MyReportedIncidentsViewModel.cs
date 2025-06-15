using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Shared.Api;

namespace Incidenten.Mobile.ViewModels;

public class MyReportedIncidentsViewModel : _BaseViewModel
{
    private readonly IIncidentApi _incidentApi;

    public MyReportedIncidentsViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;
        
        LoadData();
    }

    /* Fields */
    // List of incidents
    public List<Incident> Incidents { get; set; } = new();
    
    /*  Methods */
    /**
     * Load the incidents reported by the user from the backend.
     */
    public async Task LoadData()
    {
        try
        {
            Error = string.Empty;
            
            // Load the incidents and store them locally.
            var result = await _incidentApi.GetMyReportedIncidents();
            Incidents = result;
            
            // Notify the application about the incidents property changes.
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
        await Shell.Current.GoToAsync($"IncidentDetailsPage?id={id}");
    });
}