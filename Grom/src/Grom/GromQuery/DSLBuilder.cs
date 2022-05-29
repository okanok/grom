namespace Grom.GromQuery;

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
