using Incidenten.Domain;
using Incidenten.Shared.DTO.Incident;
using Refit;

namespace Incidenten.Shared.Api;

public interface IIncidentApi
{
    [Post("/incident")]
    Task<Incident> CreateIncident([Body] CreateIncidentRequest incident);
}