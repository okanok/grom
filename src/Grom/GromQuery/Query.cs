using System.Linq.Expressions;
using Grom.Entities;

namespace Grom.GromQuery;

public class Query<T> : DbRetriever<T> where T : EntityNode
{

    internal Query(IConstraintNode state) : base(new QueryState(state))
    {
    }

    /// <summary>
    /// Filter nodes based on given Grom DSL syntax tree
    /// This method is created for optimization purposes as it wont require parsing a lambda expression which is reflection heavy
    /// but is less safe and less expressive than a lambda filter
    /// </summary>
    /// <param name="constr">the constraints in the Grom DSL format</param>
    /// <returns></returns>
    public static DbRetriever<T> Where(IConstraintNode constr)
    {
        return new DbRetriever<T>(new QueryState(constr));
    }

    /// <summary>
    /// Filter nodes based on given lambda expression
    /// Grom will parse this into a query
    /// </summary>
    /// <param name="expr">a lambda expression with a single EntityNode parameter which returns a boolean</param>
    /// <returns></returns>
    public static DbRetriever<T> Where(Expression<Func<T, bool>> expr)
    {
        var expressoinMapper = new ExpressionToGromDSLMapper<T>(expr);
        var mappedExpression = expressoinMapper.Map();
        return new DbRetriever<T>(new QueryState(mappedExpression));
    }
}

public class DbRetriever<T> where T : EntityNode
{
    private readonly QueryState state;

    internal DbRetriever(QueryState state)
    {
        this.state = state;
    }

    public DbRetriever<T> IgnoreRelatedNodes()
    {
        state.RetrieveRelationships = false;
        return this;
    }

    /// <summary>
    /// Retrieves a single node 
    /// </summary>
    /// <returns></returns>
    public async Task<T> GetSingle()
    {
        return await GromGraph
            .GetDbConnector()
            .GetSingleNode<T>(state);
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        return await GromGraph
            .GetDbConnector()
            .GetNodes<T>(state);
    }
}

// Just an alias so QueryBase is still descriptive of its job and Retrieve is a nice name the user can use
public class Retrieve<T> : Query<T> where T : EntityNode
{
    private Retrieve(IConstraintNode state) : base(state)
    {
    }
}
