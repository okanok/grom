using Gremlin.Net.Driver;
using Grom.GraphDbConnectors;
using Grom.GraphDbConnectors.Gremlin;
using Grom.GraphDbConnectors.Neo4J;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom;

public class GromGraph
{
    private static GromGraphDbConnector? _dbConnector;
    private static ITypeMapper? _typeMapper;

    private GromGraph()
    {
    }

    /// <summary>
    /// Makes sure Grom will use given Neo4J driver to create connection in underlying methods that require calls to the database
    /// </summary>
    /// <param name="driver">Any valid instance created with Neo4J GraphDatabase.Driver(...)</param>
    /// <returns></returns>
    public static void CreateConnection(IDriver driver)
    {
        _dbConnector = new GromNeo4jConnector(driver);
        _typeMapper = new Neo4JTypeMapper();
    }

    /// <summary>
    /// Makes sure Grom will use given Gremlin client to create connection in underlying methods that require calls to the database
    /// </summary>
    /// <param name="driver">Any valid instance of GremlinClient</param>
    /// <returns></returns>
    public static void CreateConnection(GremlinClient driver)
    {
        _dbConnector = new GromGremlinConnector(driver);
        _typeMapper = new GremlinTypeMapper();
    }

    /// <summary>
    /// Helper method to dispose the active connection. 
    /// </summary>
    public static void DisposeConnection()
    {
        _dbConnector = null;
    }

    internal static GromGraphDbConnector GetDbConnector()
    {
        if (_dbConnector is null)
        {
            throw new ConnectionNotInitializedException();
        }
        else
        {
            return _dbConnector;
        }
    }

    internal static ITypeMapper GetTypeMapper()
    {
        if (_typeMapper is null)
        {
            throw new ConnectionNotInitializedException();
        }
        else
        {
            return _typeMapper;
        }
    }
}
