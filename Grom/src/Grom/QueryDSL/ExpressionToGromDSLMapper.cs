using Grom.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Grom.QueryDSL;

internal class ExpressionToGromDSLMapper<T>
{
    private readonly ExpressionType[] valueExpressionTypes = { ExpressionType.MemberAccess, ExpressionType.Constant };

    private string parameterName;
    private Expression<Func<T, bool>> expr;

    internal ExpressionToGromDSLMapper(Expression<Func<T, bool>> expr)
    {
        this.expr = expr;
        parameterName = expr.Parameters.First().Name;//TODO: make more robust (check stuff)
    }

    internal IConstraintNode Map()
    {
        return Map(expr.Body);//TODO: check before cast
    }

    //internal IConstraintNode Map(BinaryExpression expr)
    //{
    //    switch(expr.NodeType)
    //    {
    //        case ExpressionType.AndAlso:
    //            return MapExpressionToInfixOperator(expr, InfixOperator.And);
    //        case ExpressionType.Or:
    //            return MapExpressionToInfixOperator(expr, InfixOperator.Or);
    //        default:
    //            throw new InvalidOperationException("Expression not supported!"); //TODO: make better add reference to actual unsupported thing.
    //    }
    //    return null;
    //}

    internal IConstraintNode Map(Expression expr)
    {
        switch(expr.NodeType)
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
                throw new InvalidOperationException("Expression not supported!"); //TODO: make better add reference to actual unsupported thing.

        }
        return null;
    }

    internal IConstraintNode Map(MemberExpression expr)
    {
        switch (expr.NodeType)
        {
            
            default:
                throw new InvalidOperationException("Expression not supported!"); //TODO: make better add reference to actual unsupported thing.

        }
        return null;
    }

    //internal IConstraintNode Map(FieldExpression expr)
    //{
    //    switch (expr.NodeType)
    //    {

    //        default:
    //            throw new InvalidOperationException("Expression not supported!"); //TODO: make better add reference to actual unsupported thing.

    //    }
    //    return null;
    //}

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
        if( !valueExpressionTypes.Contains(expr.Left.NodeType) || !valueExpressionTypes.Contains(expr.Right.NodeType))
        {
            throw new InvalidOperationException("Something went wrong!");//TODO: make better message 
        }

        var propertyNode = new PropertyConstraint
        {
            Left = MapMemberExpressionToValueNode(expr.Left),
            Comparison = comparison,
            Right = MapMemberExpressionToValueNode(expr.Right),
        };

        if(propertyNode.Left.GetType() != typeof(NodeProperty) && propertyNode.Right.GetType() != typeof(NodeProperty))
        {
            throw new InvalidOperationException($"Either Left hand side or right hand side of a constraint must reference the node!"); //TODO: make better message 
        }

        return new PropertyConstraint
        {
            Left = MapMemberExpressionToValueNode(expr.Left),
            Comparison = comparison,
            Right = MapMemberExpressionToValueNode(expr.Right),
        };
    }

    private IValue MapMemberExpressionToValueNode(Expression expr)
    {
        if(isPropertyExpression(expr, out string name))
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
        if(expr.NodeType == ExpressionType.MemberAccess)
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
        switch(expr.NodeType)
        {
            case ExpressionType.MemberAccess:
                return getValueFromMemberExpression((MemberExpression)expr);
            case ExpressionType.Constant:
                return getValueFromConstantExpression((ConstantExpression)expr);
            default:
                throw new InvalidOperationException("Something went wrong!");//TODO: make better message
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
}
