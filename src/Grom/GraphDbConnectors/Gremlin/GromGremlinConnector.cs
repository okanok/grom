using Gremlin.Net.Driver;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.GromQuery;
using Grom.Util;
using System.Reflection;

namespace Grom.GraphDbConnectors.Gremlin;

public class GromGremlinConnector : GromGraphDbConnector
{
    private static readonly string CreateNodeQueryBase = "g.addV('{0}').property('nodeIdentifier','{1}'){2}.values('nodeIdentifier')";
    private static readonly string UpdateNodeQueryBase = "g.V().has('nodeIdentifier', '{0}'){1}.values('nodeIdentifier')";
    private static readonly string DeleteNodeQueryBase = "g.V().has('nodeIdentifier', '{0}').drop().iterate()";
    private static readonly string CreateDirectedRelationshipQueryBase = "g.V().has('nodeIdentifier', '{0}').as('c').V().has('nodeIdentifier', '{1}').as('p').addE('{2}').property('relationshipIdentifier','{3}'){4}.from('p').to('c').iterate()";
    private static readonly string UpdateDirectedRelationshipQueryBase = "g.E().has('relationshipIdentifier', '{0}'){1}.iterate()";
    private static readonly string DeleteDirectedRelationshipQueryBase = "g.E().has('relationshipIdentifier', '{0}').drop().iterate()";
    private static readonly string QueryNodeOnly = "g.V().hasLabel('{0}').has('nodeIdentifier'){1}.project('p').by(valueMap().with(WithOptions.tokens,WithOptions.labels)).toList()";
    private static readonly string QueryNodeWithRelationships = "g.V().hasLabel('{0}').has('nodeIdentifier'){1}.outE().emit().repeat(inV().outE().as('c').dedup()).project('p','r','c').by(outV().valueMap().with(WithOptions.tokens,WithOptions.labels)).by(valueMap().with(WithOptions.tokens,WithOptions.labels)).by(inV().valueMap().with(WithOptions.tokens,WithOptions.labels)).toList()";

    private readonly IQueryBuilder _gremlinQueryBuilder;
    private readonly GremlinResultMapper _gremlinResultMapper;
    private readonly GremlinClient _gremlinClient;

    public GromGremlinConnector(GremlinClient gremlinClient)
    {
        _gremlinQueryBuilder = new GremlinQueryBuilder();
        _gremlinResultMapper = new GremlinResultMapper();
        _gremlinClient = gremlinClient;
    }


    internal override async Task<Guid> CreateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel)
    {
        var nodeIdentifier = Guid.NewGuid();
        var props = string.Join("", properties.Select(p => string.Format(".property({0})" ,Utils.StringifyProperty("'{0}',{1}", p, node))));
        var query = string.Format(CreateNodeQueryBase, nodeLabel, nodeIdentifier, props);

        var guidString = await _gremlinClient.SubmitWithSingleResultAsync<string>(query);
        return new Guid(guidString);
    }

    internal override async Task DeleteNode(Guid nodeId)
    {
        var query = string.Format(DeleteNodeQueryBase, nodeId);
        await _gremlinClient.SubmitAsync(query);
    }

    internal override async Task UpdateNode(EntityNode node, IEnumerable<PropertyInfo> properties, string nodeLabel, Guid nodeId)
    {
        var props = string.Join("", properties.Select(p => string.Format(".property({0})", Utils.StringifyProperty("'{0}',{1}", p, node))));
        var query = string.Format(UpdateNodeQueryBase, node.EntityNodeId, props);

        await _gremlinClient.SubmitAsync(query);
    }

    internal override async Task<Guid> CreateDirectedRelationship(RelationshipBase relationship, IEnumerable<PropertyInfo> properties, Guid childNodeId, Guid parentNodeId)
    {
        var relationshipIdentifier = Guid.NewGuid();
        var props = string.Join("", properties.Select(p => string.Format(".property({0})", Utils.StringifyProperty("'{0}',{1}", p, relationship))));
        var query = string.Format(CreateDirectedRelationshipQueryBase, childNodeId, parentNodeId, relationship.GetType().Name, relationshipIdentifier, props);

        await _gremlinClient.SubmitAsync(query);
        return relationshipIdentifier;
    }

    internal override async Task UpdateDirectedRelationship(RelationshipBase relationship, IEnumerable<PropertyInfo> properties)
    {
        var props = string.Join("", properties.Select(p => string.Format(".property({0})", Utils.StringifyProperty("'{0}',{1}", p, relationship))));
        var query = string.Format(UpdateDirectedRelationshipQueryBase, relationship.EntityRelationshipId, props);

        await _gremlinClient.SubmitAsync(query);
    }

    internal override async Task DeleteRelationship(Guid relationshipId)
    {
        var query = string.Format(DeleteDirectedRelationshipQueryBase, relationshipId);
        await _gremlinClient.SubmitAsync(query);
    }

    internal override async Task<T> GetSingleNode<T>(QueryState state)
    {
        var constraints = _gremlinQueryBuilder.BuildQuery(state.Query);
        var query = state.RetrieveRelationships
            ? string.Format(QueryNodeWithRelationships, state.RootNodeName, constraints)
            : string.Format(QueryNodeOnly, state.RootNodeName, constraints);

        var r = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query);
        var a = _gremlinResultMapper.Map<T>(r);

        // QueryNodeWithRelationships only finds nodes if they have any relationships.
        // if the node has no relationships but user still marked the Retrieve with RetrieveRelationships
        // We still need to use QueryNodeOnly
        if (a is null && state.RetrieveRelationships)
        {
            query = string.Format(QueryNodeOnly, state.RootNodeName, constraints);
            r = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query);
            a = _gremlinResultMapper.Map<T>(r);
        }
        return a; // can be null since query may find no results
    }

    internal override async Task<IEnumerable<T>> GetNodes<T>(QueryState state)
    {
        var constraints = _gremlinQueryBuilder.BuildQuery(state.Query);
        var query = state.RetrieveRelationships
            ? string.Format(QueryNodeWithRelationships, state.RootNodeName, constraints)
            : string.Format(QueryNodeOnly, state.RootNodeName, constraints);

        var r = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query);
        var a = _gremlinResultMapper.MapMultiple<T>(r);

        // QueryNodeWithRelationships only finds nodes if they have any relationships.
        // if the node has no relationships but user still marked the Retrieve with RetrieveRelationships
        // We still need to use QueryNodeOnly
        if (!a.Any() && state.RetrieveRelationships)
        {
            query = string.Format(QueryNodeOnly, state.RootNodeName, constraints);
            r = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query);
            a = _gremlinResultMapper.MapMultiple<T>(r);
        }
        return a; // can be null since query may find no results
    }
}
