using System.Reflection;
using Grom.Entities.Attributes;
using Grom.GraphDbConnectors;
using Grom.Util.Exceptions;

namespace Grom.Entities.Relationships;

public class EntityDirectedRelationship
{
    private readonly GromGraphDbConnector _dbConnector;

    /*
     * EntityId is the entity id assigned by the graph database itself to the node and should always be unique.
     * This property is for internal use to know if the node has been created in the database for example.
     */
    internal long? EntityRelationshipId;

    public EntityNode Child { get; }

    public EntityDirectedRelationship(EntityNode child)
    {
        _dbConnector = GromGraph.GetDbConnector();
        Child = child;
        var properties = GetEntityProperties();
        foreach (var property in properties)
        {
            // TODO: make sure types are not nullable or support nullable properties
            if (!_dbConnector.GetSupportedTypes().Contains(property.PropertyType))
            {
                throw new RelationshipPropertyTypeNotSupportedException(property.PropertyType.Name, property.Name, GetType().Name);
            }

        }
    }

    internal async Task Persist(long? parentId)
    {
        await Child.Persist();
        var fld = GetEntityProperties();
        if (EntityRelationshipId is null)
        {
            if (!Child.EntityNodeId.HasValue || !parentId.HasValue)
            {
                // TODO: throw exception
                return;
            }
            EntityRelationshipId = await _dbConnector.CreateDirectedRelationship(this, fld, Child.EntityNodeId.Value, parentId.Value);
            return;
        }
        await _dbConnector.UpdateDirectedRelationship(this, fld);

    }

    // For now public to make deleting relationships more natural.
    public async Task Delete()
    {
        if (EntityRelationshipId is null)
        {
            return;
        }
        await _dbConnector.DeleteRelationship(EntityRelationshipId.Value);
        EntityRelationshipId = null; // To make sure code keeps working correctly

    }

    // TODO: check for optimization
    private IEnumerable<PropertyInfo> GetEntityProperties()
    {
        Type t = GetType();
        PropertyInfo[] fld = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return fld.Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(RelationshipProperty)));
    }
}
