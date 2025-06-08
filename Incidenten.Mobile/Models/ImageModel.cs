namespace Incidenten.Mobile.Models;

public class ImageModel
{
    public string Name { get; set; } = string.Empty;
    public ImageSource Source { get; set; }

    public Func<Task<Stream>> GetStream { get; set; } = default!;
}