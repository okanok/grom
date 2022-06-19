﻿using System.Reflection;
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
    private static readonly string CreateNodeQueryBase = "CREATE (n:{0} {{{1}}}) RETURN id(n) as r;";
    private static readonly string UpdateNodeQueryBase = "MATCH (n:{0}) WHERE id(n) = {1} SET n = {{{2}}};";
    private static readonly string DeleteNodeQueryBase = "MATCH (a) WHERE id(a) = {0} DETACH DELETE a;";
    private static readonly string CreateDirectedRelationshipQueryBase = "MATCH (a), (b) WHERE id(a) = {0} AND id(b) = {1} CREATE (a)-[r:{2} {{{3}}}]->(b) RETURN id(r) as r";
    private static readonly string UpdateDirectedRelationshipQueryBase = "MATCH (a)-[r:{0}]->(b) WHERE id(r) = {1} SET r = {{{2}}};";
    private static readonly string DeleteDirectedRelationshipQueryBase = "MATCH (a)-[r]->(b) WHERE id(r) = {0} DELETE r";
    private static readonly string Query = "MATCH (n) WHERE {0} OPTIONAL MATCH (n)-[r*]->(p) RETURN n, last(r) AS r, p";

    private readonly IDriver _driver;
    private readonly GromNeo4jQueryBuilder _gromNeo4JQueryBuilder;
    private readonly GromNeo4jResultMapper _gromNeo4JResultMapper;

    public GromNeo4jConnector(IDriver driver)
    {
        _gromNeo4JQueryBuilder = new GromNeo4jQueryBuilder(); //TODO: make static?
        _gromNeo4JResultMapper = new GromNeo4jResultMapper(); //TODO: make static?
        _driver = driver;
    }

    internal override async Task<long> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel)
    {
        var props = string.Join(", ", properties.Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(node))));
        var query = string.Format(CreateNodeQueryBase, nodeLabel, props);


        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);
            return await cursor.SingleAsync(r => r["r"].As<long>());
        } finally
        {
            await session.CloseAsync();
        }
    }

    internal override async Task DeleteNode(long nodeId)
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

    internal override async Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, long nodeId)
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

    internal override async Task<long> CreateDirectedRelationship(RelationshipBase relationship, IEnumerable<PropertyInfo> properties, long childNodeId, long parentNodeId)
    {
        var props = string.Join(", ", properties.ToList().Select(x => x.Name + ": " + Utils.TypeStringify(x.GetValue(relationship))));
        var query = string.Format(CreateDirectedRelationshipQueryBase, parentNodeId, childNodeId, relationship.GetType().Name, props);

        var session = _driver.AsyncSession();
        try
        {
            var cursor = await session.RunAsync(query);
            return await cursor.SingleAsync(r => r["r"].As<long>());
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

    internal override async Task DeleteRelationship(long relationshipId)
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
            catch (InvalidOperationException ex) //TODO: do this better
            {
                if (ex.Message.Equals("Sequence contains no elements"))
                {
                    return null;
                }
                throw ex;
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
            catch (InvalidOperationException ex) //TODO: do this better
            {
                if (ex.Message.Contains("Sequence contains no elements"))
                {
                    return new List<T>();
                }
                throw ex;
            }
        }
        finally
        {
            await session.CloseAsync();
        }
    }
}
