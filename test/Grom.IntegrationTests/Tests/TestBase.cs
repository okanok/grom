using Neo4j.Driver;

namespace Grom.IntegrationTests.Tests;

public class TestBase : IDisposable
{
    //private static readonly IDriver _driver;
    public static Action DisposeMethod;

    //static TestBase()
    //{
    //    //GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test")));
    //    //_driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test"));
    //}

    public void Dispose()
    {
        DisposeMethod();
    }
}
