using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.NodeTests;

[Collection("neo4j-collection")]
public class UpdateNodeTests: Neo4JTestBase
{
    [Fact]
    public async Task UpdatingNodeShouldUpdateRightProperties()
    {
        var node = new Person("Arya", 8);
        await node.Persist();
        var nodeBeforeChange = await Retrieve<Person>
            .Where(p => p.Name == "Arya")
            .GetSingle();

        Assert.NotNull(nodeBeforeChange);
        Assert.Equal(node.Name, nodeBeforeChange!.Name);
        Assert.Equal(node.Age, nodeBeforeChange.Age);

        node.Age = 10;

        await node.Persist();
        var nodeAfterChange = await Retrieve<Person>
            .Where(p => p.Name == "Arya")
            .GetSingle();

        Assert.NotNull(nodeAfterChange);
        Assert.Equal(node.Name, nodeAfterChange!.Name);
        Assert.Equal(node.Age, nodeAfterChange.Age);
    }

    [Fact]
    public async Task UpdatingAllSupportedPropertiesNodeShouldUpdatePropertiesCorrectly()
    {
        var node = new SupportedPropertiesNode
        {
            StringProp = "hello world",
            IntProp = 222,
            BoolProp = true,
            FloatProp = 1.1F,
            LongProp = 12345678901235L
        };
        await node.Persist();

        node.StringProp = "goodbye world";
        node.IntProp = 30;
        node.BoolProp = false;
        node.FloatProp = 200.1F;
        node.LongProp = 1L;

        await node.Persist();
        var nodeAfterChange = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.StringProp == "goodbye world")
            .GetSingle();

        Assert.NotNull(nodeAfterChange);
        Assert.Equal(node.StringProp, nodeAfterChange!.StringProp);
        Assert.Equal(node.IntProp, nodeAfterChange.IntProp);
        Assert.Equal(node.BoolProp, nodeAfterChange.BoolProp);
        Assert.Equal(node.FloatProp, nodeAfterChange.FloatProp);
        Assert.Equal(node.LongProp, nodeAfterChange.LongProp);
    }

    [Fact]
    public async Task CorrectNodeShouldBeUpdated()
    {
        var node1 = new Person("Theon", 15);
        var node2 = new Person("Arya", 10);
        var node3 = new Person("Sansa", 12);

        await node1.Persist();
        await node2.Persist();
        await node3.Persist();

        node3.Age = 22;
        await node3.Persist();

        var updatedNode = await Retrieve<Person>
            .Where(n => n.Name == "Sansa")
            .GetSingle();

        Assert.NotNull(updatedNode);
        Assert.Equal(node3.Name, updatedNode!.Name);
        Assert.Equal(node3.Age, updatedNode.Age);

        var otherNode = await Retrieve<Person>
            .Where(n => n.Name == "Arya")
            .GetSingle();

        Assert.NotNull(otherNode);
        Assert.Equal(node2.Name, otherNode!.Name);
        Assert.Equal(node2.Age, otherNode.Age);
    }
}
