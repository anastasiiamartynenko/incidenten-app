using Incidenten.Mobile.Models;

namespace Incidenten.Mobile.Services;

public static class ImageHelper
{
    /**
     * Get the image mimetype.
     */
    public static string GetMimetype(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".bmp" => "image/bmp",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    /**
     * Convert the file result into the image model.
     */
    public static async Task<ImageModel?> ProcessImage(FileResult? fileResult)
    {
        if (fileResult == null) return null;
        
        // Get the stream from the file and copy it to the memory.
        var stream = await fileResult.OpenReadAsync();
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;
        
        // Get the image from the memory.
        var image = ImageSource.FromStream(() => memory);

        // Return the model created from the stream.
        return new ImageModel
        {
            Name = fileResult.FileName,
            Source = image,
            GetStream = () => Task.FromResult<Stream>(new MemoryStream(memory.ToArray()))
        };
    }
}