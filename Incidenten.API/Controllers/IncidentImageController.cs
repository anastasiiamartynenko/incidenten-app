using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Infrastructures;
using Incidenten.Shared.DTO.Incident;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentImageController(IncidentenDbContext db) : Controller
{
    private async Task<User?> GetUserByEmail(string? email)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    // TODO: code refactor 
    private async Task<bool> DoesHaveUdPermissions(string? email, Guid incidentId)
    {
        var incident = await db.Incidents
            .FirstOrDefaultAsync(i => i.Id == incidentId);
        
        var user = await GetUserByEmail(email);
        if (user == null) return false;
        
        var isCitizen = user.Role == UserRole.Citizen && incident?.ReporterId == user.Id;
        var isEmployeeOrOfficial = user.Role == UserRole.Employee || user.Role == UserRole.Official;
        
        if (!isCitizen && !isEmployeeOrOfficial) return false;
        return true;
    }
    
    // TODO: move to service / helper
    private string GetUploadsFolder()
    {
        // Get the folder where the images will be stored.
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        
        // If the folder does not exist, create it.
        if (!Directory.Exists(folder)) 
            Directory.CreateDirectory(folder);

        return folder;
    }

    private async Task<IncidentImage> CreateImage(Incident incident, IFormFile image)
    {
        var folder = GetUploadsFolder();
        
        // Generate the ID for each image.
        var imageId = Guid.NewGuid();
            
        // Assemble the filename and the path to the image. 
        var filename = $"{imageId}{Path.GetExtension(image.FileName)}";
        var path = Path.Combine(folder, filename);
            
        // Store the image on the disk.
        using var stream = new FileStream(path, FileMode.Create);
        await image.CopyToAsync(stream);

        // Create a new incident image table record.
        var newImage = new IncidentImage
        {
            Id = imageId,
            Filename = filename,
            Incident = incident,
        };
        db.IncidentImages.Add(newImage);
        return newImage;
    }

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
            var doesHaveUdPermissions = await DoesHaveUdPermissions(User.Identity?.Name, id);
            if (!doesHaveUdPermissions) return Unauthorized();
            
            // Make sure the incident whose ID is provided exists.
            var incident = await db.Incidents
                .Include(i => i.Images)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (incident == null) return NotFound();

            // Upload the images.
            foreach (var image in images)
            {
                await CreateImage(incident, image);
            }

            // Save all changes to DB.
            await db.SaveChangesAsync();
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
        // Get the folder where the images will be stored.
        var folder = GetUploadsFolder();
        
        // Make sure the incident whose ID is provided exists.
        var incident = await db.Incidents.FindAsync(id);
        if (incident == null) return NotFound();

        // Process and store the images.
        foreach (var image in images)
        {
            await CreateImage(incident, image);
        }
        
        // Save the changes to the DB and return nothing.
        await db.SaveChangesAsync();
        return Ok();
    }
}