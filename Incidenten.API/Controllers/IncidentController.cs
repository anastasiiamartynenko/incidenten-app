using Incidenten.Domain;
using Incidenten.Domain.Enums;
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

    /**
     * Get the incidents reported by the user.
     */
    [Authorize]
    [HttpGet("my/reported")]
    public async Task<IActionResult> GetMyReportedIncidents()
    {
        var email = User.Identity?.Name;
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // The endpoint should only be available for the authorized users.
        if (user == null || user.Role == UserRole.Anonym) return Unauthorized();
        
        // Return the incidents created by the user who initiated the request.
        var incidents = await db.Incidents
            .Include(i => i.Reporter)
            .Include(i => i.Images)
            .Where(i => i.ReporterId == user.Id)
            .ToListAsync();
        return Ok(incidents);
    }

    /**
     * Get the incidents assigned to the user.
     */
    [HttpGet("my/assigned")]
    public async Task<IActionResult> GetMyAssignedIncidents()
    {
        var email = User.Identity?.Name;
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // The endpoint should only be available for the employees.
        if (user is not { Role: UserRole.Employee }) return Unauthorized();
        
        // Return the incidents whose executor is the user who initiated the request.
        var incidents = await db.Incidents
            .Include(i => i.Executor)
            .Where(i => i.ExecutorId == user.Id)
            .ToListAsync();
        return Ok(incidents);
    }

    /**
     * Get all the incidents (filtered).
     */
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetFilteredIncidents(
        [FromQuery] IncidentStatus? status,
        [FromQuery] IncidentPriority? priority)
    {
        var email = User.Identity?.Name;
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

        // The endpoint should only be available for the employees and officials.
        if (user is not { Role: UserRole.Employee } && user is not { Role: UserRole.Official }) 
            return Unauthorized();
        
        // Create the query.
        var query = db.Incidents.AsQueryable();
        
        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status);
        }

        if (priority.HasValue)
        {
            query = query.Where(i => i.Priority == priority);
        }
        
        // Return the filtered incidents.
        var incidents = await query.ToListAsync();
        return Ok(incidents);
    }
}