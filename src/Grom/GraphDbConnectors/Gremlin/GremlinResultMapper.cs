using Gremlin.Net.Driver;
using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Gremlin;

internal class GremlinResultMapper
{
    internal T? Map<T>(ResultSet<Dictionary<object, object>> records) where T : EntityNode
    {
        return MapMultiple<T>(records).FirstOrDefault();

    }

    internal IEnumerable<T> MapMultiple<T>(ResultSet<Dictionary<object, object>> records) where T : EntityNode
    {
        var relationshipsStructure = Utils.getAllRelatedTypesFromType(typeof(T), new());
        var nodes = new Dictionary<Guid, EntityNode>();

        foreach (var record in records)
        {
            // retrieve parent from record
            record.TryGetValue("p", out object? parentNode);
            if (parentNode is not null)
            {
                var parentDictNode = (Dictionary<object, object>)parentNode;
                var parentTypeString = GetLabelFromNodeDictionary(parentDictNode);
                var parentType = parentTypeString == typeof(T).Name
                    ? typeof(T)
                    : relationshipsStructure
                       .Where(t => t.Item1.Name.Equals(parentTypeString))
                       .Select(e => e.Item1)
                       .FirstOrDefault();
                if (parentType is null)
                {
                    throw new InvalidOperationException($"Cannot map relationship type {parentType} to a class!");
                }

                var mappedNode = MapNodeToObject((Dictionary<object,object>)parentNode, parentType);
                if(!nodes.ContainsKey(mappedNode.EntityNodeId.Value))
                {
                    nodes.Add(mappedNode.EntityNodeId.Value, mappedNode);
                }
            }

            // retrieve child from record
            record.TryGetValue("c", out object? childNode);
            if (childNode is not null)
            {
                var childDictNode = (Dictionary<object, object>)childNode;
                var childTypeString = GetLabelFromNodeDictionary(childDictNode);
                var childType = relationshipsStructure
                   .Where(t => t.Item3.Name.Equals(childTypeString))
                   .Select(e => e.Item3)
                   .FirstOrDefault();
                if (childType is null)
                {
                    throw new InvalidOperationException($"Cannot map relationship type {childTypeString} to a class!");
                }

                var mappedNode = MapNodeToObject((Dictionary <object,object>)childNode, childType);
                if (!nodes.ContainsKey(mappedNode.EntityNodeId.Value))
                {
                    nodes.Add(mappedNode.EntityNodeId.Value, mappedNode);
                }
            }

        }

        var rootnodes = new Dictionary<Guid, EntityNode>(nodes);
        // create all relationships
        foreach (var record in records)
        {
            // retrieve relationship from record
            record.TryGetValue("r", out object? relationshipNode);
            if (relationshipNode is not null)
            {
                var relationshipDictNode = (Dictionary<object, object>) relationshipNode;
                var relationshipTypeString = GetLabelFromNodeDictionary(relationshipDictNode);
                var relationshipType = relationshipsStructure
                    .Where(t => t.Item2.Name.Equals(relationshipTypeString))
                    .Select(e => e.Item2)
                    .FirstOrDefault();
                if (relationshipType is null)
                {
                    throw new InvalidOperationException($"Cannot map relationship type {relationshipTypeString} to a class!");
                }
                var mappedRelationship = MapRelationshipToObject(relationshipDictNode, relationshipType);
                var parentId = RetrieveNodeIdentifier((Dictionary<object, object>)record["p"]);
                var parent = nodes[parentId];
                var childId = RetrieveNodeIdentifier((Dictionary<object, object>)record["c"]);
                var child = nodes[childId];
                Utils.AddRelationshipToNode(parent, mappedRelationship, child);

                // If a node is a child in a relationship it obviously is not a root node.
                // casues bug: If query should find 2 root nodes but they are related it will only show the parent of these two.
                // Will fix later since getting a relationship in .net gremlin for any graph database is hard
                rootnodes.Remove(childId);
            }
        }
        // get root nodes
        return rootnodes.Select(e => (T)e.Value);
    }

    internal EntityNode MapNodeToObject(Dictionary<object, object> node, Type t)
    {
        var nodeInstance = (EntityNode)Activator.CreateInstance(t)!;
        if (nodeInstance is null)
        {
            throw new ArgumentException($"Cant create an instance of {t.Name}!");
        }
        var properties = Utils.GetEntityProperties(nodeInstance.GetType()).ToList();
        foreach (var property in properties)
        {
            var propertyName = Utils.GetNodePropertyName(property);
            // if false property is not in result and thus value is null
            if (node.ContainsKey(propertyName))
            {
                var propertyValue = Utils.Typify(property.PropertyType, ((object[])node[propertyName])[0]);
                property.SetValue(nodeInstance, propertyValue);
            }

        }
        nodeInstance.EntityNodeId = RetrieveNodeIdentifier(node);
        return nodeInstance;
    }

    internal RelationshipBase MapRelationshipToObject(Dictionary<object, object> relationship, Type t)
    {
        var relationshipInstance = (RelationshipBase)Activator.CreateInstance(t)!;
        if (relationshipInstance is null)
        {
            throw new ArgumentException($"Cant create an instance of {t.Name}!");
        }
        var properties = Utils.GetRelationshipProperties(relationshipInstance.GetType()).ToList();
        foreach (var property in properties)
        {
            var propertyName = Utils.GetRelationshipPropertyName(property);
            // if false property is not in result and thus value is null
            if (relationship.ContainsKey(propertyName))
            {
                var propertyValue = Utils.Typify(property.PropertyType, relationship[Utils.GetRelationshipPropertyName(property)]);
                property.SetValue(relationshipInstance, propertyValue);
            }

        }
        relationshipInstance.EntityRelationshipId = RetrieveRelationshipIdentifier(relationship);
        return relationshipInstance;
    }

    private Guid RetrieveNodeIdentifier(Dictionary<object, object> node)
    {
        if (node.ContainsKey("nodeIdentifier"))
        {
            var guidString = ((object[])node["nodeIdentifier"])[0].As<string>();
            return Utils.StringToGuid(guidString);
        }
        else
        {
            // This should not happen as the query should only find nodes with the nodeIdentifier property!
            throw new QueryResultException("Node should have nodeIdentifier property!");
        }
    }

    private Guid RetrieveRelationshipIdentifier(Dictionary<object, object> relationship)
    {
        if (relationship.ContainsKey("relationshipIdentifier"))
        {
            var guidString = relationship["relationshipIdentifier"].As<string>();
            return Utils.StringToGuid(guidString);
        }
        else
        {
            // This should not happen as the query should only find relationships with the relationshipIdentifier property!
            throw new QueryResultException("Relationship should have relationshipIdentifier property!");
        }
    }

    private string? GetLabelFromNodeDictionary(Dictionary<object, object> node)
    {
        return node.Where(e => e.Key.ToString() == "Gremlin.Net.Process.Traversal.T").Select(e => (string)e.Value).SingleOrDefault();
    }
}
