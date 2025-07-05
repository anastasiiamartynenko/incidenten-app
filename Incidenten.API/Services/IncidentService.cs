using Incidenten.API.Helpers;
using Incidenten.API.Interfaces;
using Incidenten.Domain;
using Incidenten.Domain.Enums;
using Incidenten.Infrastructures;
using Microsoft.EntityFrameworkCore;

namespace Incidenten.API.Services;

public class IncidentService : IIncidentService
{
    private readonly IncidentenDbContext _db;
    private readonly string _imagesDir;
    

    public IncidentService(IncidentenDbContext db, IConfiguration configuration)
    {
        _db = db;
        _imagesDir = configuration["ImagesDir"] ?? "Uploads";
    }
    
    /**
     * Create an incident and persist it to DB.
     */
    public async Task<Incident?> CreateIncident(string name, string? description, User user)
    {
        // Create a new incident.
        var incident = new Incident
        {
            Name = name,
            Description = description,
            Reporter = user
        };

        // Persist the new incident in the DB and return it.
        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync();
        return incident;
    }

    /**
     * Retrieves an incident from the DB by ID.
     */
    public async Task<Incident?> GetIncident(Guid incidentId)
    {
        return await _db.Incidents.FirstOrDefaultAsync(i => i.Id == incidentId);
    }

    /**
     * Retrieves a populated incident from the DB by ID.
     */
    public async Task<Incident?> GetIncidentPopulated(Guid incidentId)
    {
        var incident = await _db.Incidents
            .Include(i => i.Images)
            .Include(i => i.Location)
            .Include(i => i.Reporter)
            .Include(i => i.Executor)
            .FirstOrDefaultAsync(i => i.Id == incidentId);
        return incident;
    }
    
    /**
     * Retrieves a populated incident from the DB by ID.
     */
    public async Task<Incident?> GetIncidentPopulatedImagesAndLocation(Guid incidentId)
    {
        var incident = await _db.Incidents
            .Include(i => i.Images)
            .Include(i => i.Location)
            .FirstOrDefaultAsync(i => i.Id == incidentId);
        return incident;
    }
    
    /**
     * Returns true if the user has the Update/Delete permissions regarding the specific incident.
     */
    public async Task<bool> DoesHaveUdPermissions(User? user, Guid incidentId)
    {
        // Get incident.
        var incident = await GetIncident(incidentId);
        
        // Get user and make sure they exist.
        if (user == null) return false;
        
        // Check whether the user is citizen and has reported the incident.
        var isCitizen = user.Role == UserRole.Citizen && incident?.ReporterId == user.Id;
        // Check whether the user is employee or official.
        var isEmployeeOrOfficial = user.Role == UserRole.Employee || user.Role == UserRole.Official;
        
        // Return true only if user has these properties.
        if (!isCitizen && !isEmployeeOrOfficial) return false;
        return true;
    }

    /**
     * Update an incident.
     */
    public async Task UpdateIncident(Guid id, string? name, string? description)
    {
        var incident = await GetIncident(id);
        if (incident == null) return;
        
        incident.Name = name;
        incident.Description = description;
        
        await _db.SaveChangesAsync();
    }

    /**
     * Delete the incident and remove all the data associated with it.
     */
    public async Task DeleteIncident(Incident incident)
    {
        var images = incident.Images;
        
        // Clear up the image records.
        foreach (var image in incident.Images)
        {
            _db.IncidentImages.Remove(image);
        }
        // Delete the location record.
        if (incident.Location != null) _db.IncidentLocations.Remove(incident.Location);
        
        // Delete the incident.
        _db.Incidents.Remove(incident);
        // Persist the changes in the DB.
        await _db.SaveChangesAsync();
        
        ImagesHelper.DeleteImages(_imagesDir, images);
    }

    /**
     * Get incidents reported by the user.
     */
    public async Task<List<Incident>> GetMyIncidents(User user)
    {
        return await _db.Incidents
            .Include(i => i.Reporter)
            .Include(i => i.Images)
            .Where(i => i.ReporterId == user.Id)
            .ToListAsync();
    }

    /**
     * Get incidents assigned to the user.
     */
    public async Task<List<Incident>> GetMyAssignedIncidents(User user)
    {
        return await _db.Incidents
            .Include(i => i.Executor)
            .Where(i => i.ExecutorId == user.Id)
            .ToListAsync();
    }

    /**
     * Get incidents filtered by status or priority.
     */
    public async Task<List<Incident>> GetFilteredIncidents(User user, IncidentStatus? status, IncidentPriority? priority)
    {
        var query = _db.Incidents.Include(i => i.Location).AsQueryable();
        
        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status);
        }

        if (priority.HasValue)
        {
            query = query.Where(i => i.Priority == priority);
        }

        // For employee, return only the registered / picked up / completed tasks.
        if (user.Role == UserRole.Employee)
        {
            query = query.Where(i => 
                i.Status == IncidentStatus.Completed || 
                i.Status == IncidentStatus.Registered ||
                i.Status == IncidentStatus.InProgress
            );
        }
        
        // Return the filtered incidents.
        return await query.ToListAsync();
    }
    
    /**
     * Returns the dictionary with incident fields for the future use within the template generation.
     */
    public Dictionary<string, string> GetIncidentDictionary(Incident incident)
    {
        var dict = new Dictionary<string, string>();

        foreach (var prop in typeof(Incident).GetProperties())
        {
            var value = prop.GetValue(incident);
            if (value != null)
            {
                // Add kv pair to the dictionary.
                dict[prop.Name] = value.ToString() ?? string.Empty;
            }
        }
        
        return dict;
    }
    
    /**
     * Returns the deadline of the incident based on its priority.
     */
    public DateTime GetDeadline(IncidentPriority priority)
    {
        switch (priority)
        {
            case IncidentPriority.Low:
                return DateTime.UtcNow.AddDays(30);
            case IncidentPriority.Regular:
                return DateTime.UtcNow.AddDays(7);
            case IncidentPriority.High:
                return DateTime.UtcNow.AddDays(1);
        }
        throw new Exception("Unknown priority");
    }

    public async Task<Incident> UpdateIncidentStatus(Incident incident, User user, IncidentStatus? status, IncidentPriority? newPriority)
    {
        switch (status)
        {
            case IncidentStatus.Registered:
                if (user.Role == UserRole.Official)
                {
                    var priority = newPriority ?? IncidentPriority.Low;
                    incident.Priority = priority;
                    incident.DeadlineAt = GetDeadline(priority);
                } else if (user.Role == UserRole.Employee)
                {
                    incident.ExecutorId = user.Id;
                }
                break;
            case IncidentStatus.Completed:
                incident.CompletedAt = DateTime.UtcNow;
                break;
        }
        
        // Update incident status if all the previous steps are successfully performed.
        if (status != null) incident.Status = (IncidentStatus) status;
        if (newPriority != null && user.Role == UserRole.Official) 
            incident.Priority = (IncidentPriority) newPriority;
        
        await _db.SaveChangesAsync();
        
        return incident;
    }
}