namespace IncidentTriage.Domain.Auditing;

public enum AuditEventType { IncidentDetected, RecommendationCreated, ApprovalGranted, ApprovalRejected }

public sealed record AuditEvent(
    string Id,
    string SubjectId,
    AuditEventType EventType,
    DateTimeOffset OccuredAt,
    string ActorId,
    string Details);