using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Grom.GraphDbConnectors.Neo4J;

namespace Grom.IntegrationTests.Neo4J;

public class Neo4JFixture : IDisposable
{

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

        var testcontainers = testcontainersBuilder.Build();
        testcontainers.StartAsync().Wait();
        GromGraph.CreateConnection(new GromNeo4jConnector("bolt://localhost:7687", "neo4j", "test"));
    }

    public void Dispose()
    {

    }
}

[CollectionDefinition("neo4j-collection")]
public class Neo4JDatabaseCollection : ICollectionFixture<Neo4JFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
