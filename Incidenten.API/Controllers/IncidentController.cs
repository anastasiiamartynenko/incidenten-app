using Incidenten.Domain;
using Incidenten.Infrastructures;
using Incidenten.Shared.DTO.Incident;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentController(IncidentenDbContext db, IConfiguration configuration) : Controller
{
    /**
     * Create a new incident.
     */
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncidentRequest request)
    {
        var email = User.Identity?.Name;
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // If the user is not attached to the request, try to create an incident with ANONYM reporter.
        if (user == null)
        {
            var anonymId = configuration["Utils:AnonymId"] 
                           ?? throw new Exception("Anonym ID is missing in the configuration file.");
            user = await db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(anonymId));
        }

        // Otherwise, the configuration file is configured incorrectly. Return an internal server error response.
        if (user == null)
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        // Create a new incident.
        var incident = new Incident
        {
            Name = request.Name,
            Description = request.Description,
            Reporter = user
        };

        // Persist the new incident in the DB and return it.
        db.Incidents.Add(incident);
        await db.SaveChangesAsync();
        
        return Ok(incident);
    }
}