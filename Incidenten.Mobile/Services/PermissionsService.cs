namespace Incidenten.Mobile.Services;

public static class PermissionsService
{
    /**
     * Check and request camera permissions.
     */
    public static async Task CheckAndRequestCameraPermission()
    {
        var isCameraPermissionGranted = await IsCameraPermissionGranted();
        if (!isCameraPermissionGranted)
            await Permissions.RequestAsync<Permissions.Camera>();
    }

    /**
     * Check and request location permissions.
     */
    public static async Task CheckAndRequestLocationPermission()
    {
        var isLocationPermissionGranted = await IsLocationPermissionGranted();
        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
        {
            // TODO: show rationale.
        }
        if (!isLocationPermissionGranted)
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }

    /**
     * Check camera permission.
     */
    public static async Task<bool> IsCameraPermissionGranted()
    {
        return await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Granted;
    }

    /**
     * Check location permissions.
     */
    public static async Task<bool> IsLocationPermissionGranted()
    {
        return await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted
            || await Permissions.CheckStatusAsync<Permissions.LocationAlways>() == PermissionStatus.Granted;
    }
}