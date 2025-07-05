using Incidenten.API.Services;
using Incidenten.Domain.Enums;
using Incidenten.Shared.DTO.Incident;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentController(IncidentService incidentService, UserService userService, EmailService emailService) : Controller
{
    /**
     * Create a new incident.
     */
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIncidentRequest request)
    {
        // If the user is not attached to the request, try to create an incident with ANONYM reporter.
        var user = await userService.GetUserByEmail(User.Identity?.Name) ?? await userService.GetAnonymUser();

        // Otherwise, the configuration file is configured incorrectly. Return an internal server error response.
        if (user == null)
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);

        // Create a new incident.
        var incident = await incidentService.CreateIncident(request.Name, request.Description, user);
        
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
        var user = await userService.GetUserByEmail(User.Identity?.Name);

        // If no user was found, return the Unauthorized response.
        if (user == null) return Unauthorized();
        if (id == null) return BadRequest();

        // Find the corresponding incident and populate all the relevant data.
        var incident = await incidentService.GetIncidentPopulated((Guid) id);
        
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
     * Update the incident.
     */
    [Authorize]
    [HttpPut("upd/{id}")]
    public async Task<IActionResult> UpdateIncident(Guid id, [FromBody] UpdateIncidentRequest request)
    {
        // Make sure the user has the Update/Delete permissions regarding this incident.
        var user = await userService.GetUserByEmail(User.Identity?.Name);
        var doesHaveUdPermissions = await incidentService.DoesHaveUdPermissions(user, id);
        
        if (doesHaveUdPermissions)
        {
            await incidentService.UpdateIncident(id, request.Name, request.Description);
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
        var user = await userService.GetUserByEmail(User.Identity?.Name);
        var doesHaveUdPermissions = await incidentService.DoesHaveUdPermissions(user, id);
        
        // Get the incident.
        var incident = await incidentService.GetIncidentPopulatedImagesAndLocation(id);
        
        // Make sure the incident can be deleted.
        if (incident == null) return NotFound();
        if (!doesHaveUdPermissions) return Unauthorized();
        if (incident.Status != IncidentStatus.Open) 
            return BadRequest("An incident can only be deleted if it is opened.");
        
        // Delete the incident.
        await incidentService.DeleteIncident(incident);
        
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
        var user = await userService.GetUserByEmail(User.Identity?.Name);

        // The endpoint should only be available for the authorized users.
        if (user == null || user.Role == UserRole.Anonym) return Unauthorized();
        
        // Return the incidents created by the user who initiated the request.
        var incidents = await incidentService.GetMyIncidents(user);
        return Ok(incidents);
    }

    /**
     * Get the incidents assigned to the user.
     */
    [HttpGet("my/assigned")]
    public async Task<IActionResult> GetMyAssignedIncidents()
    {
        var user = await userService.GetUserByEmail(User.Identity?.Name);

        // The endpoint should only be available for the employees.
        if (user is not { Role: UserRole.Employee }) return Unauthorized();
        
        // Return the incidents whose executor is the user who initiated the request.
        var incidents = await incidentService.GetMyAssignedIncidents(user);
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
        var user = await userService.GetUserByEmail(User.Identity?.Name);

        // The endpoint should only be available for the employees and officials.
        if (user is not { Role: UserRole.Employee } && user is not { Role: UserRole.Official }) 
            return Unauthorized();
        
        // Return the filtered incidents.
        var incidents = await incidentService.GetFilteredIncidents(user, status, priority);
        return Ok(incidents);
    }
    
    /**
     * Update the incident status.
     */
    [Authorize]
    [HttpPut("status/{id}")]
    public async Task<IActionResult> UpdateIncidentStatus(Guid id, [FromBody] UpdateIncidentStatusRequest request)
    {
        // Make sure the user exists.
        var user = await userService.GetUserByEmail(User.Identity?.Name);
        if (user == null) return Unauthorized();
        
        // Make sure the incident exists.
        var incident = await incidentService.GetIncidentPopulated(id);
        if (incident == null) return NotFound();

        /*
         * Status flow:
         * OPEN => REGISTERED => IN_PROGRESS => COMPLETED
         * ----------------------------------------------
         * Permissions:
         * Open => Registered: Official Y
         * Registered => Open: Official Y
         * Registered => InProgress: Employee Y
         * InProgress => Registered: Employee Y
         * InProgress => Completed: Employee
         * ----------------------------------------------
         * Only sequential status updates are possible.
         */
        switch (request.Status)
        {
            case IncidentStatus.Open:
                if (incident.Status != IncidentStatus.Registered) return BadRequest();
                if (user.Role != UserRole.Official) return Unauthorized(); 
                break;
            case IncidentStatus.Registered:
                if (user.Role == UserRole.Official)
                {
                    if (incident.Status != IncidentStatus.Open) return BadRequest();
                } else if (user.Role == UserRole.Employee)
                {
                    if (incident.Status != IncidentStatus.Registered) return BadRequest();
                }
                else
                {
                    return Unauthorized();
                }
                break;
            case IncidentStatus.InProgress:
                if (incident.Status != IncidentStatus.Registered) return BadRequest();
                if (user.Role != UserRole.Employee) return Unauthorized();
                break;
            case IncidentStatus.Completed:
                if (incident.Status != IncidentStatus.InProgress) return BadRequest();
                if (user.Role != UserRole.Employee) return Unauthorized();
                break;
        }
        
        // Update incident status if all the previous steps are successfully performed.
        await incidentService.UpdateIncidentStatus(incident, user, request.Status, request.Priority);

        // If the reporter is signed up to the notifications, make sure to send a notification to them.
        if (incident.Reporter.SendNotifications)
        {
            // Get the email template for the update status email.
            var emailTemplate = await emailService.GetUpdateStatusTemplate();
            if (emailTemplate == null) return NotFound();

            // Send the email to the incident reporter.
            emailService.SendMail(incident.Reporter.Email, emailTemplate, incidentService.GetIncidentDictionary(incident));
        }
        
        return Ok();
    }
}