using System.Collections.ObjectModel;
using System.Windows.Input;
using Incidenten.Mobile.Models;
using Incidenten.Mobile.Services;
using Incidenten.Shared.Api;
using Incidenten.Shared.DTO.Incident;
using Incidenten.Shared.Utils;
using Refit;
using Exception = System.Exception;

namespace Incidenten.Mobile.ViewModels;

public class CreateIncidentViewModel : _BaseViewModel
{
    private readonly IIncidentApi _incidentApi;
    private readonly PermissionsService _permissionsService = new ();
    private readonly ValidationHelper _validationHelper = new ();
    private readonly ImageHelper _imageHelper = new ();

    public CreateIncidentViewModel(IIncidentApi incidentApi)
    {
        _incidentApi = incidentApi;
        CreateIncidentCommand = new Command(async () => await CreateIncident());
        UploadFromGalleryCommand = new Command(async () => await UploadFromGallery());
        UploadFromCameraCommand = new Command(async () => await UploadFromCamera());
    }
    
    // Name.
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
    
    // Description.
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
    
    // Images associated with an incident.
    public ObservableCollection<ImageModel> Images { get; set; } = new ();
    
    /**
     * Upload the images from the gallery.
     */
    public async Task UploadFromGallery()
    {
        try
        {
            // Launch the file picker with the corresponding options to get the images from the user's filesystem.
            var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Choose image(s)",
                FileTypes = FilePickerFileType.Images,
            });

            // Process the file picker results.
            foreach (var result in results)
            {
                // Process an image and add it to the images array.
                var processedImage = await _imageHelper.ProcessImage(result);
                if (processedImage != null) Images.Add(processedImage);
            }
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }

    /*
     * Upload images from camera.
     */
    public async Task UploadFromCamera()
    {
        try
        {
            // Request the permissions if needed first.
            await _permissionsService.CheckAndRequestCameraPermission();
            
            // Make sure the camera is supported on the user's device.
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                Error = "The capture is not supported on this device :<";
                return;
            }

            // Make sure the camera permission is granted.
            var isCameraPermissionGranted = await _permissionsService.IsCameraPermissionGranted();
            if (!isCameraPermissionGranted)
            {
                Error = "Camera permission is not granted on this device :<";
                return;
            }

            // Launch the MediaPicker to allow user to take a photo.
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            // Process an image and add it to the images array.
            var processedImage = await _imageHelper.ProcessImage(photo);
            if (processedImage != null) Images.Add(processedImage);
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
    
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

            // Assemble the images multipart stuff.
            var imageParts = new List<StreamPart>();
            foreach (var image in Images)
            {
                var stream = await image.GetStream();
                var mimetype = _imageHelper.GetMimetype(image.Name);
                imageParts.Add(new StreamPart(stream, image.Name, mimetype));
            }

            // Upload the images.
            await _incidentApi.UploadImages(incident.Id, imageParts);
            
            // Redirect the user to the main page.
            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            Error = "An error occurred:" + ex.Message;
        }
    }
    
    // Commands.
    public ICommand UploadFromGalleryCommand { get; }
    public ICommand UploadFromCameraCommand { get; }
    public ICommand CreateIncidentCommand { get; }
}