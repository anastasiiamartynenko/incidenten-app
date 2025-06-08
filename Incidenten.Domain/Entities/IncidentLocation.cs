using System.Text.Json.Serialization;

namespace Incidenten.Domain;

public class IncidentLocation : _Base
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public Guid IncidentId { get; set; } = Guid.NewGuid();
    [JsonIgnore]
    public Incident Incident { get; set; } = new();
}