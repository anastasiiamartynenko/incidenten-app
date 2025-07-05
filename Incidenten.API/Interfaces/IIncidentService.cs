using Incidenten.Domain;
using Incidenten.Domain.Enums;

namespace Incidenten.API.Interfaces;

public interface IIncidentService
{
    Task<Incident?> CreateIncident(string name, string description, User user);
    
    Task<Incident?> GetIncident(Guid incidentId);
    Task<Incident?> GetIncidentPopulated(Guid incidentId);
    Task<Incident?> GetIncidentPopulatedImagesAndLocation(Guid incidentId);
    Task<bool> DoesHaveUdPermissions(User? user, Guid incidentId);
    Task UpdateIncident(Guid id, string? name, string? description);
    Task DeleteIncident(Incident incident);
    Task<List<Incident>> GetMyIncidents(User user);
    Task<List<Incident>> GetMyAssignedIncidents(User user);
    Task<List<Incident>> GetFilteredIncidents(User user, IncidentStatus? status, IncidentPriority? priority);
    Dictionary<string, string> GetIncidentDictionary(Incident incident);
    DateTime GetDeadline(IncidentPriority priority);
}