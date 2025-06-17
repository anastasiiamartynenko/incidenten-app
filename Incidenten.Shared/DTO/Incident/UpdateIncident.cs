using Incidenten.Domain.Enums;

namespace Incidenten.Shared.DTO.Incident;

public class UpdateIncidentRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateIncidentStatusRequest
{
    public IncidentStatus Status { get; set; }
}