using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grom.Entities.Relationships;

public class RelationshipCollection<T1, T2> : List<RelationshipItem<T1, T2>>, IRelationshipCollection where T1 : RelationshipBase  where T2 : EntityNode
{
    /// <summary>
    /// Persists all relationships, if the relationship exists the relationship will be updated
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    async Task IRelationshipCollection.Persist(Guid parentId)
    {
        foreach (var relationshipItem in this)
        {
            await relationshipItem.Node.Persist();
            await relationshipItem.Relationship.Persist(relationshipItem.Node.EntityNodeId!.Value, parentId);
        }
    }

    public void Add(T1 relationship, T2 node)
    {
        //TODO: check if relationship allready exists either throw an error or silenty dont add
        Add(new RelationshipItem<T1, T2>(relationship, node));
    }

    void IRelationshipCollection.Add(RelationshipBase relationship, EntityNode node)
    {
        Add(new RelationshipItem<T1, T2>((T1)relationship, (T2)node));
    }

    /// <summary>
    /// Same as Remove in List and relationship is removed from the database
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public new bool Remove(RelationshipItem<T1, T2> item)
    {
        item.Relationship.Delete().Wait(); //TODO: check if this works
        return base.Remove(item);
    }

    /// <summary>
    /// Same as RemoveAt as List and the relationship is removed from the database
    /// </summary>
    /// <param name="index"></param>
    public new void RemoveAt(int index)
    {
        var itemToRemove = base[index];
        itemToRemove.Relationship.Delete().ConfigureAwait(false); //TODO: check if this works
        base.RemoveAt(index);
    }

    /// <summary>
    /// Same as RemoveRange as List and the relationships are removed from the database
    /// </summary>
    /// <param name="index"></param>
    public new void RemoveRange(int index, int count)
    {
        var itemsToRemove = GetRange(index, count); //TODO: copies the range, in place delete is also possible.
        foreach (var itemToRemove in itemsToRemove)
        {
            itemToRemove?.Relationship.Delete().ConfigureAwait(false);
        }
        base.RemoveRange(index, count);
    }
}

public class RelationshipItem<T1, T2> where T1 : RelationshipBase  where T2 : EntityNode
{
    public readonly T1 Relationship;
    public readonly T2 Node;

    public RelationshipItem(T1 relationship, T2 node)
    {
        Relationship = relationship;
        Node = node;
    }

    public async Task Delete()
    {
        await Relationship.Delete();
    }
}
