using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Tests.OrientDB.NodeTests;

[Collection("orientdb-collection")]
public class CreateNodeTests : TestBase
{

    [Fact]
    public void CreateNodeInstance()
    {
        var node = new Person
        {
            Name = "Jon",
            Age = 16
        };

        Assert.Equal("Jon", node.Name);
        Assert.Equal(16, node.Age);
    }

    [Fact]
    public void CreateNodeInstanceWithConstructor()
    {
        var node = new Person("Jon", 16);

        Assert.Equal("Jon", node.Name);
        Assert.Equal(16, node.Age);
    }

    [Fact]
    public async Task PersistNodeShouldRun()
    {
        var node = new Person
        {
            Name = "Bran",
            Age = 6
        };
        await node.Persist();

        Assert.Equal("Bran", node.Name);
        Assert.Equal(6, node.Age);
        //TODO: also check entityNodeId with reflection
    }

    [Fact]
    public async Task PersistMultipleNodesShouldRun()
    {
        var node1 = new Person
        {
            Name = "Tyrion",
            Age = 32
        };
        await node1.Persist();

        var node2 = new Person
        {
            Name = "Jaime",
            Age = 40
        };
        await node2.Persist();
    }

    [Fact]
    public async Task PersistMultipleNodesWithSamePropertiesShouldRun()
    {
        var node1 = new Person
        {
            Name = "Tyrion",
            Age = 32
        };
        await node1.Persist();

        var node2 = new Person
        {
            Name = "Tyrion",
            Age = 32
        };
        await node2.Persist();
    }
}
