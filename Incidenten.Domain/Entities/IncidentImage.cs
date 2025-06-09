using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Incidenten.Domain;

public class IncidentImage : _Base
{
    public string Filename { get; set; } = "NO_IMAGE_PLACEHOLDER.png";
 
    public Guid IncidentId { get; set; } = Guid.NewGuid();
    [JsonIgnore]
    public Incident Incident { get; set; } = new();

    [NotMapped] public string ImageUrl => $"http://localhost:5000/images/{Filename}";

    [NotMapped]
    public string ImageUrlAndroid => $"http://10.0.2.2:5000/images/{Filename}";
}