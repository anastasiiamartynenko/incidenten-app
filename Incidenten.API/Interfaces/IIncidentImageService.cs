using Incidenten.Domain;

namespace Incidenten.API.Interfaces;

public interface IIncidentImageService
{
    Task<IncidentImage> CreateImage(Incident incident, IFormFile image);
    Task CreateImages(Incident incident, IFormFile[] images);
}