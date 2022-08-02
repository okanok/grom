namespace Grom.GromQuery;

internal class QueryState
{
    internal IConstraintNode Query { get; set; }

    internal bool RetrieveRelationships { get; set; } = true;

    public QueryState(IConstraintNode query)
    {
        this.Query = query;
    }
}
