namespace IncidentTriage.Domain.Incidents;

public enum IncidentSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public sealed record Incident(
    Guid Id,
    string ServiceId,
    string ServiceVersion,
    IncidentSeverity Severity,
    DateTimeOffset DetectedAt);
