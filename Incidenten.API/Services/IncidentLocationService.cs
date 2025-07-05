using Incidenten.API.Interfaces;
using Incidenten.Domain;
using Incidenten.Infrastructures;

namespace Incidenten.API.Services;

public class IncidentLocationService : IIncidentLocationService
{
    private readonly IncidentenDbContext _db;

    public IncidentLocationService(IncidentenDbContext db)
    {
        _db = db;
    }
    
    private void CreateLocation(Incident incident, double latitude, double longitude)
    {
        var newLocation = new IncidentLocation
        {
            Latitude = latitude,
            Longitude = longitude,
            Incident = incident,
        };
        _db.IncidentLocations.Add(newLocation);
    }

    private Incident UpdateLocation(Incident incident, double latitude, double longitude)
    {
        if (incident.Location != null)
        {
            incident.Location.Latitude = latitude;
            incident.Location!.Longitude = longitude;
            _db.IncidentLocations.Update(incident.Location);
        }
        return incident;
    }

    public async Task CreateOrUpdateLocation(Incident incident, double latitude, double longitude)
    {
        // Create the new location or update the existing one.
        if (incident.Location == null)
        {
            // Create the new location in case no location is bound to the incident.
            CreateLocation(incident, latitude, longitude);
        }
        else
        {
            // Otherwise, update the existing location of the incident.
            incident = UpdateLocation(incident, latitude, longitude);
        }
        
        // Persist the changes in the DB and return nothing.
        await _db.SaveChangesAsync();
    }
}