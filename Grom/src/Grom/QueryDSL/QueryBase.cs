using System.Linq.Expressions;
using Grom.Entities;
using Grom.Util;

namespace Grom.QueryDSL;

public class QueryBase<T> : DbRetriever<T> where T : EntityNode 
{
    private readonly IConstraintNode state;

    internal QueryBase(IConstraintNode state): base(state)
    {
        this.state = state;
    }

    // TODO: Implement Quick query for simply filtering one property
    //public static ConstraintBase<T> Where()
    //{
    //    return new ConstraintBase<T>(new QueryState());
    //}

    public static DbRetriever<T> Where(IConstraintNode constr)
    {
        return new DbRetriever<T>(constr);
    }

    public static DbRetriever<T> Where(Expression<Func<T, bool>> expr)
    {
        var a = new ExpressionToGromDSLMapper<T>(expr);
        var b = a.Map();
        // convert BinaryExpression to IConstraintNode
        return new DbRetriever<T>(b);
    }
}

public class DbRetriever<T> where T : EntityNode
{
    private readonly IConstraintNode state;

    internal DbRetriever(IConstraintNode state)
    {
        this.state = state;
    }

    public async Task<T?> GetSingle()
    {
        return await GromGraph
            .GetDbConnector()
            .GetSingleNode<T>(state);
    }

    public async Task<IEnumerable<T?>> GetAll()
    {
        return null;
        //return await GromGraph
        //    .GetDbConnector()
        //    .GetNodes<T>(state);
    }
}

// Just an alias so QueryBase is still descriptive of its job and Retrieve is a nice name the user can use
public class Retrieve<T> : QueryBase<T> where T : EntityNode
{
    private Retrieve(IConstraintNode state) : base(state)
    {
    }
}
