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

    public List<Incident> Incidents { get; set; } = new();
    
    public async Task LoadData()
    {
        try
        {
            Error = string.Empty;
            
            var result = await _incidentApi.GetMyReportedIncidents();
            Incidents = result;
            OnPropertyChanged(nameof(Incidents));
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    

    public ICommand GoToDetailsCommand => new Command<Guid>(async (id) =>
    {
        await Shell.Current.GoToAsync($"IncidentDetailsPage?id={id}");
    });
}