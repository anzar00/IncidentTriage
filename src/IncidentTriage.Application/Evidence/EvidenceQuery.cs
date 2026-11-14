namespace IncidentTriage.Application.Evidence;

public sealed record EvidenceQuery
{
    public const int MaximumAllowedItems = 100;

    public Guid IncidentId {get;}
    public int MaxItems {get;}

    public EvidenceQuery(Guid incidentId, int maxItems)
    {
        if(maxItems <1 || maxItems > MaximumAllowedItems)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxItems), maxItems, $"MaxItems must be between 1 and {MaximumAllowedItems}.");
        }

        IncidentId = incidentId;
        MaxItems = maxItems;
    }
}
