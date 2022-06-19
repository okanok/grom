using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.NodeTests;

[Collection("neo4j-collection")]
public class DeleteNodeTests: Neo4JTestBase
{
    [Fact]
    public async Task DeletedNodeShouldReturnNull()
    {
        var node = new Person("Theon", 15);
        await node.Persist();

        var retrievedNode = await Retrieve<Person>
            .Where(n => n.Name == "Theon")
            .GetSingle();
        
        Assert.NotNull(retrievedNode);
        Assert.Equal(node.Name, retrievedNode!.Name);
        Assert.Equal(node.Age, retrievedNode.Age);

        await node.DeleteNode();

        var deletedNode = await Retrieve<Person>
            .Where(n => n.Name == "Theon")
            .GetSingle();

        Assert.Null(deletedNode);
    }

    [Fact]
    public async Task CorrectNodeShouldBeDeleted()
    {
        var node1 = new Person("Theon", 15);
        var node2 = new Person("Arya", 10);
        var node3 = new Person("Sansa", 12);

        await node1.Persist();
        await node2.Persist();
        await node3.Persist();

        await node2.DeleteNode();

        var deletedNode = await Retrieve<Person>
            .Where(n => n.Name == "Arya")
            .GetSingle();

        Assert.Null(deletedNode);

        var otherNode = await Retrieve<Person>
            .Where(n => n.Name == "Theon")
            .GetSingle();

        Assert.NotNull(otherNode);
    }
}
