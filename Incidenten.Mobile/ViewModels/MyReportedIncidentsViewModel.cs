using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;

namespace Incidenten.Mobile.ViewModels;

public class MyReportedIncidentsViewModel : BaseIncidentsViewModel
{
    private readonly IIncidentApi _incidentApi;

    public MyReportedIncidentsViewModel(IIncidentApi incidentApi) : base(incidentApi)
    {
        _incidentApi = incidentApi;
        ShowActionButton = false;
    }
    
    /* Methods */
    /**
     * Fetch my reported incidents.
     */
    protected override async Task<ObservableCollection<Incident>> FetchIncidents(GetIncidentsFilter? filters = null)
    {
        var result = await _incidentApi.GetMyReportedIncidents();
        return new ObservableCollection<Incident>(result);
    }
}