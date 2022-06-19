using Grom.Entities.Attributes;
using Grom.GraphDbConnectors;
using Grom.Util.Exceptions;
using System.Reflection;

namespace Grom.Entities.Relationships;

public abstract class RelationshipBase
{
    private readonly GromGraphDbConnector _dbConnector;

    /*
     * EntityId is the entity id assigned by the graph database itself to the node and should always be unique.
     * This property is for internal use to know if the node has been created in the database for example.
     */
    internal long? EntityRelationshipId;

    public RelationshipBase()
    {
        _dbConnector = GromGraph.GetDbConnector();
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

    internal async Task Persist(long? childId, long? parentId)
    {
        var fld = GetEntityProperties();
        if (EntityRelationshipId is null)
        {
            if (!childId.HasValue || !parentId.HasValue)
            {
                throw new InvalidOperationException("Cannot create relationship when child or parent node is not created!");
            }
            EntityRelationshipId = await _dbConnector.CreateDirectedRelationship(this, fld, childId.Value, parentId.Value);
            return;
        }
        await _dbConnector.UpdateDirectedRelationship(this, fld);
    }

    // For now internal to force use of remove methods in RelationshipCollection.
    internal async Task Delete()
    {
        if (EntityRelationshipId is null)
        {
            return;
        }
        await _dbConnector.DeleteRelationship(EntityRelationshipId.Value);
        EntityRelationshipId = null; // To make sure code keeps working correctly

    }

    // TODO: check for optimization, use from Utils class
    private IEnumerable<PropertyInfo> GetEntityProperties()
    {
        Type t = GetType();
        PropertyInfo[] fld = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return fld.Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(RelationshipProperty)));
    }
}
