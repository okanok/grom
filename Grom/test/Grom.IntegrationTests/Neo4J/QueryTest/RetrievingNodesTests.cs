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
            LongProp = 12345678901235L
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
    public async Task RetrievePropertiesTest()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.StringProp == "Some String")
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node!.StringProp);
        Assert.Equal(55, node.IntProp);
        Assert.True(node.BoolProp);
        Assert.Equal(42.0F, node.FloatProp);
        Assert.Equal(12345678901235L, node.LongProp);
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

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
