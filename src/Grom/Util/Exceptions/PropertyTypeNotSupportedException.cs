namespace Grom.Util.Exceptions;

/// <summary>
/// Excpetion that is thrown when a property in a node or relationship is annotated with a type that is not supported by Grom
/// </summary>
[Serializable]
public class PropertyTypeNotSupportedException : NotSupportedException
{
    public PropertyTypeNotSupportedException(string type) : base(string.Format("Type {0} not supported as porperty!", type))
    {
    }
}
