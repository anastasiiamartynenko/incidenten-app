using System.Collections.ObjectModel;
using Incidenten.Mobile.Models;
using Incidenten.Mobile.Services;
using Refit;

namespace Incidenten.Mobile.ViewModels.Handlers;

public class ImageHandler
{
    public ObservableCollection<ImageModel> Images { get; set; } = new ();

    public void SetImages(IEnumerable<ImageModel> images)
    {
        Images.Clear();
        foreach (var image in images)
        {
            Images.Add(image);
        }
    }
    /**
     * Upload the images from the gallery.
     */
    public async Task UploadFromGallery()
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
            var processedImage = await ImageHelper.ProcessImage(result);
            if (processedImage != null) Images.Add(processedImage);
        }
    }
    
    /*
     * Upload images from camera.
     */
    public async Task UploadFromCamera()
    {
        // Request the permissions if needed first.
        await PermissionsService.CheckAndRequestCameraPermission();
            
        // Make sure the camera is supported on the user's device.
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            throw new Exception("The capture is not supported on this device :<");
        }

        // Make sure the camera permission is granted.
        var isCameraPermissionGranted = await PermissionsService.IsCameraPermissionGranted();
        if (!isCameraPermissionGranted)
        {
            throw new Exception("Camera permission is not granted on this device :<");
        }

        // Launch the MediaPicker to allow user to take a photo.
        var photo = await MediaPicker.Default.CapturePhotoAsync();
        // Process an image and add it to the images array.
        var processedImage = await ImageHelper.ProcessImage(photo);
        if (processedImage != null) Images.Add(processedImage);    
    }

    /**
     * Convert the images to stream parts in order to attach them to the multipart form request.
     */
    public async Task<List<StreamPart>> ToStreamParts()
    {
        var parts = new List<StreamPart>();
     
        foreach (var img in Images)
        {
            // The image source might be either the actual stream, or the URL of the image from the backend.
            // The try-catch block is essential to convert to stream parts only those images
            // that are not yet stored in the DB.
            try
            {
                // Try to get the image stream.
                var stream = await img.GetStream();
                // Get the image mimetype.
                var mime = ImageHelper.GetMimetype(img.Name);
                // Add the image stream part to the parts.
                parts.Add(new StreamPart(stream, img.Name, mime));
            }
            catch (Exception ex)
            {
                // Do nothing - it is image defined by URL.
            }
        }
        
        // Return the result.
        return parts;
    }
}