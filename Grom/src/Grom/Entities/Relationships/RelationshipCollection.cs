namespace Grom.Entities.Relationships;

internal interface RelationshipCollection
{
    internal abstract Task Persist(long? parentId);
}
