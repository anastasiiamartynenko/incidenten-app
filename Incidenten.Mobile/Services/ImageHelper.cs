using Incidenten.Mobile.Models;

namespace Incidenten.Mobile.Services;

public class ImageHelper
{
    public string GetMimetype(string fileName)
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

    public async Task<ImageModel?> ProcessImage(FileResult? fileResult)
    {
        if (fileResult == null) return null;
        
        var stream = await fileResult.OpenReadAsync();
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;
        
        var image = ImageSource.FromStream(() => memory);

        return new ImageModel
        {
            Name = fileResult.FileName,
            Source = image,
            GetStream = () => Task.FromResult<Stream>(new MemoryStream(memory.ToArray()))
        };
    }
}