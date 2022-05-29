using Grom.Entities;
using Grom.Entities.Attributes;
using Grom.Entities.Relationships;

namespace GromTest.GraphTest.Relationships;

public class Knows : EntityDirectedRelationship
{
    [RelationshipProperty]
    public int ForYears { get; set; }

    public Knows(EntityNode child) : base(child)
    {
    }

    public Knows(EntityNode child, int forYears) : base(child)
    {
        ForYears = forYears;

    }
}
