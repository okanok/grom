using Grom.Entities.Attributes;
using Grom.GraphDbConnectors;
using Grom.Util;
using Grom.Util.Exceptions;
using System.Reflection;

namespace Grom.Entities.Relationships;

public abstract class RelationshipBase
{
    private readonly GromGraphDbConnector _dbConnector;
    private readonly IEnumerable<PropertyInfo> _cachedRelationshipProperties;

    /// <summary>
    ///  EntityRelationshipId is the entity id assigned by Grom itself to the node and should always be unique.
    ///  This property is for internal use to identify entities in the database and link them to objects in code.
    ///  If the Guid is null then the object has not been created yet in the database.
    /// </summary>
    internal Guid? EntityRelationshipId;

    public RelationshipBase()
    {
        _dbConnector = GromGraph.GetDbConnector();
        _cachedRelationshipProperties = Utils.GetRelationshipProperties(GetType());
    }

    /// <summary>
    /// Updates the properties of only the relationship. The parent, child and other relationships will be ignored.
    /// Does not work if relationship has not been created already.
    /// </summary>
    /// <returns></returns>
    public async Task UpdateRelationshipOnly()
    {
        if(EntityRelationshipId is not null)
        {
            await _dbConnector.UpdateDirectedRelationship(this, _cachedRelationshipProperties);
        }
    }

    internal async Task Persist(Guid childId, Guid parentId)
    {
        if (EntityRelationshipId is null)
        {
            EntityRelationshipId = await _dbConnector.CreateDirectedRelationship(this, _cachedRelationshipProperties, childId, parentId);
            return;
        }
        await _dbConnector.UpdateDirectedRelationship(this, _cachedRelationshipProperties);
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
}
