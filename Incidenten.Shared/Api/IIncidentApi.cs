using Incidenten.Domain;
using Incidenten.Shared.DTO.Incident;
using Refit;

namespace Incidenten.Shared.Api;

public interface IIncidentApi
{
    [Post("/incident")]
    Task<Incident> CreateIncident([Body] CreateIncidentRequest incident);
    
    [Get("/incident/data/{id}")]
    Task<Incident?> GetIncident(Guid id);
    
    [Put("/incident/{id}")]
    Task UpdateIncident(Guid id, [Body] UpdateIncidentRequest request);
    
    [Delete("/incident/{id}")]
    Task DeleteIncident(Guid id);

    [Get("/incident/my/reported")]
    Task<List<Incident>> GetMyReportedIncidents();
    
    [Get("/incident/my/assigned")]
    Task<List<Incident>> GetMyAssignedIncidents();
    
    [Get("/incident/all")]
    Task<List<Incident>> GetAllIncidents([Query] GetIncidentsFilter filter);

    [Multipart]
    [Post("/incidentImage/{id}")]
    Task UploadImages(
        Guid id,
        [AliasAs("images")] IEnumerable<StreamPart> images);

    [Multipart]
    [Put("/incidentImage/{id}")]
    Task UpdateImages(
        Guid id,
        [AliasAs("images")] IEnumerable<StreamPart> images);
    
    [Post("/incidentLocation/{id}")]
    Task ProvideIncidentLocation(
        Guid id,
        [Body] ProvideIncidentLocationRequest incidentLocation);
}