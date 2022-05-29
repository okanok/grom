using Grom.Util;

namespace Grom.QueryDSL;

enum InfixOperator
{
    And,
    Or
}

public enum PrefixOperator
{
    Not
}

public enum Comparison
{
    Eq,     // ==
    Neq,    // !=
    Ge,     // >
    Le,     // <
    Geq,    // >=
    Leq     // <=
}
public interface IConstraintNode
{
}

struct InfixConstraint : IConstraintNode
{
    internal IConstraintNode left;
    internal InfixOperator infixOperator;
    internal IConstraintNode right;
}

struct PrefixConstraint : IConstraintNode
{
    internal PrefixOperator prefixOperator;
    internal IConstraintNode constraint;
}

struct PropertyConstraint : IConstraintNode
{
    internal IValue Left;
    internal Comparison Comparison;
    internal IValue Right;
}

public interface IValue
{
}

struct NodeProperty : IValue
{
    internal string PropertyName;
}

struct Value : IValue
{
    internal string ValueString;
}


// Query AST builder code
public static class Logical
{
    public static IConstraintNode And(IConstraintNode constrLeft, IConstraintNode constrRight)
    {
        var c = new InfixConstraint
        {
            left = constrLeft,
            infixOperator = InfixOperator.And,
            right = constrRight
        };
        return c;
    }

    public static IConstraintNode Or(IConstraintNode constrLeft, IConstraintNode constrRight)
    {
        var c = new InfixConstraint
        {
            left = constrLeft,
            infixOperator = InfixOperator.Or,
            right = constrRight
        };
        return c;
    }

    public static IConstraintNode Not(IConstraintNode constr)
    {
        var c = new PrefixConstraint
        {
            prefixOperator = PrefixOperator.Not,
            constraint = constr
        };
        return c;
    }
}

public static class Constraint
{
    public static IConstraintNode Property(IValue left, Comparison comparison, IValue right)
    {
        var c = new PropertyConstraint
        {
            Left = left,
            Comparison = comparison,
            Right = right
        };
        return c; 
    }
}