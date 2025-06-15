using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Shared.Api;

namespace Incidenten.Mobile.ViewModels;

public class IncidentDetailsViewModel : _BaseViewModel
{
    private readonly IIncidentApi _incidentApi;

    public IncidentDetailsViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;

        DeleteIncidentCommand = new Command(async () => await DeleteIncident());
    }

    /* Fields */
    // Id
    private Guid _incidentId;
    public Guid IncidentId { get => _incidentId; set => SetProperty(ref _incidentId, value); }
    // Name
    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    // Description
    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    // Latitude
    private double _latitude = 0.0;
    public double Latitude { get => _latitude; set => SetProperty(ref _latitude, value); }
    // Longitude
    private double _longitude = 0.0;
    public double Longitude { get => _longitude; set => SetProperty(ref _longitude, value); }
    // Created at
    private string _createdAt = string.Empty;
    public string CreatedAt { get => _createdAt; set => SetProperty(ref _createdAt, value); }
    // Updated at
    private string _updatedAt = string.Empty;
    public string UpdatedAt { get => _updatedAt; set => SetProperty(ref _updatedAt, value); }
    // Deadline at
    private string _deadlineAt = string.Empty;
    public string DeadlineAt { get => _deadlineAt; set => SetProperty(ref _deadlineAt, value); }
    // Completed at
    private string _completedAt = string.Empty;
    public string CompletedAt { get => _completedAt; set => SetProperty(ref _completedAt, value); }
    // Status
    private string _status = string.Empty;
    public string Status { get => _status; set => SetProperty(ref _status, value); }
    // Priority
    private string _priority = string.Empty;
    public string Priority { get => _priority; set => SetProperty(ref _priority, value); }
    // Images
    public ObservableCollection<string> Images { get; set; } = new(); 
    
    /* Methods */
    /**
     * Load incident data from the backend
     */
    public async void LoadData(Guid id)
    {
        IncidentId = id;
        Error = string.Empty;

        try
        {
            // Get the incident.
            var response = await _incidentApi.GetIncident(id);
            
            // If the incident could have not been loaded, redirect the user to the main page.
            if (response == null)
            {
                Error = "The response was null.";
                // TODO: redirect to the 404 page.
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }
            
            // Assign the values.
            Name = response.Name;
            Description = response.Description ?? string.Empty;
            Priority = response.Priority.ToString();

            // Assign the status value.
            if (response.Status == IncidentStatus.Open || response.Status == IncidentStatus.Registered)
            {
                Status = response.Status.ToString();
            }
            else
            {
                var prefix = response.Status == IncidentStatus.InProgress ? "Is processed" : "Completed";
                Status = $"{prefix} by {response.Executor?.Email ?? "Unknown employee"}";
            }

            // Assign the value of the location.
            if (response.Location != null)
            {
                Latitude = response.Location.Latitude;
                Longitude = response.Location.Longitude;
                
                // Emit an event that notifies the application about the location changes.
                LocationChanged?.Invoke(new Location(Latitude, Longitude));
            }

            // Load the images.
            if (response.Images.Any())
            {
                foreach (var image in response.Images)
                {
                    Images.Add(image.ImageUrlAndroid);
                }
            }
            else
            {
                // If no images were provided for the incident, load the default image.
                var defaultImage = new IncidentImage();
                Images.Add(defaultImage.ImageUrlAndroid);
            }
            
            // Assign the date-time fields.
            CreatedAt = response.CreatedAt.ToString("dd-MM-yyyy HH:mm");
            if (!response.CreatedAt.Equals(response.UpdatedAt))
            {
                UpdatedAt = response.UpdatedAt.ToString("dd-MM-yyyy HH:mm");
            }
            if (response.DeadlineAt != null)
            {
                DeadlineAt = response.DeadlineAt?.ToString("dd-MM-yyyy HH:mm");
            }
            if (response.CompletedAt != null)
            {
                CompletedAt = response.CompletedAt?.ToString("dd-MM-yyyy HH:mm");
            }
            
            // Notify the application about the property changes.
            OnPropertyChanged();
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }

    public async Task DeleteIncident()
    {
        try
        {
            await _incidentApi.DeleteIncident(IncidentId);
            await Shell.Current.GoToAsync("//MyReportedIncidentsPage");
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    /* Events */
    public event Action<Location>? LocationChanged;
    
    /* Commands */
    public ICommand GoToUpdatePageCommand => new Command(async () =>
    {
        await Shell.Current.GoToAsync($"UpdateIncidentPage?id={IncidentId}");
    });

    public ICommand DeleteIncidentCommand { get; }
}