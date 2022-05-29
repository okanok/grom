namespace Grom.Entities.Relationships;

public class DirectedRelationships<T> : List<T>, RelationshipCollection where T : EntityDirectedRelationship
{
    //private List<T> relationships = new List<T>();

    //public DirectedRelationships()
    //{

    //}

    //public void AddRelationship(T relationship) 
    //{
    //    relationships.Add(relationship);
    //}

    async Task RelationshipCollection.Persist(long? parentId)
    {
        foreach (var relationship in this)
        {
            await relationship.Persist(parentId);
        }
    }

    public new bool Remove(T item)
    {
        item.Delete().ConfigureAwait(false);
        return base.Remove(item);
    }

    public new int RemoveAll(Predicate<T> match)
    {
        var itemsToRemove = FindAll(match);
        foreach (var itemToRemove in itemsToRemove)
        {
            itemToRemove.Delete().ConfigureAwait(false);
        }
        return base.RemoveAll(match);
    }

    public new void RemoveAt(int index)
    {
        var itemToRemove = base[index];
        itemToRemove.Delete().ConfigureAwait(false);
        base.RemoveAt(index);
    }

    public new void RemoveRange(int index, int count)
    {
        var itemsToRemove = GetRange(index, count);
        foreach (var itemToRemove in itemsToRemove)
        {
            itemToRemove?.Delete().ConfigureAwait(false);
        }
        base.RemoveRange(index, count);
    }
}

