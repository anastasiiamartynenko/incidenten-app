using Incidenten.Domain;

namespace Incidenten.API.Interfaces;

public interface IIncidentLocationService
{
    Task CreateOrUpdateLocation(Incident incident, double latitude, double longitude);
}