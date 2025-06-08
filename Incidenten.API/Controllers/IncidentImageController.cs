using Incidenten.Domain;
using Incidenten.Infrastructures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.API.Controllers;

[ApiController]
[Route("[controller]")]
public class IncidentImageController(IncidentenDbContext db) : Controller
{
    /**
     * Upload images associated with an incident.
     */
    [HttpPost("{id}")]
    public async Task<IActionResult> UploadImages([FromForm] IFormFile[] images, Guid id)
    {
        // Get the folder where the images will be stored.
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        
        // If the folder does not exist, create it.
        if (!Directory.Exists(folder)) 
            Directory.CreateDirectory(folder);
        
        // Make sure the incident whose ID is provided exists.
        var incident = await db.Incidents.FindAsync(id);
        if (incident == null) return NotFound();

        // Process and store the images.
        foreach (var image in images)
        {
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
        }
        
        // Save the changes to the DB and return nothing.
        await db.SaveChangesAsync();
        return Ok();
    }
}