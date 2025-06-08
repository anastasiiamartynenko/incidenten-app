using Incidenten.Domain;

namespace Incidenten.Mobile.Services;

// TODO: might be helpful for the future use.
//       If it is not, then remove. 
public class LocationService
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isCheckingLocation;
    
    /**
     * Get the current location of the user.
     */
    public async Task<IncidentLocation?> GetCurrentLocation()
    {
        _isCheckingLocation = true;
        GeolocationRequest request = new GeolocationRequest(
            GeolocationAccuracy.Medium, 
            TimeSpan.FromSeconds(10)
        );
        _cancellationTokenSource = new CancellationTokenSource();
        var location = await Geolocation.Default.GetLocationAsync(request, _cancellationTokenSource.Token);
        if (location != null) return new IncidentLocation
        {
            Latitude = location.Latitude, 
            Longitude = location.Longitude
        };
        _isCheckingLocation = false;
        return null;
    }

    /**
     * Get the last known location of the user.
     */
    public async Task<IncidentLocation?> GetLastKnownLocation()
    {
        _isCheckingLocation = true;
        Location? location = await Geolocation.Default.GetLastKnownLocationAsync();
        if (location == null) return null;

        return new IncidentLocation { Latitude = location.Latitude, Longitude = location.Longitude };
    }

    /**
     * Cancel the location request.
     */
    public void CancelRequest()
    {
        if (_isCheckingLocation && _cancellationTokenSource is { IsCancellationRequested: false })
        {
            _cancellationTokenSource.Cancel();
        }
    }

    /**
     * Calculate the distance between 2 locations in km.
     */
    public double CalculateDistance(IncidentLocation location1Raw, IncidentLocation location2Raw)
    {
        Location location1 = new Location(location1Raw.Latitude, location1Raw.Longitude);
        Location location2 = new Location(location2Raw.Latitude, location2Raw.Longitude);
        return Location.CalculateDistance(location1, location2, DistanceUnits.Kilometers);
    }
}