namespace IncidentTriage.Domain.Evidence;

public enum EvidenceType
{
    Log,
    Metric,
    Email,
    Alert,
    ChangeEvent,
    Other
}

public sealed record EvidenceItem(
    Guid Id,
    Guid IncidentId,
    string Source,
    EvidenceType Type,
    DateTimeOffset OccurredAt,
    string Summary,
    string? SourceRecordId = null);
