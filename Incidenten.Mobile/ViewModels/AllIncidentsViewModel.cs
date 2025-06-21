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
    
    /* Fields */
    // Filtering options
    public List<string> StatusOptions { get; } = new() { "None", "Registered", "InProgress", "Completed" };
    public List<string> PriorityOptions { get; } = new() { "None", "Low", "Regular", "High" };

    // Whether the current view is list view
    private bool _isListView = true;
    public bool IsListView
    {
        get => _isListView;
        set
        {
            if (SetProperty(ref _isListView, value))
            {
                // Notify that the isMapView variable has also been updated.
                OnPropertyChanged(nameof(IsMapView));
                // Reload the pins if the current view is map view.
                if (!value) LoadMapPins();
            }
        }
    }
    // Whether the current view is map view
    public bool IsMapView => !IsListView;
    // Selected status option
    private string _selectedStatus;
    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (SetProperty(ref _selectedStatus, value))
                // When the selected status is changed, reload the incidents with the filters applied.
                ApplyFilters();
        }
    }
    // Selected priority option
    private string _selectedPriority;
    public string SelectedPriority
    {
        get => _selectedPriority;
        set
        {
            if (SetProperty(ref _selectedPriority, value))
                // When the selected priority is changed, reload the incidents with the filters applied.
                ApplyFilters();
        }
    }
    
    /* Methods */
    /**
     * Fetch all the incidents.
     */
    protected async override Task<ObservableCollection<Incident>> FetchIncidents(GetIncidentsFilter? filters = null)
    {
        var result = await _incidentApi.GetAllIncidents(filters);
        return new ObservableCollection<Incident>(result);
    }

    /**
     * Returns the enum value of the status.
     */
    private IncidentStatus? StringToStatus(string status)
    {
        if (status == "Open") return IncidentStatus.Open;
        if (status == "Registered") return IncidentStatus.Registered;
        if (status == "InProgress") return IncidentStatus.InProgress;
        if (status == "Completed") return IncidentStatus.Completed;
        return null;
    }

    /**
     * Returns the enum value of the priority.
     */
    private IncidentPriority? StringToPriority(string priority)
    {
        if (priority == "Low") return IncidentPriority.Low;
        if (priority == "Regular") return IncidentPriority.Regular;
        if (priority == "High") return IncidentPriority.High;
        return null;
    }

    /**
     * Apply the filters and refresh the list of the incidents.
     */
    private async void ApplyFilters()
    {
        // Load the incidents from the backend with the corresponding filters applied.
        await LoadData(new GetIncidentsFilter
        {
            Status = StringToStatus(SelectedStatus),
            Priority = StringToPriority(SelectedPriority),
        });

        // Update map pins if the view is map view.
        if (IsMapView)
            OnMapUpdateRequested?.Invoke();
    }
    
    /**
     * Load the map pins.
     */
    private void LoadMapPins()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (OnMapUpdateRequested is null) return;
            OnMapUpdateRequested.Invoke();
        });
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
    
    /* Events */
    public event Action? OnMapUpdateRequested;
    
    /* Commands */
    public ICommand PickUpCommand { get; set; }
    public ICommand CompleteCommand { get; set; }
}