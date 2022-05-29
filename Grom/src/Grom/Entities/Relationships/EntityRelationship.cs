using System.Reflection;
using Grom.Entities.Attributes;
using Grom.GraphDbConnectors;
using Grom.Util.Exceptions;

namespace Grom.Entities.Relationships;

// TODO: remove or actually use for polymorphic relationships
public class EntityRelationship
{

    private readonly GromGraphDbConnector _dbConnector;

    /*
     * EntityId is the entity id assigned by the graph database itself to the node and should always be unique.
     * This property is for internal use to know if the node has been created in the database for example.
     */
    internal long? EntityRelationshipId;

    public EntityRelationship()
    {
        _dbConnector = GromGraph.GetDbConnector();
        var flds = GetEntityFields();
        foreach (var fld in flds)
        {
            // TODO: make sure types are not nullable or support nullable fields
            if (!_dbConnector.GetSupportedTypes().Contains(fld.FieldType))
            {
                throw new RelationshipPropertyTypeNotSupportedException(fld.FieldType.Name, fld.Name, GetType().Name);
            }

        }
    }

    internal void PersistUndirected(EntityNode a, EntityNode b)
    {
        // TODO: support undirected relationships
    }

    internal async Task PersistDirected(EntityNode child, EntityNode parent)
    {
        var fld = GetEntityFields();
        if (EntityRelationshipId == null)
        {
            if (!child.EntityNodeId.HasValue || !parent.EntityNodeId.HasValue)
            {
                // TODO: throw exception
            }
            //EntityRelationshipId = await _dbConnector.CreateDirectedRelationship(this, fld, child.EntityNodeId.Value, parent.EntityNodeId.Value);
        }
        else
        {
            //await _dbConnector.UpdateDirectedRelationship(this, fld);
        }
    }

    private IEnumerable<FieldInfo> GetEntityFields()
    {
        Type t = GetType();
        FieldInfo[] fld = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

        return fld.Where(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(RelationshipProperty)));
    }
}
