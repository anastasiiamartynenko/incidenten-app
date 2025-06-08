using Incidenten.Domain;
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
            var newLocation = new IncidentLocation
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Incident = incident,
            };
            db.IncidentLocations.Add(newLocation);
        }
        else
        {
            // Otherwise, update the existing location of the incident.
            incident.Location.Latitude = request.Latitude;
            incident.Location.Longitude = request.Longitude;
            db.IncidentLocations.Update(incident.Location);
        }
        
        // Persist the changes in the DB and return nothing.
        await db.SaveChangesAsync();
        return Ok();
    }
}