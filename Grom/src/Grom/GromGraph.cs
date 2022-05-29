using Grom.GraphDbConnectors;

namespace Grom;

public class GromGraph
{
    // TODO: make this async and transaction friendly
    // TODO: make this also 
    private static GromGraph? _connectionInstance;
    private static GromGraphDbConnector? _dbConnector;

    private GromGraph()
    {
    }

    public static GromGraph CreateConnection(GromGraphDbConnector dbConnector)
    {
        _dbConnector = dbConnector;
        if (_connectionInstance == null)
        {
            _connectionInstance = new GromGraph();
        }
        return _connectionInstance;
    }

    internal static GromGraphDbConnector GetDbConnector()
    {
        if (_dbConnector == null)
        {
            // TODO: custom exception
            throw new Exception("Database connection not initialized yet! First create a connection with CreateConnection() before retrieving a session.");
        }
        else
        {
            // TODO: support session for specific database name
            return _dbConnector;
        }
    }
}
