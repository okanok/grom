namespace Grom.Util.Exceptions;

/// <summary>
/// This exception is thrown when a method like Persist() or Delete() (anything involving actually running queries on the database) 
/// is used before initializing a connection with GromGraph.CreateConnection(...).
/// </summary>
[Serializable]
public class ConnectionNotInitializedException : InvalidOperationException
{
    public ConnectionNotInitializedException()
        : base("A connection to a database has not been initialized yet! Please create a connection using GromGraph.CreateConnection(...)")
    {
    }
}
