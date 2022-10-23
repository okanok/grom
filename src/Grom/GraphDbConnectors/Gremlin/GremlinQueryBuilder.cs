using Grom.GromQuery;
using System.Text;

namespace Grom.GraphDbConnectors.Gremlin;

internal class GremlinQueryBuilder : IQueryBuilder
{
    string IQueryBuilder.BuildQuery(IConstraintNode state)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append('.');
        ParseQueryNode((dynamic)state, strBuilder);
        return strBuilder.ToString();
    }

    private void ParseQueryNode(PropertyConstraint constr, StringBuilder strBuilder)
    {
        // has(prop, value)
        strBuilder.Append("has(");
        if(constr.Left is NodeProperty)
        {
            ParseValueNode((dynamic)constr.Left, strBuilder);
            strBuilder.Append(',');
            ParseComparison(constr.Comparison, strBuilder);
            ParseValueNode((dynamic)constr.Right, strBuilder);
            strBuilder.Append(')');
        }
        else if(constr.Right is NodeProperty)
        {
            ParseValueNode((dynamic)constr.Right, strBuilder);
            strBuilder.Append(',');
            ParseComparison(constr.Comparison, strBuilder);
            ParseValueNode((dynamic)constr.Left, strBuilder);
            strBuilder.Append(')');
        }
        else
        {
            throw new InvalidOperationException($"Comparison must compare a node property to some value");
        }
        strBuilder.Append(')');
    }

    private void ParseComparison(Comparison comparison, StringBuilder strBuilder)
    {
        switch (comparison)
        {
            case Comparison.Eq:
                // eq is implicit in has but still added because it makes the code less complex
                strBuilder.Append("eq");
                break;
            case Comparison.Neq:
                strBuilder.Append("neq");
                break;
            case Comparison.Ge:
                strBuilder.Append("gt");
                break;
            case Comparison.Le:
                strBuilder.Append("lt");
                break;
            case Comparison.Leq:
                strBuilder.Append("lte");
                break;
            case Comparison.Geq:
                strBuilder.Append("gte");
                break;
            default:
                throw new InvalidOperationException($"Operator {comparison} not supported by Gremlin connector!");
        }
        strBuilder.Append('(');
    }

    private void ParseQueryNode(InfixConstraint constr, StringBuilder strBuilder)
    {
        strBuilder.Append("where(");
        ParseQueryNode((dynamic)constr.left, strBuilder);

        switch (constr.infixOperator)
        {
            case InfixOperator.And:
                strBuilder.Append(".and().");
                break;
            case InfixOperator.Or:
                strBuilder.Append(".or().");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.infixOperator} not supported by Gremlin connector!");
        }

        ParseQueryNode((dynamic)constr.right, strBuilder);
        strBuilder.Append(')');
    }

    private void ParseQueryNode(PrefixConstraint constr, StringBuilder strBuilder)
    {
        switch (constr.prefixOperator)
        {
            case PrefixOperator.Not:
                strBuilder.Append("not(");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.prefixOperator} not supported by Gremlin connector!");
        }

        ParseQueryNode((dynamic)constr.constraint, strBuilder);
        strBuilder.Append(')');
    }

    private void ParseValueNode(NodeProperty property, StringBuilder strBuilder)
    {
        strBuilder.Append(String.Format("'{0}'", property.PropertyName));
    }

    private void ParseValueNode(Value value, StringBuilder strBuilder)
    {
        strBuilder.Append(value.ValueString);
    }


}
