using Incidenten.Shared.DTO.Incident;
using Refit;

namespace Incidenten.Shared.Api;

public interface IIncidentStatusApi
{
    [Put("/incident/status/{id}")]
    Task UpdateIncidentStatus(Guid id, [Body] UpdateIncidentStatusRequest request);
}