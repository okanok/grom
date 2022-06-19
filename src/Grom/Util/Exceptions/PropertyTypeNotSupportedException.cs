namespace Grom.Util.Exceptions;

/// <summary>
/// Excpetion that is thrown when a property in a node is annotated with a type that is not supported by Grom
/// </summary>
[Serializable]
public class NodePropertyTypeNotSupportedException : NotSupportedException
{
    public NodePropertyTypeNotSupportedException(string type, string fieldName, string entityName)
        : base(string.Format("Type {0} not supported as property for field {1} in node {2}", type, fieldName, entityName))
    {
    }
}

/// <summary>
/// Excpetion that is thrown when a property in a relationship is annotated with a type that is not supported by Grom
/// </summary>
[Serializable]
public class RelationshipPropertyTypeNotSupportedException : NotSupportedException
{
    public RelationshipPropertyTypeNotSupportedException(string type, string fieldName, string entityName)
        : base(string.Format("Type {0} not supported as porperty for field {1} in relationship {2}", type, fieldName, entityName))
    {
    }
}
