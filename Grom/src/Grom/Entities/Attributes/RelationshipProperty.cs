namespace Grom.Entities.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RelationshipProperty : Attribute
{
    public RelationshipProperty()
    {

    }
}
