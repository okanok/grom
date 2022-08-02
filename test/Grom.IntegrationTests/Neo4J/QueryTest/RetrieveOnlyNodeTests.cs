using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.QueryTest;

[Collection("neo4j-collection")]
public class RetrieveOnlyNodeTests: Neo4JTestBase
{
    [Fact]
    public async Task RetrieveOnlyNodePersonWithRelationshipsShouldRetrieveOnlyNodeTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        person1.KnownPeople.Add(new KnowsRelationship(30), person2);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Jaime")
            .IgnoreRelatedNodes()
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.Equal("Jaime", retrievedPerson!.Name);
        Assert.Equal(40, retrievedPerson.Age);
        Assert.True(retrievedPerson.KnownPeople.Count() == 0);
    }

    [Fact]
    public async Task RetrieveOnlyNodePersonWithRelationshipsShouldWorkWhenPersonHasNoRelationshipsNodeTest()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);

        await person1.Persist();

        var retrievedPerson = await Retrieve<PersonWithRelationship>
            .Where(p => p.Name == "Jaime")
            .GetSingle();

        Assert.NotNull(retrievedPerson);
        Assert.Equal("Jaime", retrievedPerson!.Name);
        Assert.Equal(40, retrievedPerson.Age);
        Assert.True(retrievedPerson.KnownPeople.Count() == 0);
    }
}
