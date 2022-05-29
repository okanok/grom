using System.Reflection;
using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.GraphDbConnectors;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;


namespace Grom.Entities;

public class EntityNode
{
    private readonly GromGraphDbConnector _dbConnector;
    private readonly Type t; 
    private readonly IEnumerable<PropertyInfo> properties;

    /*
     * EntityId is the entity id assigned by the graph database itself to the node and should always be unique.
     * This property is for internal use to know if the node has been created in the database for example.
     */
    internal long? EntityNodeId;

    public EntityNode()
    {
        _dbConnector = GromGraph.GetDbConnector();
        t = GetType();
        properties = Utils.GetEntityProperties(t);
        foreach (var property in properties)
        {
            // TODO: make sure types are not nullable or support nullable properties
            if (!_dbConnector.GetSupportedTypes().Contains(property.PropertyType))
            {
                throw new NodePropertyTypeNotSupportedException(property.PropertyType.Name, property.Name, GetType().Name);
            }

        }
    }

    /*
     * Persists the node and all its ancestors and predecessors in the database.
     * Node label will be the name of the class and the properties will be the properties with the NodeProperty attribute.
     * Based on if EntityId is null or not this method knows if the node has to be updated or created.
     */
    public async Task Persist()
    {
        if (EntityNodeId == null)
        {
            EntityNodeId = await _dbConnector.CreateNode(this, properties, t.Name);
        }
        else
        {
            await _dbConnector.UpdateNode(this, properties, t.Name, EntityNodeId.Value);
        }

        // Persist relationships
        var relationships = GetRelationshipFields();
        foreach (var relationship in relationships)
        {
            await relationship.Persist(EntityNodeId);
        }
    }

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
        return t
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(x => typeof(RelationshipCollection).IsAssignableFrom(x.FieldType))
            .Select(x => x.GetValue(this).As<RelationshipCollection>());

    }
}
