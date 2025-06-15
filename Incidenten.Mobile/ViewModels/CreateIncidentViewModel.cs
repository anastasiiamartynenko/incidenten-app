using System.Windows.Input;
using Incidenten.Mobile.Interfaces;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;
using Incidenten.Shared.Utils;
using Exception = System.Exception;

namespace Incidenten.Mobile.ViewModels;

public class CreateIncidentViewModel : BaseIncidentFormViewModel, IIncidentForm
{
    private readonly IIncidentApi _incidentApi;
    private readonly ValidationHelper _validationHelper = new ();

    public CreateIncidentViewModel(IIncidentApi incidentApi) : base(incidentApi)
    {
        _incidentApi = incidentApi;
        
        SubmitIncidentFormCommand = new Command(async () => await CreateIncident());
    }
    
    /* Fields */
    // Form title
    public string FormTitle => "Create Incident";
    // Label of the submit button
    public string SubmitIncidentFormLabel => "Create";
    

    /* Methods */
    /**
     * Create an incident.
     */
    public async Task CreateIncident()
    {
        Error = string.Empty;

        // Validate the data.
        if (!_validationHelper.IsNotBlank(Name))
        {
            Error = "Name is required.";
            return;
        }

        try
        {
            // Create an incident.
            var incident = await _incidentApi.CreateIncident(new CreateIncidentRequest
            {
                Name = Name,
                Description = Description
            });

            // Upload the images.
            var parts = await _imageHandler.ToStreamParts();
            if (parts.Any())
            {
                await _incidentApi.UploadImages(incident.Id, parts);
            }
            
            // Provide the location of an incident.
            await _incidentApi.ProvideIncidentLocation(incident.Id, new ProvideIncidentLocationRequest
            {
                Latitude = Latitude,
                Longitude = Longitude
            });
            
            // Redirect the user to the main page.
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
    
    /* Commands */
    public ICommand SubmitIncidentFormCommand { get; }
}