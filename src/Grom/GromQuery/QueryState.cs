namespace Grom.GromQuery;

internal class QueryState
{
    internal IConstraintNode Query { get; }

    internal string RootNodeName { get; }

    internal bool RetrieveRelationships { get; set; } = true;


    public QueryState(IConstraintNode query)
    {
        Query = query;
    }

    public QueryState(IConstraintNode query, string rootNodeName)
    {
        Query = query;
        RootNodeName = rootNodeName;
    }
}
