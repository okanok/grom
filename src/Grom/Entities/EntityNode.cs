using System.Reflection;
using Grom.Entities.Relationships;
using Grom.GraphDbConnectors;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom.Entities;

public abstract class EntityNode
{
    private readonly Type cachedEntityType;
    private readonly IEnumerable<PropertyInfo> cachedEntityProperties;

    /// <summary>
    /// EntityId is the entity id assigned by Grom itself to the node and should always be unique.
    /// This property is for internal use to identify entities in the database and link them to objects in code.
    /// If the Guid is null then the object has not been created yet in the database.
    /// </summary>
    internal Guid? EntityNodeId;

    public EntityNode()
    {
        cachedEntityType = GetType();
        cachedEntityProperties = Utils.GetEntityProperties(cachedEntityType);
    }

    /// <summary>
    /// Persists the object instance that inherits from this class. If it allready exists in the database the node will be updated.
    /// Node Label will be the name of the class and properties will be the ones with the NodeProperty attribute.
    /// </summary>
    /// <returns></returns>
    public async Task Persist()
    {
        if (!EntityNodeId.HasValue)
        {
            EntityNodeId = await GromGraph.GetDbConnector().CreateNode(this, cachedEntityProperties, cachedEntityType.Name);
        }
        else
        {
            await GromGraph.GetDbConnector().UpdateNode(this, cachedEntityProperties, cachedEntityType.Name, EntityNodeId.Value);
        }

        // Persist relationships
        var relationships = GetRelationshipFields();
        foreach (var relationship in relationships)
        {
            await relationship.Persist(EntityNodeId.Value);
        }
    }

    /// <summary>
    /// Deletes the node in the database if it exists. Grom can only know this if the node has been persisted or retrieved with Retrieve<T>.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteNode()
    {
        if (EntityNodeId is null)
        {
            // Cant delete if entity does not exist
            return;
        }
        await GromGraph.GetDbConnector().DeleteNode(EntityNodeId.Value);
        EntityNodeId = null; // To make sure code keeps working correctly

    }

    // TODO: check for optimisation
    private IEnumerable<IRelationshipCollection> GetRelationshipFields()
    {
        return cachedEntityType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => typeof(IRelationshipCollection).IsAssignableFrom(x.PropertyType))
            .Select(x => x.GetValue(this).As<IRelationshipCollection>());

    }
}
