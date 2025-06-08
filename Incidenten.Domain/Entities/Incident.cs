using System.ComponentModel.DataAnnotations.Schema;
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
    
    public List<IncidentImage> Images { get; set; } = new();
    public IncidentLocation? Location { get; set; } = null!;

    [NotMapped] public string FirstImageUrl => Images.FirstOrDefault() is { Filename: var name }
        ? $"http://localhost:5000/images/{Images.FirstOrDefault()?.Filename}"
        : "http://localhost:5000/images/NO_IMAGE_PLACEHOLDER.png";
    
    [NotMapped] public string FirstImageUrlAndroid => Images.FirstOrDefault() is { Filename: var name }
        ? $"http://10.0.2.2:5000/images/{Images.FirstOrDefault()?.Filename}"
        : "http://10.0.2.2:5000/images/NO_IMAGE_PLACEHOLDER.png";
}