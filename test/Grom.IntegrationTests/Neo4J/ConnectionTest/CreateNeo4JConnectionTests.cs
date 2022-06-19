using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Grom.IntegrationTests.Models;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom.IntegrationTests.Neo4J.ConnectionTest;

[Collection("neo4j-connectiontest-collection")]
public class CreateNeo4JConnectionTests: IDisposable
{
    [Fact]
    public async Task CreateNeo4JConnectionBeforeInitializationShouldThrowException()
    {
        await Assert.ThrowsAsync<ConnectionNotInitializedException>(async () =>
        {
            var node = new Person("Aegon", 90);
            await node.Persist();
        });
    }

    [Fact]
    public async Task CreateConnectionWithBasicAuthShouldRun()
    {
        GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7688", AuthTokens.Basic("neo4j", "test")));
        var node = new Person("Aegon", 90);
        await node.Persist();
    }

    public void Dispose()
    {
        GromGraph.DisposeConnection();
    }
}


public class Neo4JConnectionFixture : IDisposable
{
    private static readonly TestcontainersContainer? testcontainer;

    static Neo4JConnectionFixture()
    {
        GromGraph.DisposeConnection();
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("neo4j:4.2.18")
          .WithName("noe4jConnectionTest")
          .WithEnvironment("NEO4J_AUTH", "neo4j/test")
          .WithPortBinding(7475, 7474)
          .WithPortBinding(7688, 7687)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(7474)
            .UntilPortIsAvailable(7687)
          );

        testcontainer = testcontainersBuilder.Build();
        testcontainer.StartAsync().Wait();
    }

    public void Dispose()
    {
        testcontainer!.DisposeAsync().AsTask().Wait();
    }
}

[CollectionDefinition("neo4j-connectiontest-collection")]
public class Neo4JDatabaseConnectionTestCollection : ICollectionFixture<Neo4JConnectionFixture>
{
}
