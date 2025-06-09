using System.Collections.ObjectModel;
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
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }
    private string _description = string.Empty;
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }
    
    // Incident location latitude.
    private double _latitude = 0.0;
    public double Latitude
    {
        get => _latitude;
        set
        {
            _latitude = value;
            OnPropertyChanged();
        }
    }
    
    // Incident location longitude.
    private double _longitude = 0.0;
    public double Longitude
    {
        get => _longitude;
        set
        {
            _longitude = value;
            OnPropertyChanged();
        }
    }

    private string _createdAt = string.Empty;
    public string CreatedAt
    {
        get => _createdAt;
        set
        {
            _createdAt = value;
            OnPropertyChanged();
        }
    }
    
    private string _updatedAt = string.Empty;
    public string UpdatedAt
    {
        get => _updatedAt;
        set
        {
            _updatedAt = value;
            OnPropertyChanged();
        }
    }
    
    private string _deadlineAt = string.Empty;
    public string DeadlineAt
    {
        get => _deadlineAt;
        set
        {
            _deadlineAt = value;
            OnPropertyChanged();
        }
    }
    
    private string _completedAt = string.Empty;
    public string CompletedAt
    {
        get => _completedAt;
        set
        {
            _completedAt = value;
            OnPropertyChanged();
        }
    }
    
    private string _status = string.Empty;
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }
    
    private string _priority = string.Empty;
    public string Priority
    {
        get => _priority;
        set
        {
            _priority = value;
            OnPropertyChanged();
        }
    }
    
    
    public ObservableCollection<string> Images { get; set; } = new(); 
    
    public async void LoadData(Guid id)
    {
        try
        {
            Error = string.Empty;
            
            var response = await _incidentApi.GetIncident(id);
            if (response == null)
            {
                Error = "The response was null.";
                // TODO: redirect to the 404 page.
                await Shell.Current.GoToAsync("//MainPage");
                return;
            }
            
            Name = response.Name;
            Description = response.Description ?? string.Empty;
            Priority = response.Priority.ToString();

            if (response.Status == IncidentStatus.Open || response.Status == IncidentStatus.Registered)
            {
                Status = response.Status.ToString();
            }
            else
            {
                var prefix = response.Status == IncidentStatus.InProgress ? "Is processed" : "Completed";
                Status = $"{prefix} by {response.Executor?.Email ?? "Unknown employee"}";
            }

            if (response.Location != null)
            {
                Latitude = response.Location.Latitude;
                Longitude = response.Location.Longitude;
                
                LocationChanged?.Invoke(new Location(Latitude, Longitude));
            }

            if (response.Images.Any())
            {
                foreach (var image in response.Images)
                {
                    Images.Add(image.ImageUrlAndroid);
                }
            }
            else
            {
                var defaultImage = new IncidentImage();
                Images.Add(defaultImage.ImageUrlAndroid);
            }
            
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
            OnPropertyChanged();
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    public event Action<Location>? LocationChanged;
}