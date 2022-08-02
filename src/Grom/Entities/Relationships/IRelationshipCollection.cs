namespace Grom.Entities.Relationships;

internal interface IRelationshipCollection
{
    internal abstract Task Persist(Guid parentId);

    internal void Add(RelationshipBase relationship, EntityNode node);
}
