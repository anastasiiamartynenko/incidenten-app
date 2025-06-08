using System.Text.Json.Serialization;

namespace Incidenten.Domain;

public class IncidentImage : _Base
{
    public string Filename { get; set; } = string.Empty;
 
    public Guid IncidentId { get; set; } = Guid.NewGuid();
    [JsonIgnore]
    public Incident Incident { get; set; } = new();
}