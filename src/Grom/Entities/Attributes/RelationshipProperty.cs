namespace Grom.Entities.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RelationshipProperty : Attribute
{
    internal string? dbPropertyName { get; } = null;

    /// <summary>
    /// Tells Grom that a property should be mapped to a database node
    /// </summary>
    /// <param name="dbPropertyName">This will specify the name of the property in the database. Null means that the name of the property will be used.</param>
    public RelationshipProperty(string? dbPropertyName = null)
    {
        this.dbPropertyName = dbPropertyName;
    }
}
