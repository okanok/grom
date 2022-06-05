namespace Grom.Entities.Relationships;

// TODO: rename to DirectedRelationshipsCollection
public class DirectedRelationships<T> : List<T>, RelationshipCollection where T : EntityDirectedRelationship
{

    /// <summary>
    /// Persists all relationships, if the relationship exists the relationship will be updated
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    async Task RelationshipCollection.Persist(long? parentId)
    {
        foreach (var relationship in this)
        {
            await relationship.Persist(parentId);
        }
    }

    /// <summary>
    /// Same as Remove in List and relationship is removed from the database
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public new bool Remove(T item)
    {
        item.Delete().ConfigureAwait(false);
        return base.Remove(item);
    }

    /// <summary>
    /// Same as RemoveAt as List and the relationship is removed from the database
    /// </summary>
    /// <param name="index"></param>
    public new void RemoveAt(int index)
    {
        var itemToRemove = base[index];
        itemToRemove.Delete().ConfigureAwait(false);
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
            itemToRemove?.Delete().ConfigureAwait(false);
        }
        base.RemoveRange(index, count);
    }
}

