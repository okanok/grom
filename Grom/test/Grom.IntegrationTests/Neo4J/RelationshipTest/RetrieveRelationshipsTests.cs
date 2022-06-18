using Grom.Entities;
using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class RetrieveRelationshipsTests: Neo4JTestBase
{

    [Fact]
    public async Task RetrievePersonWithRelationshipsTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        person1.KnownPeople.Add(new KnowsRelationship(30), person2);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Jaime")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.Equal("Jaime", retrievedPerson!.Name);
        Assert.Equal(40, retrievedPerson.Age);
        Assert.True(retrievedPerson.KnownPeople.Count() == 1);
        Assert.NotNull(retrievedPerson.KnownPeople.FirstOrDefault());
        Assert.Equal(30, retrievedPerson.KnownPeople.FirstOrDefault()!.Relationship.ForYears);

        var retrievedRelatedPerson = retrievedPerson.KnownPeople.FirstOrDefault()!.Node;
        Assert.NotNull(retrievedRelatedPerson);
        Assert.Equal("Tyrion", retrievedRelatedPerson!.Name);
        Assert.Equal(30, retrievedRelatedPerson.Age);
        Assert.True(retrievedRelatedPerson.KnownPeople.Count() == 0);
    }

    [Fact]
    public async Task RetrievePersonWithMultipleRelationships()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        var person3 = new PersonWithRelationship("Tywin", 60);
        var person4 = new PersonWithRelationship("Drogo", 30);

        person2.KnownPeople.Add(new KnowsRelationship(30), person1);
        person2.KnownPeople.Add(new KnowsRelationship(20), person3);

        await person2.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();
        
        Assert.NotNull(retrievedPerson);
        Assert.Equal("Tyrion", retrievedPerson!.Name);
        Assert.Equal(30, retrievedPerson.Age);
        Assert.True(retrievedPerson.KnownPeople.Count() == 2);
        Assert.True(retrievedPerson.KnownPeople.Where(r => r.Node.Name == "Jaime").Count() == 1);
        Assert.True(retrievedPerson.KnownPeople.Where(r => r.Node.Name == "Tywin").Count() == 1);
    }

    [Fact]
    public async Task RetrievePersonWithMultipleKindsOfRelationships()
    {
        var person1 = new PersonMultipleRelationshipKinds
        {
            Name = "Tyrion"
        };
        var person2 = new PersonWithRelationship("Jaime", 30);
        var person3 = new PersonMultipleRelationshipKinds
        {
            Name = "Tywin"
        };

        person1.LikedPeople.Add(new LikesRelationship { ReasonToLike = "Brother" }, person2);
        person1.HatedPeople.Add(new HatesRelationship { ReasonToHate = "Bad father" }, person3);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonMultipleRelationshipKinds>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.Equal("Tyrion", retrievedPerson!.Name);
        Assert.True(retrievedPerson.LikedPeople.Count() == 1);
        Assert.True(retrievedPerson.HatedPeople.Count() == 1);
        Assert.True(retrievedPerson.LikedPeople.Where(r => r.Node.Name == "Jaime").Count() == 1);
        Assert.True(retrievedPerson.HatedPeople.Where(r => r.Node.Name == "Tywin").Count() == 1);
    }

    [Fact]
    public async Task RetrievePersonWithEmptyRelationships()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Jaime")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.Equal("Jaime", person1.Name);
        Assert.True(person1.KnownPeople.Count() == 0);
    }

    [Fact]
    public async Task RetrievePersonWithThreeGenerationsOfAncestors()
    {
        var person1 = new PersonWithRelationship("Rickon", 4);
        var person2 = new PersonWithRelationship("Eddard", 40);
        var person3 = new PersonWithRelationship("Rickard", 60);

        person1.KnownPeople.Add(new KnowsRelationship(4), person2);
        person2.KnownPeople.Add(new KnowsRelationship(20), person3);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Rickon")
            .GetSingle();

        Assert.NotNull(person1);
        Assert.Equal("Rickon", person1.Name);
        Assert.True(person1.KnownPeople.Count() == 1);
        Assert.Equal("Eddard", person1.KnownPeople.First().Node.Name);
        Assert.True(person1.KnownPeople.First().Node.KnownPeople.Count() == 1);
        Assert.Equal("Rickard", person1.KnownPeople.First().Node.KnownPeople.First().Node.Name);
        Assert.True(person1.KnownPeople.First().Node.KnownPeople.First().Node.KnownPeople.Count() == 0);
    }
}


public class PersonMultipleRelationshipKinds: EntityNode
{
    [NodeProperty]
    public string Name { get; set; }

    public RelationshipCollection<LikesRelationship, PersonWithRelationship> LikedPeople { get; set; } = new();
    public RelationshipCollection<HatesRelationship, PersonMultipleRelationshipKinds> HatedPeople { get; set; } = new();

}

public class LikesRelationship: RelationshipBase
{
    [RelationshipProperty]
    public string ReasonToLike { get; set; }
}

public class HatesRelationship : RelationshipBase
{
    [RelationshipProperty]
    public string ReasonToHate { get; set; }
}
