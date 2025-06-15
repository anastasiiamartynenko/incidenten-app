using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Infrastructures;
using Incidenten.Shared.DTO.Incident;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentLocationController(IncidentenDbContext db) : Controller
{
    /**
     * Create new location record.
     */
    private void CreateLocation(Incident incident, double latitude, double longitude)
    {
        var newLocation = new IncidentLocation
        {
            Latitude = latitude,
            Longitude = longitude,
            Incident = incident,
        };
        db.IncidentLocations.Add(newLocation);
    }

    /**
     * Update the existing location record.
     */
    private Incident UpdateLocation(Incident incident, double latitude, double longitude)
    {
        if (incident.Location != null)
        {
            incident.Location.Latitude = latitude;
            incident.Location!.Longitude = longitude;
            db.IncidentLocations.Update(incident.Location);
        }
        
        return incident;
    }
    
    /**
     * Provide the incident location.
     */
    [HttpPost("{id}")]
    public async Task<IActionResult> ProvideIncidentLocation(Guid id, [FromBody] ProvideIncidentLocationRequest request)
    {
        // Make sure the provided incident exists.
        var incident = await db.Incidents
            .Include(i => i.Location)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
        if (incident == null) return NotFound();

        // Create the new location or update the existing one.
        if (incident.Location == null)
        {
            // Create the new location in case no location is bound to the incident.
            CreateLocation(incident, request.Latitude, request.Longitude);
        }
        else
        {
            // Otherwise, update the existing location of the incident.
            incident = UpdateLocation(incident, request.Latitude, request.Longitude);
        }
        
        // Persist the changes in the DB and return nothing.
        await db.SaveChangesAsync();
        return Ok();
    }
}