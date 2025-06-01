using System.Windows.Input;
using Incidenten.Domain;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;
using Incidenten.Shared.Utils;

namespace Incidenten.Mobile.ViewModels;

public class CreateIncidentViewModel : _BaseViewModel
{
    private readonly IIncidentApi _incidentApi;
    private readonly ValidationHelper _validationHelper = new ();

    public CreateIncidentViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;
        CreateIncidentCommand = new Command(async () => await CreateIncident());
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
    
    public ICommand CreateIncidentCommand { get; }
    public async Task CreateIncident()
    {
        Error = string.Empty;

        if (!_validationHelper.IsNotBlank(Name))
        {
            Error = "Name is required.";
            return;
        }

        try
        {
            await _incidentApi.CreateIncident(new CreateIncidentRequest
            {
                Name = Name,
                Description = Description
            });
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
}