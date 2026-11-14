using IncidentTriage.Application.Approvals;
using IncidentTriage.Domain.Approvals;
using IncidentTriage.Domain.Triage;
using Xunit;

namespace IncidentTriage.Tests;

public sealed class ApprovalEligibilityPolicyTests
{
    private static readonly DateTimeOffset Now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid RecommendationId = new("57e25a9b-2d1e-4892-8ba0-f8db74ea13a1");
    private static readonly TriageRecommendation Recommendation = new(
        Id: RecommendationId,
        IncidentId: new Guid("7f6fb2ed-04de-4b66-b775-c39e343326d8"),
        TargetServiceId: "service-1",
        TargetServiceVersion: "1.0.0",
        Action: TriageAction.InvestigateFurther,
        Rationale: "Investigate further due to high severity.",
        CreatedAt: Now);

    [Fact]
    public void PendingApproval_IsNotEligible()
    {
        var result = Evaluate(Approval(status: ApprovalStatus.Pending));

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.NotApproved, result.DenialReason);
    }

    [Fact]
    public void ExpiredApproval_IsNotEligible()
    {
        var result = Evaluate(Approval(expiresAt: Now.AddDays(-1)));

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.Expired, result.DenialReason);
    }

    [Fact]
    public void RecommendationMismatch_IsNotEligible()
    {
        var result = Evaluate(Approval(recommendationId: Guid.NewGuid()));

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.RecommendationMismatch, result.DenialReason);
    }

    [Fact]
    public void TargetVersionChanged_IsNotEligible()
    {
        var result = Evaluate(Approval(targetVersion: "2.0.0"));

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.TargetVersionChanged, result.DenialReason);
    }

    [Fact]
    public void EligibleApproval_IsEligible()
    {
        var result = Evaluate(Approval());

        Assert.True(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.None, result.DenialReason);
    }

    [Fact]
    public void ApprovalExpiringAtCurrentTime_IsNotEligible()
    {
        var result = Evaluate(Approval(expiresAt: Now));

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.Expired, result.DenialReason);
    }

    [Fact]
    public void CurrentServiceVersionChanged_IsNotEligible()
    {
        var result = Evaluate(Approval(), currentTargetVersion: "2.0.0");

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.TargetVersionChanged, result.DenialReason);
    }

    private static ApprovalEligibilityResult Evaluate(Approval approval, string? currentTargetVersion = null)
    {
        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(Now));
        return policy.Evaluate(approval, Recommendation, currentTargetVersion ?? Recommendation.TargetServiceVersion);
    }

    private static Approval Approval(
        ApprovalStatus status = ApprovalStatus.Approved,
        DateTimeOffset? expiresAt = null,
        Guid? recommendationId = null,
        string? targetVersion = null) =>
        new(
            Id: new Guid("b5e2ff58-65f9-4131-9b1f-b4f869f33d6c"),
            RecommendationId: recommendationId ?? Recommendation.Id,
            ApproverId: "approver-1",
            Status: status,
            ApprovedAt: status == ApprovalStatus.Pending ? null : Now,
            ExpiresAt: expiresAt ?? Now.AddDays(1),
            TargetServiceVersion: targetVersion ?? Recommendation.TargetServiceVersion);

    private sealed class FixedTimeProvider(DateTimeOffset fixedTime) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => fixedTime;
    }
}
