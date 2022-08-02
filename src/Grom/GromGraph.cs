using Grom.GraphDbConnectors;
using Grom.GraphDbConnectors.Neo4J;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom;

public class GromGraph
{
    private static GromGraphDbConnector? _dbConnector;

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
        if (_dbConnector == null)
        {
            throw new ConnectionNotInitializedException();
        }
        else
        {
            return _dbConnector;
        }
    }
}
