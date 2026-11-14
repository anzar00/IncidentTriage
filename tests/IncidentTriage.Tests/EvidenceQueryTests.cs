using IncidentTriage.Application.Evidence;

namespace IncidentTriage.Tests;

public sealed class EvidenceQueryTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void EvidenceQuery_MaxItems_InvalidRange_Throws_ArgumentOutOfRangeException(int maxItems)
    {
        // Arrange
        var incidentId = Guid.NewGuid();

        // Act & Assert 
        Assert.Throws<ArgumentOutOfRangeException>(() => new EvidenceQuery(incidentId, maxItems));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void EvidenceQuery_MaxItems_ValidRange_DoesNotThrow(int maxItems)
    {
        // Arrange 
        var incidentId = Guid.NewGuid();

        // Act & Assert 
        var query = new EvidenceQuery(incidentId, maxItems);

        Assert.Equal(incidentId, query.IncidentId);
        Assert.Equal(maxItems, query.MaxItems);
    }
}