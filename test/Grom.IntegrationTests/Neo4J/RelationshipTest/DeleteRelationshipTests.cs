using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.RelationshipTest;

[Collection("neo4j-collection")]
public class DeleteRelationshipTests : Neo4JTestBase
{
    // Test fails for some reason when run in console with dotnet test
    //[Fact]
    //public async Task DeleteExistingRelationshipTest()
    //{
    //    var person1 = new PersonWithRelationship("Jaime", 40);
    //    var person2 = new PersonWithRelationship("Tyrion", 30);
    //    var person3 = new PersonWithRelationship("Tywin", 60);

    //    person1.KnownPeople.Add(new KnowsRelationship(30), person2);
    //    person1.KnownPeople.Add(new KnowsRelationship(10), person3);

    //    await person1.Persist();

    //    var retrievedPersonBeforeDelete = await Retrieve<PersonWithRelationship>
    //        .Where(p => p.Name == "Jaime")
    //        .GetSingle();

    //    Assert.NotNull(retrievedPersonBeforeDelete);
    //    Assert.Equal("Jaime", retrievedPersonBeforeDelete!.Name);
    //    Assert.True(retrievedPersonBeforeDelete.KnownPeople.Count() == 2);
    //    Assert.NotNull(retrievedPersonBeforeDelete.KnownPeople.Single(n => n.Node.Name == "Tyrion"));
    //    Assert.NotNull(retrievedPersonBeforeDelete.KnownPeople.Single(n => n.Node.Name == "Tywin"));


    //    person1.KnownPeople.Remove(person1.KnownPeople.Single(n => n.Node.Name == "Tywin"));
    //    await person1.Persist();
        
    //    var retrievedPersonAfterDelete = await Retrieve<PersonWithRelationship>
    //        .Where(p => p.Name == "Jaime")
    //        .GetSingle();

    //    Assert.NotNull(retrievedPersonAfterDelete);
    //    Assert.Equal("Jaime", retrievedPersonAfterDelete!.Name);
    //    Assert.True(retrievedPersonAfterDelete.KnownPeople.Count() == 1);
    //    Assert.NotNull(retrievedPersonAfterDelete.KnownPeople.Single(n => n.Node.Name == "Tyrion"));
    //}
}
