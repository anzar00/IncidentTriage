using IncidentTriage.Domain.Approvals;
using IncidentTriage.Domain.Triage;

namespace IncidentTriage.Application.Approvals;

public enum ApprovalDenialReason {None, NotApproved, Expired, TargetVersionChanged, RecommendationMismatch}

public sealed record ApprovalEligibilityResult(bool IsEligible, ApprovalDenialReason DenialReason)
{
    public static ApprovalEligibilityResult Eligible {get;} = new ApprovalEligibilityResult(true, ApprovalDenialReason.None);
    public static ApprovalEligibilityResult Denied(ApprovalDenialReason reason) => new(false, reason);
}

public sealed class ApprovalEligibilityPolicy(TimeProvider timeProvider)
{
    public ApprovalEligibilityResult Evaluate(Approval approval, TriageRecommendation recommendation, string currentTargetServiceVersion)
    {
        if(approval.Status != ApprovalStatus.Approved)
            return ApprovalEligibilityResult.Denied(ApprovalDenialReason.NotApproved);

        if(!string.Equals(approval.RecommendationId, recommendation.Id, StringComparison.Ordinal))
            return ApprovalEligibilityResult.Denied(ApprovalDenialReason.RecommendationMismatch);

        if(approval.ExpiresAt <= timeProvider.GetUtcNow())
            return ApprovalEligibilityResult.Denied(ApprovalDenialReason.Expired);

        if(!string.Equals(approval.TargetServiceVersion, recommendation.TargetServiceVersion, StringComparison.Ordinal) ||
           !string.Equals(currentTargetServiceVersion, approval.TargetServiceVersion, StringComparison.Ordinal))
            return ApprovalEligibilityResult.Denied(ApprovalDenialReason.TargetVersionChanged);

        return ApprovalEligibilityResult.Eligible;
    }
}