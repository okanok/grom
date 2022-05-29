using System.Reflection;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.Util;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Neo4J;

public class GromNeo4jConnector : GromGraphDbConnector
{
    private static readonly string CreateNodeQueryBase = "CREATE (n:{0} {{{1}}}) RETURN id(n) as r;";
    private static readonly string UpdateNodeQueryBase = "MATCH (n:{0}) WHERE id(n) = {1} SET n = {{{2}}};";
    private static readonly string DeleteNodeQueryBase = "MATCH (a) WHERE id(a) = {0} OPTIONAL MATCH (a)-[r1]->(b1), (a)<-[r2]-(b2), (a)-[r3]-(b3) DELETE r1, r2, r3, a;";
    private static readonly string CreateDirectedRelationshipQueryBase = "MATCH (a), (b) WHERE id(a) = {0} AND id(b) = {1} CREATE (a)-[r:{2} {{{3}}}]->(b) RETURN id(r) as r";
    private static readonly string UpdateDirectedRelationshipQueryBase = "MATCH (a)-[r:{0}]->(b) WHERE id(r) = {1} SET r = {{{2}}};";
    private static readonly string DeleteDirectedRelationshipQueryBase = "MATCH (a)-[r]->(b) WHERE id(r) = {0} DELETE r";
    private static readonly string Query = "MATCH (n) WHERE {0} RETURN n";

    private readonly IDriver _driver;
    private readonly GromNeo4jQueryBuilder _gromNeo4JQueryBuilder;
    private readonly GromNeo4jResultMapper _gromNeo4JResultMapper;

    // TODO: support multiple ways to authenticate
    public GromNeo4jConnector(string uri, string username, string password)
    {
        _gromNeo4JQueryBuilder = new GromNeo4jQueryBuilder();
        _gromNeo4JResultMapper = new GromNeo4jResultMapper();
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
    }

    internal override async Task<long> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel)
    {
        var props = string.Join(", ", properties.Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(node))));
        var query = string.Format(CreateNodeQueryBase, nodeLabel, props);

        await using var session = _driver.AsyncSession();
        return await session.RunAsync(query).Result.SingleAsync(r => r["r"].As<long>());
    }

    internal override async Task DeleteNode(long nodeId)
    {
        var query = string.Format(DeleteNodeQueryBase, nodeId);

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query);
    }

    internal override async Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, long nodeId)
    {
        // TODO: optimize retrieving properties and values
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(node))));
        var query = string.Format(UpdateNodeQueryBase, node.GetType().Name, nodeId, props);

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query);
    }

    internal override async Task<long> CreateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties, long childNodeId, long parentNodeId)
    {
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(relationship))));
        var query = string.Format(CreateDirectedRelationshipQueryBase, parentNodeId, childNodeId, relationship.GetType().Name, props);

        await using var session = _driver.AsyncSession();
        var relationshipId = await session.RunAsync(query).Result.SingleAsync(r => r["r"].As<long>());

        return relationshipId;
    }

    internal override async Task UpdateDirectedRelationship(EntityDirectedRelationship relationship, IEnumerable<PropertyInfo> properties)
    {
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(relationship))));
        var query = string.Format(UpdateDirectedRelationshipQueryBase, relationship.GetType().Name, relationship.EntityRelationshipId, props);

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query);
    }

    internal override async Task DeleteRelationship(long relationshipId)
    {
        var query = string.Format(DeleteDirectedRelationshipQueryBase, relationshipId);

        await using var session = _driver.AsyncSession();
        await session.RunAsync(query);
    }

    internal override async Task<T> GetSingleNode<T>(IConstraintNode state)
    {
        var constraints = _gromNeo4JQueryBuilder.BuildQuery(state);
        var query = string.Format(Query, constraints);

        await using var session = _driver.AsyncSession();
        var cursor = await session.RunAsync(query);

        try
        {
            var result = (await cursor.SingleAsync());
            return _gromNeo4JResultMapper.Map<T>(result);

        } catch (InvalidOperationException ex)
        {
            if (ex.Message.Equals("The result is empty."))
            {
                return null;
            }
            throw ex;
        }
    }

    internal override Task<IEnumerable<T>> GetNodes<T>(IConstraintNode state)
    {
        throw new NotImplementedException();
    }
}
