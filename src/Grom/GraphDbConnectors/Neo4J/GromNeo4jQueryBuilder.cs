using System.Text;
using Grom.GromQuery;

namespace Grom.GraphDbConnectors.Neo4J;

internal class GromNeo4jQueryBuilder
{
    internal string BuildQuery(IConstraintNode state)
    {
        var strBuilder = new StringBuilder();
        ParseQueryNode((dynamic)state, strBuilder);
        return strBuilder.ToString();
    }

    private StringBuilder ParseQueryNode(PropertyConstraint constr, StringBuilder strBuilder)
    {
        ParseValueNode((dynamic)constr.Left, strBuilder);

        switch (constr.Comparison)
        {
            case Comparison.Eq:
                strBuilder.Append(" = ");
                break;
            case Comparison.Neq:
                strBuilder.Append(" <> ");
                break;
            case Comparison.Ge:
                strBuilder.Append(" > ");
                break;
            case Comparison.Le:
                strBuilder.Append(" < ");
                break;
            case Comparison.Leq:
                strBuilder.Append(" <= ");
                break;
            case Comparison.Geq:
                strBuilder.Append(" >= ");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.Comparison} not supported by Neo4J connector!");
        }

        ParseValueNode((dynamic)constr.Right, strBuilder);
        return strBuilder;
    }

    private StringBuilder ParseQueryNode(InfixConstraint constr, StringBuilder strBuilder)
    {
        strBuilder.Append("(");
        ParseQueryNode((dynamic)constr.left, strBuilder);

        switch (constr.infixOperator)
        {
            case InfixOperator.And:
                strBuilder.Append(" AND ");
                break;
            case InfixOperator.Or:
                strBuilder.Append(" OR ");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.infixOperator} not supported by Neo4J connector!");
        }

        ParseQueryNode((dynamic)constr.right, strBuilder);
        strBuilder.Append(')');
        return strBuilder;
    }

    private StringBuilder ParseQueryNode(PrefixConstraint constr, StringBuilder strBuilder)
    {
        switch (constr.prefixOperator)
        {
            case PrefixOperator.Not:
                strBuilder.Append("(NOT ");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.prefixOperator} not supported by Neo4J connector!");
        }

        ParseQueryNode((dynamic)constr.constraint, strBuilder);
        strBuilder.Append(')');
        return strBuilder;
    }

    private void ParseValueNode(NodeProperty property, StringBuilder strBuilder)
    {
        strBuilder.Append(String.Format("n.{0}", property.PropertyName));
    }

    private void ParseValueNode(Value value, StringBuilder strBuilder)
    {
        strBuilder.Append(value.ValueString);
    }
}
