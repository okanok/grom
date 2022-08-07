using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.QueryTest;

[Collection("neo4j-collection")]
public class RetrievingNodesTests: IClassFixture<Neo4JTestBase>
{
    static RetrievingNodesTests()
    {
        var personNode1 = new Person("Drogo", 30);
        var personNode2 = new Person("Khaleesi", 16);

        var propertyNode = new SupportedPropertiesNode
        {
            StringProp = "Some String",
            IntProp = 55,
            BoolProp = true,
            FloatProp = 42.0F,
            LongProp = 12345678901235L,
            DateTimeProp = DateTime.Now,
            DateOnlyProp = DateOnly.FromDateTime(DateTime.Now)
        };
        personNode1.Persist().Wait();
        personNode2.Persist().Wait();
        propertyNode.Persist().Wait();
    }

    [Fact]
    public async Task RetrievePersonNodeTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Drogo")
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Drogo", node!.Name);
        Assert.Equal(30, node.Age);
    }

    [Fact]
    public async Task RetrieveNonExistingNodeTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Drog")
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task RetrieveMultipleNodesTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name != "Drog")
            .GetAll();

        Assert.NotNull(node);
        Assert.True(node.Count() == 2);
        Assert.NotNull(node.SingleOrDefault(n => n.Name == "Drogo"));
        Assert.NotNull(node.SingleOrDefault(n => n.Name == "Khaleesi"));
    }

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

    [Fact]
    public async Task RetrieveNodeByDateTimeLessThanComparison()
    {
        var node = await Retrieve<SupportedPropertiesNode>
         .Where(n => n.DateTimeProp < DateTime.Now.AddDays(1))
         .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node.StringProp);
        Assert.True(node.DateTimeProp < DateTime.Now.AddDays(1));
    }

    [Fact]
    public async Task RetrieveNodeByDateTimeGreaterThanComparison()
    {
        var node = await Retrieve<SupportedPropertiesNode>
         .Where(n => n.DateTimeProp > DateTime.Now.AddDays(-1))
         .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node.StringProp);
        Assert.True(node.DateTimeProp > DateTime.Now.AddDays(-1));
    }

    [Fact]
    public async Task RetrieveNodeByDateOnlyLessThanComparison()
    {
        var node = await Retrieve<SupportedPropertiesNode>
         .Where(n => n.DateTimeProp < DateTime.Now.AddDays(1))
         .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node.StringProp);
        Assert.True(node.DateOnlyProp < DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
    }

    [Fact]
    public async Task RetrieveNodeByDateOnlyGreaterThanComparison()
    {
        var node = await Retrieve<SupportedPropertiesNode>
         .Where(n => n.DateTimeProp > DateTime.Now.AddDays(-1))
         .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node.StringProp);
        Assert.True(node.DateOnlyProp > DateOnly.FromDateTime(DateTime.Now.AddDays(-1)));
    }
}
