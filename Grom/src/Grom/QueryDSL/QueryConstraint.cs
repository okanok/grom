//using Grom.Entities;
//using Grom.Util;

//namespace Grom.QueryDSL;

//public class QueryConstraint<T> where T : EntityNode
//{
//    internal string propertyName;
//    private readonly QueryState state;

//    internal QueryConstraint(string propertyName, QueryState state)
//    {
//        this.propertyName = propertyName;
//        this.state = state;
//    }

//    public QueryBase<T> Eq(object o)
//    {
//        var valueString = Utils.TypeStringify(o);
//        state.propertyConstraints.Add(new PropertyConstraint
//        {
//            Name = propertyName,
//            Comparison = Comparison.Eq,
//            Value = valueString
//        });
//        return new QueryBase<T>(state);
//    }

//    public QueryBase<T> Neq(object o)
//    {
//        var valueString = Utils.TypeStringify(o);
//        state.PropertyInequalityConstraints.Add(propertyName, valueString);
//        return new QueryBase<T>(state);
//    }

//    public QueryBase<T> Leq(object o)
//    {
//        var valueString = Utils.TypeStringify(o);
//        state.PropertyLeqConstraints.Add(propertyName, valueString);
//        return new QueryBase<T>(state);
//    }

//    public QueryBase<T> Geq(object o)
//    {
//        var valueString = Utils.TypeStringify(o);
//        state.PropertyGeqConstraints.Add(propertyName, valueString);
//        return new QueryBase<T>(state);
//    }
//}
