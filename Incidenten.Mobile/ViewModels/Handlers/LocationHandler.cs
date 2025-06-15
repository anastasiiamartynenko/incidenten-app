using Incidenten.Mobile.Services;

namespace Incidenten.Mobile.ViewModels.Handlers;

public class LocationHandler
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public event Action<Location>? LocationChanged;

    public void SetLocation(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        LocationChanged?.Invoke(new Location(latitude, longitude));
    }

    /**
     * Initialize the location.
     */
    public async Task InitializeLocation()
    {
        await PermissionsService.CheckAndRequestLocationPermission();
        var isLocationPermissionGranted = await PermissionsService.IsLocationPermissionGranted();
            
        if (isLocationPermissionGranted)
        {
            var currentUserLocation = await LocationService.GetCurrentLocation();
            if (currentUserLocation == null)
            {
                var lastKnownLocation = await LocationService.GetLastKnownLocation();
                if (lastKnownLocation != null)
                {
                    Latitude = lastKnownLocation.Latitude;
                    Longitude = lastKnownLocation.Longitude;
                }
            }
            else
            {
                Latitude = currentUserLocation.Latitude;
                Longitude = currentUserLocation.Longitude;
            }
        }
        LocationChanged.Invoke(new Location { Latitude = Latitude, Longitude = Longitude });
    }

    public void UpdateLocation(Location location)
    {
        Latitude = location.Latitude;
        Longitude = location.Longitude;

        LocationChanged?.Invoke(location);
    }
}