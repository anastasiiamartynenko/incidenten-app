using Incidenten.API.Helpers;
using Incidenten.API.Interfaces;
using Incidenten.Domain;
using Incidenten.Infrastructures;

namespace Incidenten.API.Services;

public class IncidentImageService : IIncidentImageService
{
    private readonly IncidentenDbContext _db;
    private readonly string _imagesDir;

    public IncidentImageService(IncidentenDbContext db, IConfiguration configuration)
    {
        _db = db;
        _imagesDir = configuration["ImagesDir"] ?? "Uploads";
    }
    
    public async Task<IncidentImage> CreateImage(Incident incident, IFormFile image)
    {
        var imageId = Guid.NewGuid();
        var filename = await ImagesHelper.SaveImageOnDisk(_imagesDir, imageId, image);

        var newImage = new IncidentImage
        {
            Id = imageId,
            Filename = filename,
            Incident = incident,
        };
        _db.IncidentImages.Add(newImage);
        return newImage;
    }

    public async Task CreateImages(Incident incident, IFormFile[] images)
    {
        foreach (var image in images)
        {
            await CreateImage(incident, image);
        }

        // Save all changes to DB.
        await _db.SaveChangesAsync();
    }
}