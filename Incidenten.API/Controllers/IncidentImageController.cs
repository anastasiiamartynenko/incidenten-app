using Incidenten.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentImageController(IncidentService incidentService, UserService userService, IncidentImageService incidentImageService) : Controller
{
    /**
     * Upload / delete images as an authorized user.
     */
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UploadOrDeleteImages(
        [FromForm] IFormFile[] images,
        Guid id
    )
    {
        try
        {
            // Make sure the user can update / delete images.
            var user = await userService.GetUserByEmail(User.Identity?.Name);
            var doesHaveUdPermissions = await incidentService.DoesHaveUdPermissions(user, id);
            if (!doesHaveUdPermissions) return Unauthorized();
            
            // Make sure the incident whose ID is provided exists.
            var incident = await incidentService.GetIncidentPopulated(id);
            if (incident == null) return NotFound();

            await incidentImageService.CreateImages(incident, images);
            return Ok();
        }
        catch (Exception e)
        {
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
    
    /**
     * Upload images associated with an incident.
     */
    [HttpPost("{id}")]
    public async Task<IActionResult> UploadImages([FromForm] IFormFile[] images, Guid id)
    {
        // Make sure the incident whose ID is provided exists.
        var incident = await incidentService.GetIncidentPopulated(id);
        if (incident == null) return NotFound();

        await incidentImageService.CreateImages(incident, images);
        return Ok();
    }
}