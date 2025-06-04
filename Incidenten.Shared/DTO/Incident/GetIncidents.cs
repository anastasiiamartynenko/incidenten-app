using Incidenten.Domain.Enums;

namespace Incidenten.Shared.DTO.Incident;

public class GetIncidentsFilter
{
    public IncidentStatus? Status { get; set; }
    public IncidentPriority? Priority { get; set; }
}