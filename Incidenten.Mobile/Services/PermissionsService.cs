namespace Incidenten.Mobile.Services;

public class PermissionsService
{
    /**
     * Check and request camera permissions.
     */
    public async Task CheckAndRequestCameraPermission()
    {
        var isCameraPermissionGranted = await IsCameraPermissionGranted();
        if (!isCameraPermissionGranted)
            await Permissions.RequestAsync<Permissions.Camera>();
    }

    /**
     * Check camera permission.
     */
    public async Task<bool> IsCameraPermissionGranted()
    {
        return await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted;
    }
}