using System.Reflection;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.Util;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Neo4J;

/// <summary>
/// Neo4J implementation of the GromGraphDbConnector class
/// </summary>
public class GromNeo4jConnector : GromGraphDbConnector
{
    private static readonly string CreateNodeQueryBase = "CREATE (n:{0} {{{1}, nodeIdentifier: '{2}'}}) RETURN n.nodeIdentifier as r;";
    private static readonly string UpdateNodeQueryBase = "MATCH (n:{0}) WHERE n.nodeIdentifier = '{1}' SET n = {{{2}, nodeIdentifier: '{1}'}};";
    private static readonly string DeleteNodeQueryBase = "MATCH (n) WHERE n.nodeIdentifier = '{0}' DETACH DELETE n;";
    private static readonly string CreateDirectedRelationshipQueryBase = "MATCH (a), (b) WHERE a.nodeIdentifier = '{0}' AND b.nodeIdentifier = '{1}' CREATE (a)-[r:{2} {{{3}, relationshipIdentifier: '{4}'}}]->(b) RETURN r.relationshipIdentifier as r";
    private static readonly string UpdateDirectedRelationshipQueryBase = "MATCH (a)-[r:{0}]->(b) WHERE r.relationshipIdentifier = '{1}' SET r = {{{2}, relationshipIdentifier: '{1}'}};";
    private static readonly string DeleteDirectedRelationshipQueryBase = "MATCH (a)-[r]->(b) WHERE r.relationshipIdentifier = '{0}' DELETE r";
    private static readonly string Query = "MATCH (n) WHERE {0} AND EXISTS(n.nodeIdentifier) OPTIONAL MATCH (n)-[r*]->(p) WHERE (ALL(rel in r WHERE EXISTS(rel.relationshipIdentifier))) AND EXISTS(p.nodeIdentifier) RETURN n, last(r) AS r, p";

    private readonly IDriver _driver;
    private readonly GromNeo4jQueryBuilder _gromNeo4JQueryBuilder;
    private readonly GromNeo4jResultMapper _gromNeo4JResultMapper;

    public GromNeo4jConnector(IDriver driver)
    {
        _gromNeo4JQueryBuilder = new GromNeo4jQueryBuilder();
        _gromNeo4JResultMapper = new GromNeo4jResultMapper();
        _driver = driver;
    }

    internal override async Task<Guid> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel)
    {
        var nodeIdentifier = Guid.NewGuid();
        var props = string.Join(", ", properties.Select(p => p.Name + ": " + Utils.TypeStringify(p.GetValue(node))));
        var query = string.Format(CreateNodeQueryBase, nodeLabel, props, nodeIdentifier);


        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);
            var guidString = await cursor.SingleAsync(r => r["r"].As<string>());

            return Utils.StringToGuid(guidString);
        } finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task DeleteNode(Guid nodeId)
    {
        var query = string.Format(DeleteNodeQueryBase, nodeId);

        var session = _driver.AsyncSession();
        try
        {
            await session.RunAsync(query);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, Guid nodeId)
    {
        // TODO: optimize retrieving properties and values
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(node))));
        var query = string.Format(UpdateNodeQueryBase, node.GetType().Name, nodeId, props);

        var session = _driver.AsyncSession();
        try
        {
            await session.RunAsync(query);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task<Guid> CreateDirectedRelationship(RelationshipBase relationship, IEnumerable<PropertyInfo> properties, Guid childNodeId, Guid parentNodeId)
    {
        var relationshipIdentifier = Guid.NewGuid();
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(relationship))));
        var query = string.Format(CreateDirectedRelationshipQueryBase, parentNodeId, childNodeId, relationship.GetType().Name, props, relationshipIdentifier);

        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);
            var guidString = await cursor.SingleAsync(r => r["r"].As<string>());

            return Utils.StringToGuid(guidString);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task UpdateDirectedRelationship(RelationshipBase relationship, IEnumerable<PropertyInfo> properties)
    {
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(relationship))));
        var query = string.Format(UpdateDirectedRelationshipQueryBase, relationship.GetType().Name, relationship.EntityRelationshipId, props);

        var session = _driver.AsyncSession();
        try
        {
            await session.RunAsync(query);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task DeleteRelationship(Guid relationshipId)
    {
        var query = string.Format(DeleteDirectedRelationshipQueryBase, relationshipId);

        var session = _driver.AsyncSession();
        try
        {
            await session.RunAsync(query);
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task<T> GetSingleNode<T>(IConstraintNode state)
    {
        var constraints = _gromNeo4JQueryBuilder.BuildQuery(state);
        var query = string.Format(Query, constraints);


        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);

            try
            {
                var result = await cursor.ToListAsync();
                return _gromNeo4JResultMapper.Map<T>(result); // Can be null if result is not found
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements"))
            {
                return null;
            }
        }
        finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task<IEnumerable<T>> GetNodes<T>(IConstraintNode state)
    {
        var constraints = _gromNeo4JQueryBuilder.BuildQuery(state);
        var query = string.Format(Query, constraints);

        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);

            try
            {
                var result = (await cursor.ToListAsync());
                return _gromNeo4JResultMapper.MapMultiple<T>(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Sequence contains no elements"))
            {
                return new List<T>();
            }
        }
        finally
        {
            await session.CloseAsync();
        }
    }
}
