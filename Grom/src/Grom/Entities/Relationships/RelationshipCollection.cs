namespace Grom.Entities.Relationships;

public interface RelationshipCollection
{
    internal abstract Task Persist(long? parentId);
}
