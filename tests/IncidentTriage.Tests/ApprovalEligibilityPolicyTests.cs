using IncidentTriage.Domain.Triage;
using IncidentTriage.Domain.Approvals;
using IncidentTriage.Application.Approvals;
using Xunit;

namespace IncidentTriage.Tests;

public sealed class ApprovalEligibilityPolicyTests
{
    private static readonly DateTimeOffset _now = new(2026, 11, 14, 12, 0, 0, TimeSpan.Zero);
    private static readonly TriageRecommendation _recommendation = new(
        Id: "recommendation-1",
        IncidentId: "incident-1",
        TargetServiceId: "service-1",
        TargetServiceVersion: "1.0.0",
        Action: TriageAction.InvestigateFurther,
        Rationale: "Investigate further due to high severity.",
        CreatedAt: _now);

    [Fact]
    public void PendingApproval_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Pending,
            ApprovedAt: null,
            ExpiresAt: _now.AddDays(1),
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.NotApproved, result.DenialReason);
    }

    [Fact]
    public void ExpiredApproval_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now,
            ExpiresAt: _now.AddDays(-1),
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.Expired, result.DenialReason);
    }

    [Fact]
    public void RecommendationMismatch_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: "different-recommendation-id",
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now,
            ExpiresAt: _now.AddDays(1),
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.RecommendationMismatch, result.DenialReason);
    }

    [Fact]
    public void TargetVersionChanged_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now,
            ExpiresAt: _now.AddDays(1),
            TargetServiceVersion: "2.0.0"); // Different version than recommendation

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.TargetVersionChanged, result.DenialReason);
    }

    [Fact]
    public void EligibleApproval_IsEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now,
            ExpiresAt: _now.AddDays(1),
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.True(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.None, result.DenialReason);
    } 

    [Fact]
    public void ApprovalExpiringAtCurrentTime_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now.AddMinutes(-1),
            ExpiresAt: _now,
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, _recommendation.TargetServiceVersion);

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.Expired, result.DenialReason);
    }

    [Fact]
    public void CurrentServiceVersionChanged_IsNotEligible()
    {
        var approval = new Approval(
            Id: "approval-1",
            RecommendationId: _recommendation.Id,
            ApproverId: "approver-1",
            Status: ApprovalStatus.Approved,
            ApprovedAt: _now,
            ExpiresAt: _now.AddDays(1),
            TargetServiceVersion: _recommendation.TargetServiceVersion);

        var policy = new ApprovalEligibilityPolicy(new FixedTimeProvider(_now));
        var result = policy.Evaluate(approval, _recommendation, currentTargetServiceVersion: "2.0.0");

        Assert.False(result.IsEligible);
        Assert.Equal(ApprovalDenialReason.TargetVersionChanged, result.DenialReason);
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _fixedTime;

        public FixedTimeProvider(DateTimeOffset fixedTime)
        {
            _fixedTime = fixedTime;
        }

        public override DateTimeOffset GetUtcNow() => _fixedTime;
    }
}
