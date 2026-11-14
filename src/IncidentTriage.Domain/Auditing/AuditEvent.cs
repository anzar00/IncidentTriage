namespace IncidentTriage.Domain.Auditing;

public enum AuditEventType
{
    IncidentDetected,
    RecommendationCreated,
    ApprovalGranted,
    ApprovalRejected
}

public enum AuditSubjectType
{
    Incident,
    Recommendation,
    Approval,
    Evidence
}

public sealed record AuditEvent(
    Guid Id,
    Guid SubjectId,
    AuditSubjectType SubjectType,
    AuditEventType EventType,
    DateTimeOffset OccurredAt,
    string ActorId,
    string Details);
