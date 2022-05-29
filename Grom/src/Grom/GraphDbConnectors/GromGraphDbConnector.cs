using System.Reflection;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.GromQuery;

namespace Grom.GraphDbConnectors;

public abstract class GromGraphDbConnector
{
    private static readonly List<Type> SupportedTypes = new() { typeof(int), typeof(bool), typeof(string), typeof(float), typeof(long)};

    internal abstract Task<long> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel);

    /// <summary>
    /// Deletes the node in the database including all relationships this node has.
    /// Note that the object itself will keep exisiting.
    /// </summary>
    /// <param name="nodeId"> The unique identifier assigned by the database engine to the node</param>
    internal abstract Task DeleteNode(long nodeId);

    internal abstract Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, long nodeId);

    //internal abstract Task<long> CreateDirectedRelationship(EntityRelationship relationship, IEnumerable<FieldInfo> fields, long childNodeId, long parentNodeId);
    internal abstract Task<long> CreateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties, long childNodeId, long parentNodeId);

    //internal abstract Task UpdateDirectedRelationship(EntityRelationship relationship, IEnumerable<FieldInfo> fields);
    internal abstract Task UpdateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties);

    internal abstract Task DeleteRelationship(long relationshipId);

    /// <summary>
    /// Returns a list of the types supported by Grom
    /// </summary>
    /// <returns>list of supported types</returns>
    internal List<Type> GetSupportedTypes()
    {
        return SupportedTypes;
    }

    /// <summary>
    /// Retrieves a single node from the graph database
    /// </summary>
    /// <typeparam name="T"> The node that will be retrieved </typeparam>
    /// <param name="state"> A state object containing all filter information needed to build a query </param>
    /// <returns>null if no result found</returns>
    internal abstract Task<T?> GetSingleNode<T>(IConstraintNode state) where T : EntityNode;

    /// <summary>
    /// Retrieves all nodes from the graph database matching the given filters
    /// </summary>
    /// <typeparam name="T"> The node that will be retrieved </typeparam>
    /// <param name="state"> A state object containing all filter information needed to build a query</param>
    /// <returns></returns>
    internal abstract Task<IEnumerable<T>> GetNodes<T>(IConstraintNode state) where T : EntityNode;

}
