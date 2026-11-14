namespace IncidentTriage.Domain.Triage;

public enum TriageAction
{
    NoAction, 
    InvestiageFurther
}

public sealed record TriaggeRecommendation(
    string Id, 
    string IncidentId, 
    string TargetServiceId,
    string TargetServiceVersion, 
    TriageAction Action, 
    string Rationale,
    DateTimeOffset CreatedAt
);