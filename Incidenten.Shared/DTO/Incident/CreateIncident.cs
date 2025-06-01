namespace Incidenten.Shared.DTO.Incident;

public class CreateIncidentRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
