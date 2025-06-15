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
     * A helper function to get the user by email.
     */
    private async Task<User?> GetUserByEmail(string? email)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    /**
     * Create a new incident.
     */
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncidentRequest request)
    {
        var user = await GetUserByEmail(User.Identity?.Name);

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
     * Get an incident by its ID.
     */
    [Authorize]
    [HttpGet("data/{id}")]
    public async Task<IActionResult> GetIncident(Guid? id)
    {
        // Get user's email and find the user in the DB.
        var user = await GetUserByEmail(User.Identity?.Name);

        // If no user was found, return the Unauthorized response.
        if (user == null) return Unauthorized();

        // Find the corresponding incident and populate all the relevant data.
        var incident = await db.Incidents
            .Include(i => i.Images)
            .Include(i => i.Location)
            .Include(i => i.Reporter)
            .Include(i => i.Executor)
            .FirstOrDefaultAsync(i => i.Id == id);
        
        // Make sure the incident exists.
        if (incident == null) return NotFound();
        
        // Make sure the user is allowed to access the incident data.
        if (user.Role == UserRole.Employee || user.Role == UserRole.Official || incident.ReporterId == user.Id)
        {
            return Ok(incident);
        }

        // In all the rest cases, return the Unauthorized response.
        return Unauthorized();
    }

    /**
     * Returns true if the user has the Update/Delete permissions regarding the specific incident.
     */
    private async Task<bool> DoesHaveUdPermissions(string? email, Guid incidentId)
    {
        // Get incident.
        var incident = await db.Incidents
            .FirstOrDefaultAsync(i => i.Id == incidentId);
        
        // Get user and make sure they exists.
        var user = await GetUserByEmail(email);
        if (user == null) return false;
        
        // Check whether the user is citizen and has reported the incident.
        var isCitizen = user.Role == UserRole.Citizen && incident?.ReporterId == user.Id;
        // Check whether the user is employee or official.
        var isEmployeeOrOfficial = user.Role == UserRole.Employee || user.Role == UserRole.Official;
        
        // Return true only if user has these properties.
        if (!isCitizen && !isEmployeeOrOfficial) return false;
        return true;
    }
 
    /**
     * Update the incident.
     */
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentRequest request)
    {
        // Make sure the user has the Update/Delete permissions regarding this incident.
        var doesHaveUdPermissions = await DoesHaveUdPermissions(User.Identity?.Name, id);
        var incident = await db.Incidents.FirstOrDefaultAsync(i => i.Id == id);
        
        if (incident == null) return NotFound();
        
        if (doesHaveUdPermissions)
        {
            // Update the incident properties.
            if (request.Name != null) incident.Name = request.Name;
            if (request.Description != null) incident.Description = request.Description;
            
            // Save the changes to the DB and return nothing.
            await db.SaveChangesAsync();
            return Ok();
        }
        
        // Throw unauthorized exception in all the rest cases.
        return Unauthorized();
    }

    /**
     * Delete the incident.
     */
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIncident(Guid id)
    {
        // Make sure the user has the Update/Delete permissions regarding this incident.
        var doesHaveUdPermissions = await DoesHaveUdPermissions(User.Identity?.Name, id);
        
        // Get the incident.
        var incident = await db.Incidents
            .Include(i => i.Images)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(i => i.Id == id);
        
        // Make sure the incident can be deleted.
        if (incident == null) return NotFound();
        if (!doesHaveUdPermissions) return Unauthorized();
        if (incident.Status != IncidentStatus.Open) 
            return BadRequest("An incident can only be deleted if it is opened.");
        
        var images = incident.Images;
        
        // Clear up the image records.
        foreach (var image in incident.Images)
        {
            db.IncidentImages.Remove(image);
        }
        // Delete the location record.
        if (incident.Location != null) db.IncidentLocations.Remove(incident.Location);
        
        // Delete the incident.
        db.Incidents.Remove(incident);
        // Persist the changes in the DB.
        await db.SaveChangesAsync();
        
        // Get the images directory.
        // TODO: code refactor.
        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        
        // Delete all the images attached to the incident from the corresponding Uploads directory.
        foreach (var image in images)
        {
            var filePath = Path.Combine(imagesDirectory, image.Filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        
        // Return nothing.
        return Ok();
    }

    /**
     * Get the incidents reported by the user.
     */
    [Authorize]
    [HttpGet("my/reported")]
    public async Task<IActionResult> GetMyReportedIncidents()
    {
        var user = await GetUserByEmail(User.Identity?.Name);

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
        var user = await GetUserByEmail(User.Identity?.Name);

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
        var user = await GetUserByEmail(User.Identity?.Name);

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