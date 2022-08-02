namespace Grom.Util.Exceptions;

public class QueryResultException : InvalidOperationException
{
    public QueryResultException(string message) : base(message)
    {
    }
}
