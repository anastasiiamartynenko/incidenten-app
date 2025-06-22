using Incidenten.Domain.Enums;

namespace Incidenten.Web.Helpers;

public static class Converter
{
    public static IncidentStatus? StringToStatus(string status) => status switch
    {
        "Open" => IncidentStatus.Open,
        "Registered" => IncidentStatus.Registered,
        "InProgress" => IncidentStatus.InProgress,
        "Completed" => IncidentStatus.Completed,
        _ => null,
    };
    public static string? StatusToString(IncidentStatus status) => status switch
    {
        IncidentStatus.Open => "Open",
        IncidentStatus.Registered => "Registered",
        IncidentStatus.InProgress =>"InProgress",
        IncidentStatus.Completed => "Completed",
        _ => null,
    };
    public static IncidentPriority? StringToPriority(string priority) => priority switch
    {
        "Low" => IncidentPriority.Low,
        "Regular" => IncidentPriority.Regular,
        "High" => IncidentPriority.High,
        _ => null,
    };
    public static string? PriorityToString(IncidentPriority priority) => priority switch
    {
        IncidentPriority.Low => "Low",
        IncidentPriority.Regular => "Regular",
        IncidentPriority.High => "High",
        _ => null,
    };
}