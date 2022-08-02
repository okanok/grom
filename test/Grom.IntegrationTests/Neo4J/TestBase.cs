using Neo4j.Driver;

namespace Grom.IntegrationTests.Neo4J;

public class Neo4JTestBase : IDisposable
{
    private static readonly IDriver _driver;

    static Neo4JTestBase()
    {
        GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test")));
        _driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test"));
    }

    public void Dispose()
    {
        var session = _driver.AsyncSession();

        session.WriteTransactionAsync(async tx =>
        {
            await tx.RunAsync("MATCH (a) OPTIONAL MATCH (a)-[r]-(b) DELETE r,a,b");
        }).Wait();
    }
}
