using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Grom.IntegrationTests.Tests;
using Neo4j.Driver;

namespace Grom.IntegrationTests.DbColletions;

public class Neo4JFixture : IDbCollection
{
    private static readonly TestcontainersContainer? testcontainer;
    private static readonly IDriver _driver;


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
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test"));
        GromGraph.CreateConnection(_driver);
        TestBase.DisposeMethod = DisposeData;
    }

    public void Dispose()
    {
        testcontainer!.DisposeAsync().AsTask().Wait();
    }

    public static void DisposeData()
    {
        var session = _driver.AsyncSession();

        session.WriteTransactionAsync(async tx =>
        {
            await tx.RunAsync("MATCH (a) OPTIONAL MATCH (a)-[r]-(b) DELETE r,a,b");
        }).Wait();
    }
}

[CollectionDefinition("neo4j-collection", DisableParallelization = true)]
public class Neo4JDatabaseCollection : ICollectionFixture<Neo4JFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
