using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class CreateRelationshipTests: Neo4JTestBase
{
    [Fact] 
    public async Task CreatePersonWithRelationshipTests()
    {
        var person1 = new PersonWithRelationship("Jaime", 40);
        var person2 = new PersonWithRelationship("Tyrion", 30);
        person1.KnownPeople.Add(new KnowsRelationship(30), person2);

        await person1.Persist();
    }

}
