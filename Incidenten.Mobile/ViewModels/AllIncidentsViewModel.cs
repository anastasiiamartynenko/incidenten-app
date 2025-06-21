using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;

namespace Incidenten.Mobile.ViewModels;

public class AllIncidentsViewModel : BaseIncidentsViewModel
{
    private readonly IIncidentApi _incidentApi;
    private readonly IIncidentStatusApi _incidentStatusApi;

    public AllIncidentsViewModel(IIncidentApi incidentApi, IIncidentStatusApi incidentStatusApi) : base(incidentApi)
    {
        _incidentApi = incidentApi;
        _incidentStatusApi = incidentStatusApi;
        ShowActionButton = true;

        PickUpCommand = new Command<Incident>(async (incident) => await PickUpIncident(incident));
        CompleteCommand = new Command<Incident>(async (incident) => await CompleteIncident(incident));
    }
    
    /* Methods */
    /**
     * Fetch all the incidents.
     */
    protected async override Task<ObservableCollection<Incident>> FetchIncidents()
    {
        var result = await _incidentApi.GetAllIncidents(new GetIncidentsFilter
        { 
            //
        });
        return new ObservableCollection<Incident>(result);
    }

    /**
     * Pick up an incident.
     */
    public async Task PickUpIncident(Incident incident)
    {
        try
        {
            await _incidentStatusApi.UpdateIncidentStatus(incident.Id,
                new UpdateIncidentStatusRequest { Status = IncidentStatus.InProgress, Priority = null });
            
            await LoadData();
            OnPropertyChanged(nameof(Incidents));
        }
        catch (Exception ex)
        {
            Error = "An error occured: " + ex.Message;
        }
    }

    /**
     * Complete an incident.
     */
    public async Task CompleteIncident(Incident incident)
    {
        try
        {
            await _incidentStatusApi.UpdateIncidentStatus(incident.Id,
                new UpdateIncidentStatusRequest { Status = IncidentStatus.Completed, Priority = null });
            
            await LoadData();
            OnPropertyChanged(nameof(Incidents));
        }
        catch (Exception ex)
        {
            Error = "An error occured: " + ex.Message;
        }
    }
    
    /* Commands */
    public ICommand PickUpCommand { get; set; }
    public ICommand CompleteCommand { get; set; }
}