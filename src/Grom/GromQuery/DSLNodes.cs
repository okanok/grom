namespace Grom.GromQuery;

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
