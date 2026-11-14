using IncidentTriage.Domain.Evidence;

namespace IncidentTriage.Application.Evidence;

public interface IIncidentEvidenceSource
{
    Task<IReadOnlyList<EvidenceItem>> GetEvidenceAsync(
        EvidenceQuery query,
        CancellationToken cancellationToken = default);
}