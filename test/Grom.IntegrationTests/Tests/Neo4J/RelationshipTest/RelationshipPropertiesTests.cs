using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.IntegrationTests.Models;
using Grom.IntegrationTests.Tests;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class RelationshipPropertiesTests : TestBase
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
            LongProp = 1L,
            DateTimeProp = DateTime.Now,
            DateOnlyProp = DateOnly.FromDateTime(DateTime.Now)
        }, node2) ;

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
        Assert.Equal(node1.propertiesRelationship.First().Relationship.DateTimeProp, retrievedNode.propertiesRelationship.First().Relationship.DateTimeProp);
        Assert.Equal(node1.propertiesRelationship.First().Relationship.DateOnlyProp, retrievedNode.propertiesRelationship.First().Relationship.DateOnlyProp);
    }

    [Fact]
    public async Task CreatePersonWithCustomRelationshipPropertyNameTest()
    {
        var person1 = new PersonWithCustomRelationship("Jaime", 40);
        var person2 = new PersonWithCustomRelationship("Tyrion", 30);
        person1.KnownPeople.Add(new KnowsCustomNameRelationship(30), person2);

        await person1.Persist();

        var retrievedNode = await Retrieve<PersonWithCustomRelationship>
            .Where(n => n.Name == "Jaime")
            .GetSingle();

        Assert.NotNull(retrievedNode);
        Assert.Equal(person1.Name, retrievedNode.Name);
        Assert.Equal(30, retrievedNode.KnownPeople.First().Relationship.ForYears);
        Assert.Equal("Tyrion", retrievedNode.KnownPeople.First().Node.Name);
    }

    [Fact]
    public async Task UpdatePersonWithCustomRelationshipPropertyNameTest()
    {
        var person1 = new PersonWithCustomRelationship("Jaime", 40);
        var person2 = new PersonWithCustomRelationship("Tyrion", 30);
        person1.KnownPeople.Add(new KnowsCustomNameRelationship(30), person2);

        await person1.Persist();

        person1.KnownPeople.First().Relationship.ForYears = 10;

        await person1.KnownPeople.First().Relationship.UpdateRelationshipOnly();

        var retrievedNode = await Retrieve<PersonWithCustomRelationship>
            .Where(n => n.Name == "Jaime")
            .GetSingle();

        Assert.NotNull(retrievedNode);
        Assert.Equal(person1.Name, retrievedNode.Name);
        Assert.Equal(10, retrievedNode.KnownPeople.First().Relationship.ForYears);
        Assert.Equal("Tyrion", retrievedNode.KnownPeople.First().Node.Name);
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

    [RelationshipProperty]
    public DateTime DateTimeProp { get; set; }

    [RelationshipProperty]
    public DateOnly DateOnlyProp { get; set; }
}
