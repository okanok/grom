using System.Reflection;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.GromQuery;

namespace Grom.GraphDbConnectors;

public abstract class GromGraphDbConnector
{
    private static readonly List<Type> SupportedTypes = new() { typeof(int), typeof(bool), typeof(string), typeof(float), typeof(long)};

    /// <summary>
    /// Creates the given entity as a node in the database
    /// </summary>
    /// <param name="node">the entity that will be created</param>
    /// <param name="properties">the properties of the entity</param>
    /// <param name="nodeLabel">the name of the label that should be given to the node</param>
    /// <returns>the unique entity id assigned by the database to the node</returns>
    internal abstract Task<long> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel);

    /// <summary>
    /// Deletes the node in the database including all relationships this node has.
    /// Note that the object itself will keep exisiting.
    /// </summary>
    /// <param name="nodeId"> The unique identifier assigned by the database to the node</param>
    internal abstract Task DeleteNode(long nodeId);

    /// <summary>
    /// Updates the given node with the properties
    /// </summary>
    /// <param name="node">the node that will be updated</param>
    /// <param name="properties">the properties of the node</param>
    /// <param name="nodeLabel">the label of the node</param>
    /// <param name="nodeId">the unique entity id of the node given by the database</param>
    /// <returns></returns>
    internal abstract Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, long nodeId);

    /// <summary>
    /// Creates the given directed relationship entity in the database
    /// </summary>
    /// <param name="relationship">the relationship entity that will be created</param>
    /// <param name="properties">the properties of the entity that are added</param>
    /// <param name="childNodeId">the unique id of the parent of the relationship, child should already exist in the database</param>
    /// <param name="parentNodeId">the unique id of the child of the relationship, parent should already exist in the database</param>
    /// <returns>the unique eneity id assigned to the relationship by the database</returns>
    internal abstract Task<long> CreateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties, long childNodeId, long parentNodeId);

    /// <summary>
    /// Updates the relationship in the databse to be the same as given relationship entity 
    /// </summary>
    /// <param name="relationship">the entity that will be updated</param>
    /// <param name="properties">the properties of the entity that needs to be updated</param>
    /// <returns></returns>
    internal abstract Task UpdateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties);

    /// <summary>
    /// Deletes the relationship from the database
    /// </summary>
    /// <param name="relationshipId">the unique relationship id used to identify the relationship</param>
    /// <returns></returns>
    internal abstract Task DeleteRelationship(long relationshipId);

    /// <summary>
    /// Returns a list of the types supported by Grom
    /// </summary>
    /// <returns></returns>
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
