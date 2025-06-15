using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Mobile.Interfaces;
using Incidenten.Mobile.Models;
using Incidenten.Mobile.Services;
using Incidenten.Mobile.ViewModels.Handlers;
using Incidenten.Shared.Api;
using Incidenten.Shared.Utils;

namespace Incidenten.Mobile.ViewModels;

public class BaseIncidentFormViewModel : _BaseViewModel, IMapAware
{
    private readonly IIncidentApi _incidentApi;
    protected readonly ImageHandler _imageHandler;
    protected readonly LocationHandler _locationHandler;
    private readonly ValidationHelper _validationHelper = new ();
    
    public BaseIncidentFormViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;
        _imageHandler = new ImageHandler();
        _locationHandler = new LocationHandler();
        
        UploadFromGalleryCommand = new Command(async () => await UploadFromGallery());
        UploadFromCameraCommand = new Command(async () => await UploadFromCamera());
        MapClickedCommand = new Command<Location>(_locationHandler.UpdateLocation);
        
        _locationHandler.LocationChanged += loc => LocationChanged?.Invoke(loc);
        Task.Run(() => _locationHandler.InitializeLocation());
    }
    
    /* Fields */
    // Name
    private string _name = string.Empty;
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    // Description
    private string _description = string.Empty;
    public string Description { get => _description; set => SetProperty(ref _description, value); }
    // Images
    public ObservableCollection<ImageModel> Images => _imageHandler.Images;
    // Latitude
    public double Latitude
    {
        get => _locationHandler.Latitude;
        set => _locationHandler.Latitude = value;
    }
    // Longitude
    public double Longitude
    {
        get => _locationHandler.Longitude;
        set => _locationHandler.Longitude = value;
    }
    
    /* Methods */
    /**
     * Exceptions-safe upload from gallery method.
     */
    private async Task UploadFromGallery()
    {
        try
        {
            await _imageHandler.UploadFromGallery();
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    /**
     * Exceptions-safe upload from camera method.
     */
    private async Task UploadFromCamera()
    {
        try
        {
            await _imageHandler.UploadFromCamera();
        }
        catch (Exception ex)
        {
            Error = "An error occurred: " + ex.Message;
        }
    }
    
    /* Events */
    public event Action<Location?>? LocationChanged;
    
    /* Commands */
    public ICommand UploadFromGalleryCommand { get; }
    public ICommand UploadFromCameraCommand { get; }
    public ICommand MapClickedCommand { get; }
}