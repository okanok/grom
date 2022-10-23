using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Gremlin.Net.Driver;
using Grom.IntegrationTests.Tests;

namespace Grom.IntegrationTests.DbColletions;

public class OrientDbFixture : IDbCollection
{
    private static readonly TestcontainersContainer? testcontainer;
    private static readonly GremlinClient gremlinClient;


    static OrientDbFixture()
    {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("orientdb:3.2.8-tp3")
          .WithName("orientdb")
          .WithEnvironment("ORIENTDB_ROOT_PASSWORD", "test")
          .WithPortBinding(2424, 2424)
          .WithPortBinding(2480, 2480)
          .WithPortBinding(8182, 8182)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilPortIsAvailable(2424)
            .UntilPortIsAvailable(2480)
            .UntilPortIsAvailable(8182)
          );

        testcontainer = testcontainersBuilder.Build();
        testcontainer.StartAsync().Wait();

        var gremlinServer = new GremlinServer(
            hostname: "localhost",
            port: 8182,
            username: "root",
            password: "test"
        );
        var connectionPoolSettings = new ConnectionPoolSettings
        {
            MaxInProcessPerConnection = 32,
            PoolSize = 4,
            ReconnectionAttempts = 4,
            ReconnectionBaseDelay = TimeSpan.FromSeconds(1)
        };
        gremlinClient = new GremlinClient(
            gremlinServer: gremlinServer,
            connectionPoolSettings: connectionPoolSettings
        );

        GromGraph.CreateConnection(gremlinClient);

        TestBase.DisposeMethod = DisposeData;
    }

    public void Dispose()
    {
        testcontainer!.DisposeAsync().AsTask().Wait();
    }

    public static void DisposeData()
    {
        gremlinClient.SubmitAsync("g.V().drop().iterate()").Wait();
    }
}


[CollectionDefinition("orientdb-collection", DisableParallelization = true)]
public class OrientDbDatabaseCollection : ICollectionFixture<OrientDbFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

