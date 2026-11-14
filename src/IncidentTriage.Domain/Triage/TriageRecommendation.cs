namespace IncidentTriage.Domain.Triage;

public enum TriageAction
{
    NoAction, 
    InvestigateFurther
}

public sealed record TriageRecommendation(
    string Id, 
    string IncidentId, 
    string TargetServiceId,
    string TargetServiceVersion, 
    TriageAction Action, 
    string Rationale,
    DateTimeOffset CreatedAt
);