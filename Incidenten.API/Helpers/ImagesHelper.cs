using Incidenten.Domain;

namespace Incidenten.API.Helpers;

public static class ImagesHelper
{
    public static void DeleteImages(string dir, List<IncidentImage> images)
    {
        // Get the image's directory.
        var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), dir);
        
        // Delete all the images attached to the incident from the corresponding Uploads directory.
        foreach (var image in images)
        {
            var filePath = Path.Combine(imagesDirectory, image.Filename);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
    
    private static string GetUploadsFolder(string dir)
    {
        // Get the folder where the images will be stored.
        var folder = Path.Combine(Directory.GetCurrentDirectory(), dir);
        
        // If the folder does not exist, create it.
        if (!Directory.Exists(folder)) 
            Directory.CreateDirectory(folder);

        return folder;
    }

    public static async Task<string> SaveImageOnDisk(string dir, Guid imageId, IFormFile image)
    {
        var folder = GetUploadsFolder(dir);
        
        // Assemble the filename and the path to the image. 
        var filename = $"{imageId}{Path.GetExtension(image.FileName)}";
        var path = Path.Combine(folder, filename);
            
        // Store the image on the disk.
        using var stream = new FileStream(path, FileMode.Create);
        await image.CopyToAsync(stream);

        return filename;
    }
}