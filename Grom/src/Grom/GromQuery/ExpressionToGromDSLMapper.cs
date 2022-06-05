using Grom.Util;
using System.Linq.Expressions;

namespace Grom.GromQuery;

internal class ExpressionToGromDSLMapper<T>
{
    private readonly ExpressionType[] valueExpressionTypes = { ExpressionType.MemberAccess, ExpressionType.Constant, ExpressionType.Call }; 

    private string parameterName;
    private Expression<Func<T, bool>> expr;

    internal ExpressionToGromDSLMapper(Expression<Func<T, bool>> expr)
    {
        this.expr = expr;
        if (expr.Parameters.Count() != 1 || expr.Parameters.First().Name is null)
        {
            throw new ArgumentException("Where clause expects exactly one parameter which represents the node for which you want to filter properties!");
        }
        parameterName = expr.Parameters.First().Name;
    }

    internal IConstraintNode Map()
    {
        return Map(expr.Body);
    }

    internal IConstraintNode Map(Expression expr) //TODO: split into multiple
    {
        switch (expr.NodeType)
        {
            case ExpressionType.AndAlso:
                return MapExpressionToInfixOperator((BinaryExpression)expr, InfixOperator.And);
            case ExpressionType.OrElse:
                return MapExpressionToInfixOperator((BinaryExpression)expr, InfixOperator.Or);
            case ExpressionType.Not:
                return MapExpressionToPrefixOperator((UnaryExpression)expr, PrefixOperator.Not);
            case ExpressionType.Equal:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Eq);
            case ExpressionType.NotEqual:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Neq);
            case ExpressionType.GreaterThan:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Ge);
            case ExpressionType.LessThan:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Le);
            case ExpressionType.GreaterThanOrEqual:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Geq);
            case ExpressionType.LessThanOrEqual:
                return MapExpressionToComparison((BinaryExpression)expr, Comparison.Leq);
            default:
                throw new InvalidOperationException("Expression not supported! Only Logical operators And, Or, Not and comparisons ==, !=, >, <, <= and >= are supported.");

        }
        return null;
    }


    private IConstraintNode MapExpressionToInfixOperator(BinaryExpression expr, InfixOperator infixOperator)
    {
        return new InfixConstraint
        {
            left = Map(expr.Left),
            infixOperator = infixOperator,
            right = Map(expr.Right)
        };
    }

    private IConstraintNode MapExpressionToPrefixOperator(UnaryExpression expr, PrefixOperator PrefixOperator)
    {
        return new PrefixConstraint
        {
            prefixOperator = PrefixOperator,
            constraint = Map(expr.Operand)
        };
    }

    private IConstraintNode MapExpressionToComparison(BinaryExpression expr, Comparison comparison)
    {
        if (!valueExpressionTypes.Contains(expr.Left.NodeType) || !valueExpressionTypes.Contains(expr.Right.NodeType))
        {
            throw new InvalidOperationException("Expressions other than a member access, method call or value reference not supported in a value comparison!");
        }

        var propertyNode = new PropertyConstraint
        {
            Left = MapMemberExpressionToValueNode(expr.Left),
            Comparison = comparison,
            Right = MapMemberExpressionToValueNode(expr.Right),
        };

        if (propertyNode.Left.GetType() != typeof(NodeProperty) && propertyNode.Right.GetType() != typeof(NodeProperty))
        {
            throw new InvalidOperationException($"Either Left hand side or right hand side of a constraint must reference the node!");
        }

        return propertyNode;
    }

    private IValue MapMemberExpressionToValueNode(Expression expr)
    {
        if (isPropertyExpression(expr, out string name))
        {
            return new NodeProperty
            {
                PropertyName = name
            };
        }
        return new Value
        {
            ValueString = getValueFromExpression(expr)
        };
    }

    private bool isPropertyExpression(Expression expr, out string name)
    {
        if (expr.NodeType == ExpressionType.MemberAccess)
        {
            var member = (MemberExpression)expr;
            if (member.Expression.NodeType == ExpressionType.Parameter)
            {
                name = member.Member.Name;
                return ((ParameterExpression)member.Expression).Name == parameterName;
            }
        }
        name = string.Empty;
        return false;
    }

    private string getValueFromExpression(Expression expr) // Expression can be used since its the base class for MemberExpression
    {
        switch (expr.NodeType)
        {
            case ExpressionType.MemberAccess:
                return getValueFromMemberExpression((MemberExpression)expr);
            case ExpressionType.Call:
                return getValueFromMethodCall((MethodCallExpression)expr);
            case ExpressionType.Constant:
                return getValueFromConstantExpression((ConstantExpression)expr);
            default:
                throw new InvalidOperationException("Something went wrong! Only a constant, variable, no argument function call or property access are allowed in a value comparison.");
        }
    }

    private string getValueFromMemberExpression(MemberExpression expr)
    {
        var o = Expression.Lambda(expr).Compile().DynamicInvoke();
        return Utils.TypeStringify(o);
    }

    private string getValueFromConstantExpression(ConstantExpression expr)
    {
        return Utils.TypeStringify(expr.Value);
    }

    private string getValueFromMethodCall(MethodCallExpression expr)
    {
        var o = Expression.Lambda(expr).Compile().DynamicInvoke();
        return Utils.TypeStringify(o);
    }   
}
