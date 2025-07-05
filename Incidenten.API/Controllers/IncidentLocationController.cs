using Incidenten.API.Services;
using Incidenten.Shared.DTO.Incident;
using Microsoft.AspNetCore.Mvc;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentLocationController(IncidentService incidentService, IncidentLocationService incidentLocationService) : Controller
{
    /**
     * Provide the incident location.
     */
    [HttpPost("{id}")]
    public async Task<IActionResult> ProvideIncidentLocation(Guid id, [FromBody] ProvideIncidentLocationRequest request)
    {
        // Make sure the provided incident exists.
        var incident = await incidentService.GetIncidentPopulated(id);
        if (incident == null) return NotFound();

        await incidentLocationService.CreateOrUpdateLocation(incident, request.Latitude, request.Longitude);
        return Ok();
    }
}