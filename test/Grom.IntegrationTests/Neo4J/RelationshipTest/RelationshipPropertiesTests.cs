using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class RelationshipPropertiesTests : Neo4JTestBase
{
    [Fact]
    public async Task TestCreatingRelationsipWithAllProperties()
    {
        var node1 = new PersonWithAllPropertyTypesRelationship("Shae", 25);
        var node2 = new PersonWithAllPropertyTypesRelationship("Tyrion", 30);

        node1.propertiesRelationship.Add(new RelationshipWithAllProperties
        {
            StringProp = "This is a STRING",
            IntProp = 1,
            BoolProp = false,
            FloatProp = 33F,
            LongProp = 1L
        }, node2);

        await node1.Persist();

        var retrievedNode = await Retrieve<PersonWithAllPropertyTypesRelationship>
            .Where(n => n.Name == "Shae")
            .GetSingle();

        Assert.NotNull(retrievedNode);
        Assert.Equal("Shae", retrievedNode!.Name);
        Assert.True(retrievedNode.propertiesRelationship.Count() == 1);
        Assert.Equal("This is a STRING", retrievedNode.propertiesRelationship.First().Relationship.StringProp);
        Assert.Equal(1, retrievedNode.propertiesRelationship.First().Relationship.IntProp);
        Assert.False(retrievedNode.propertiesRelationship.First().Relationship.BoolProp);
        Assert.Equal(33F, retrievedNode.propertiesRelationship.First().Relationship.FloatProp);
        Assert.Equal(1L, retrievedNode.propertiesRelationship.First().Relationship.LongProp);
    }
}

public class PersonWithAllPropertyTypesRelationship : Person
{
    public PersonWithAllPropertyTypesRelationship()
    {
    }

    public PersonWithAllPropertyTypesRelationship(string name, int age) : base(name, age)
    {
    }

    public RelationshipCollection<RelationshipWithAllProperties, PersonWithAllPropertyTypesRelationship> propertiesRelationship { get; set; } = new();
}


public class RelationshipWithAllProperties : RelationshipBase
{
    [RelationshipProperty]
    public string StringProp { get; set; }

    [RelationshipProperty]
    public int IntProp { get; set; }

    [RelationshipProperty]
    public bool BoolProp { get; set; }

    [RelationshipProperty]
    public float FloatProp { get; set; }

    [RelationshipProperty]
    public long LongProp { get; set; }
}
