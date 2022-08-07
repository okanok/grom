using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class UpdateRelationshipTests: Neo4JTestBase
{
    [Fact]
    public async Task UpdateRelationshipTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        var person3 = new PersonWithRelationship("Tywin", 60);

        person2.KnownPeople.Add(new KnowsRelationship(30), person1);
        person2.KnownPeople.Add(new KnowsRelationship(20), person3);

        await person2.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.True(retrievedPerson.KnownPeople
            .Where(r => r.Relationship.ForYears == 30 || r.Relationship.ForYears == 20)
            .Count() == 2);

        retrievedPerson.KnownPeople.First().Relationship.ForYears = 100;

        await retrievedPerson.Persist();

        var retrievedAfterUpdatePerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();

        Assert.NotNull(retrievedAfterUpdatePerson);
        Assert.True(retrievedAfterUpdatePerson.KnownPeople
            .Where(r => r.Relationship.ForYears == 100)
            .Count() == 1);
    }

    [Fact]
    public async Task UpdateRelationshipOnlyTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        var person3 = new PersonWithRelationship("Tywin", 60);
        var person4 = new PersonWithRelationship("Jon", 16);

        person2.KnownPeople.Add(new KnowsRelationship(30), person1);
        person2.KnownPeople.Add(new KnowsRelationship(20), person3);
        person3.KnownPeople.Add(new KnowsRelationship(1), person4);

        await person2.Persist();

        person2.KnownPeople.First().Relationship.ForYears = 50;
        person2.KnownPeople[1].Relationship.ForYears = 10;
        person2.KnownPeople[1].Node.KnownPeople.First().Relationship.ForYears = 3;

        await person2.KnownPeople.First().Relationship.UpdateRelationshipOnly();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.True(retrievedPerson.KnownPeople
            .Where(r => r.Node.Name.Equals("Jaime") && r.Relationship.ForYears ==  50)
            .Count() == 1);
        Assert.True(retrievedPerson.KnownPeople
            .Where(r => r.Node.Name.Equals("Tywin") && r.Relationship.ForYears == 20)
            .Count() == 1);
        Assert.True(retrievedPerson.KnownPeople[1].Node.KnownPeople
            .Where(r => r.Node.Name.Equals("Jon") && r.Relationship.ForYears == 1)
            .Count() == 1);
    }

    [Fact]
    public async Task UpdateRelationshipOnlyDoesNotWorkWhenNodeIsNotCreatedTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        var person3 = new PersonWithRelationship("Tywin", 60);
        var person4 = new PersonWithRelationship("Jon", 16);

        person2.KnownPeople.Add(new KnowsRelationship(30), person1);
        person2.KnownPeople.Add(new KnowsRelationship(20), person3);
        person3.KnownPeople.Add(new KnowsRelationship(1), person4);


        person2.KnownPeople.First().Relationship.ForYears = 50;
        await person2.KnownPeople.First().Relationship.UpdateRelationshipOnly();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Tyrion")
            .GetSingle();

        Assert.Null(retrievedPerson);
    }
}
