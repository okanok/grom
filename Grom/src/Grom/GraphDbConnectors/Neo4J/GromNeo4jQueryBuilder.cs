using System.Text;
using Grom.QueryDSL;

namespace Grom.GraphDbConnectors.Neo4J;

internal class GromNeo4jQueryBuilder
{
    internal string BuildQuery(IConstraintNode state)
    {
        var strBuilder = new StringBuilder();
        parseQueryNode((dynamic)state, strBuilder);
        return strBuilder.ToString();
    }

    private StringBuilder parseQueryNode(PropertyConstraint constr, StringBuilder strBuilder)
    {
        //strBuilder.Append(String.Format("n.{0}", constr.Name));

        //switch (constr.Comparison)
        //{
        //    case Comparison.Eq:
        //        strBuilder.Append(" = ");
        //        break;
        //    case Comparison.Neq:
        //        strBuilder.Append(" <> ");
        //        break;
        //    case Comparison.Leq:
        //        strBuilder.Append(" <= ");
        //        break;
        //    case Comparison.Geq:
        //        strBuilder.Append(" >= ");
        //        break;
        //    default:
        //        throw new InvalidOperationException($"Operator {constr.Comparison} not supported by connector!"); //TODO: also name the connector
        //}

        //strBuilder.Append(constr.Value);
        return strBuilder;
    }

    private StringBuilder parseQueryNode(InfixConstraint constr, StringBuilder strBuilder)
    {
        strBuilder.Append("(");
        parseQueryNode((dynamic)constr.left, strBuilder);

        switch (constr.infixOperator)
        {
            case InfixOperator.And:
                strBuilder.Append(" AND ");
                break;
            case InfixOperator.Or:
                strBuilder.Append(" OR ");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.infixOperator} not supported by connector!"); //TODO: also name the connector
        }

        parseQueryNode((dynamic)constr.right, strBuilder);
        strBuilder.Append(')');
        return strBuilder;
    }

    private StringBuilder parseQueryNode(PrefixConstraint constr, StringBuilder strBuilder)
    {
        switch (constr.prefixOperator)
        {
            case PrefixOperator.Not:
                strBuilder.Append("(NOT ");
                break;
            default:
                throw new InvalidOperationException($"Operator {constr.prefixOperator} not supported by connector!"); //TODO: also name the connector
        }

        parseQueryNode((dynamic)constr.constraint, strBuilder);
        strBuilder.Append(')');
        return strBuilder;
    }
}
