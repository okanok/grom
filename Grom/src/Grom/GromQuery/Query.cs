using System.Linq.Expressions;
using Grom.Entities;

namespace Grom.GromQuery;

public class Query<T> : DbRetriever<T> where T : EntityNode
{
    private readonly IConstraintNode state;

    internal Query(IConstraintNode state) : base(state)
    {
        this.state = state;
    }

    public static DbRetriever<T> Where(IConstraintNode constr)
    {
        return new DbRetriever<T>(constr);
    }

    public static DbRetriever<T> Where(Expression<Func<T, bool>> expr)
    {
        var expressoinMapper = new ExpressionToGromDSLMapper<T>(expr);
        var a = expressoinMapper.Map();
        return new DbRetriever<T>(a);
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
public class Retrieve<T> : Query<T> where T : EntityNode
{
    private Retrieve(IConstraintNode state) : base(state)
    {
    }
}
