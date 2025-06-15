using System.Windows.Input;
using Incidenten.Mobile.Interfaces;
using Incidenten.Mobile.Models;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;
using Incidenten.Shared.Utils;

namespace Incidenten.Mobile.ViewModels;

[QueryProperty(nameof(IncidentIdString), "id")]
public class UpdateIncidentViewModel : BaseIncidentFormViewModel, IIncidentForm
{
    private readonly IIncidentApi _incidentApi;
    private readonly ValidationHelper _validationHelper = new ();
    
    public UpdateIncidentViewModel(IIncidentApi incidentApi) : base(incidentApi)
    {
        _incidentApi = incidentApi;

        SubmitIncidentFormCommand = new Command(async () => await UpdateIncident());
    }

    /* Fields */
    private string _incidentIdString = string.Empty;
    public string IncidentIdString
    {
        get => _incidentIdString;
        set
        {
            _incidentIdString = value;
            if (Guid.TryParse(value, out var parsed))
            {
                IncidentId = parsed;
                LoadData(parsed);
            }
        }
    }
    // Incident ID
    private Guid _incidentId;

    public Guid IncidentId
    {
        get => _incidentId;
        set => SetProperty(ref _incidentId, value);
    }
    // Form title
    public string FormTitle => "Update incident";
    // Label of the submit button
    public string SubmitIncidentFormLabel => "Update";
    
    /* Methods */
    /**
     * Load the incident data from the backend.
     */
    public async void LoadData(Guid id)
    {
        IncidentId = id;
        Error = string.Empty;
        
        try
        {
            // Load the incident data.
            var response = await _incidentApi.GetIncident(id);
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
            
            // Assign the location value.
            if (response.Location != null)
            {
                _locationHandler.SetLocation(response.Location.Latitude, response.Location.Longitude);
            }
            
            // Load the images.
            if (response.Images.Any())
            {
                var loadedImages = response.Images.Select(image => new ImageModel
                {
                    Source = image.ImageUrlAndroid,
                    Name = image.Filename,
                });
                
                _imageHandler.SetImages(loadedImages);
            }
            
            // Notify the application about the properties changes.
            OnPropertyChanged();
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    /**
     * Update the incident.
     */
    public async Task UpdateIncident()
    {
        Error = string.Empty;

        if (IncidentId == Guid.Empty)
        {
            Error = "No incident ID provided.";
            return;
        }
        
        // Validate the data.
        if (!_validationHelper.IsNotBlank(Name))
        {
            Error = "Name is required.";
            return;
        }

        try
        {
            // Update the incident regular data.
            await _incidentApi.UpdateIncident(IncidentId, new UpdateIncidentRequest
            {
                Name = Name,
                Description = Description,
            });

            // Update the incident images.
            if (_imageHandler == null) throw new Exception("No image handler");
            var parts = await _imageHandler.ToStreamParts();
            if (parts.Any())
            {
                await _incidentApi.UpdateImages(IncidentId, parts);
            }

            // Update the incident location.
            await _incidentApi.ProvideIncidentLocation(IncidentId, new ProvideIncidentLocationRequest
            {
                Latitude = Latitude,
                Longitude = Longitude,
            });
            
            // Redirect the user to the main page.
            await Shell.Current.GoToAsync("//MyReportedIncidentsPage");
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    /* Commands */
    public ICommand SubmitIncidentFormCommand { get; }
}