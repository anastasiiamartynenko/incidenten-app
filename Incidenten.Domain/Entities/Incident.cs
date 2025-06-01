using Incidenten.Domain.Enums;

namespace Incidenten.Domain;

public class Incident : _Base
{
    public DateTime? DeadlineAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public IncidentStatus Status { get; set; } = IncidentStatus.Open;
    public IncidentPriority Priority { get; set; } = IncidentPriority.Low;
    
    public Guid ReporterId { get; set; }
    public User Reporter { get; set; } = null!;

    public Guid? ExecutorId { get; set; }
    public User? Executor { get; set; }
}