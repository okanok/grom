using Grom.Entities.Attributes;
using Grom.Entities.Relationships;

namespace Grom.IntegrationTests.Models;

public class KnowsRelationship : RelationshipBase
{
    [RelationshipProperty]
    public int ForYears { get; set; }

    public KnowsRelationship()
    {
    }

    public KnowsRelationship(int forYears)
    {
        ForYears = forYears;

    }
}
