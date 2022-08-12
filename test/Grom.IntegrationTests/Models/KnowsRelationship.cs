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

public class KnowsCustomNameRelationship : RelationshipBase
{
    [RelationshipProperty("yearsKnown")]
    public int ForYears { get; set; }

    public KnowsCustomNameRelationship()
    {
    }

    public KnowsCustomNameRelationship(int forYears)
    {
        ForYears = forYears;

    }
}
