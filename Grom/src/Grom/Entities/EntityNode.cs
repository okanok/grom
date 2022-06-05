using System.Reflection;
using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.GraphDbConnectors;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;


namespace Grom.Entities;

public abstract class EntityNode
{
    private readonly GromGraphDbConnector _dbConnector;
    private readonly Type cachedTypeObject; //TODO: rename
    private readonly IEnumerable<PropertyInfo> cachedProperties;

    /// <summary>
    /// EntityId is the entity id assigned by the graph database itself to the node and should always be unique.
    /// This property is for internal use to know if the node has been created in the database.
    /// </summary>
    internal long? EntityNodeId;

    public EntityNode()
    {
        _dbConnector = GromGraph.GetDbConnector();
        cachedTypeObject = GetType();
        cachedProperties = Utils.GetEntityProperties(cachedTypeObject);
        foreach (var property in cachedProperties)
        {
            // TODO: make sure types are not nullable or support nullable properties
            if (!_dbConnector.GetSupportedTypes().Contains(property.PropertyType))
            {
                throw new NodePropertyTypeNotSupportedException(property.PropertyType.Name, property.Name, GetType().Name);
            }

        }
    }

    /// <summary>
    /// Persists the object instance that inherits from this class. If it allready exists in the database the node will be updated.
    /// Node Label will be the name of the class and properties will be the ones with the NodeProperty attribute.
    /// </summary>
    /// <returns></returns>
    public async Task Persist()
    {
        if (EntityNodeId == null)
        {
            EntityNodeId = await _dbConnector.CreateNode(this, cachedProperties, cachedTypeObject.Name);
        }
        else
        {
            await _dbConnector.UpdateNode(this, cachedProperties, cachedTypeObject.Name, EntityNodeId.Value);
        }

        // Persist relationships
        var relationships = GetRelationshipFields();
        foreach (var relationship in relationships)
        {
            await relationship.Persist(EntityNodeId);
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
        await _dbConnector.DeleteNode(EntityNodeId.Value);
        EntityNodeId = null; // To make sure code keeps working correctly

    }

    // TODO: check for optimisation
    private IEnumerable<RelationshipCollection> GetRelationshipFields()
    {
        return cachedTypeObject
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => typeof(RelationshipCollection).IsAssignableFrom(x.FieldType))
            .Select(x => x.GetValue(this).As<RelationshipCollection>());

    }
}
