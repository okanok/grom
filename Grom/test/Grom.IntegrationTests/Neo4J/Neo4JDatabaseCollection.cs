using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Neo4j.Driver;

namespace Grom.IntegrationTests.Neo4J;

public class Neo4JFixture : IDisposable
{
    private static readonly TestcontainersContainer? testcontainer;

    static Neo4JFixture()
    {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("neo4j:4.2.18")
          .WithName("noe4j")
          .WithEnvironment("NEO4J_AUTH", "neo4j/test")
          .WithPortBinding(7474, 7474)
          .WithPortBinding(7687, 7687)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(7474)
            .UntilPortIsAvailable(7687)
          );

        testcontainer = testcontainersBuilder.Build();
        testcontainer.StartAsync().Wait();
        GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test")));
    }

    public void Dispose()
    {
        testcontainer!.DisposeAsync().AsTask().Wait();
    }
}

[CollectionDefinition("neo4j-collection")]
public class Neo4JDatabaseCollection : ICollectionFixture<Neo4JFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
