using Grom.GromQuery;
using Grom.IntegrationTests.Models;
using Grom.IntegrationTests.Tests;

namespace Grom.IntegrationTests.Neo4J.QueryTest;

[Collection("neo4j-collection")]
public class BooleanOperatorQueryTests: IClassFixture<TestBase>
{
    public BooleanOperatorQueryTests()
    {
        var personNode1 = new Person("Drogo", 30);
        var personNode2 = new Person("Jaime", 40);
        var personNode3 = new Person("Robert", 50);
        var personNode4 = new Person("Eddard", 50);
        personNode1.Persist().Wait();
        personNode2.Persist().Wait();
        personNode3.Persist().Wait();
        personNode4.Persist().Wait();
    }

    [Fact]
    public async Task QueryWithAndMatchTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Drogo" && n.Age == 30)
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Drogo", node!.Name);
        Assert.Equal(30, node.Age);
    }

    [Fact]
    public async Task QueryWithAndNoMatchTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Robb" && n.Age == 30)
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task QueryWithOrMatchTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Tywin" || n.Age == 40)
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Jaime", node!.Name);
        Assert.Equal(40, node.Age);
    }

    [Fact]
    public async Task QueryWithOrNoMatchesTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Tywin" || n.Age == 10)
            .GetSingle();

        Assert.Null(node);
    }
}
