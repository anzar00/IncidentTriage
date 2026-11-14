namespace IncidentTriage.Domain.Approvals;

public enum ApprovalStatus { Pending, Approved, Rejected, Revoked}

public sealed record Approval(
    string Id,
    string RecommendationId,
    string ApproverId, 
    ApprovalStatus Status,
    DateTimeOffset? ApprovedAt,
    DateTimeOffset ExpiresAt,
    string TargetServiceVersion);