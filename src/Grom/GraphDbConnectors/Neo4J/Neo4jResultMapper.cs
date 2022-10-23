using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.Util;
using Grom.Util.Exceptions;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Neo4J;

internal class Neo4jResultMapper
{
    internal T? Map<T>(List<IRecord> records, string nodeKey = "n", string relationshipKey = "r", string parentKey = "p") where T : EntityNode
    {
        return MapMultiple<T>(records, nodeKey, relationshipKey, parentKey).FirstOrDefault();

    }

    internal IEnumerable<T> MapMultiple<T>(List<IRecord> records, string nodeKey = "n", string relationshipKey = "r", string parentKey = "p") where T : EntityNode
    {
        var relationshipsStructure = Utils.getAllRelatedTypesFromType(typeof(T), new());
        var rootnodes = new Dictionary<long, T>();

        var nodes = new Dictionary<long, EntityNode>();
        
        // Create all nodes
        foreach (var record in records)
        {
            if (record[nodeKey] is not null)
            {
                var node = (INode)record[nodeKey];
                if(!rootnodes.ContainsKey(node.Id))
                {
                    var mappedNode = (T)MapNodeToObject((INode)record[nodeKey], typeof(T));
                    rootnodes.Add(node.Id, mappedNode);
                    nodes.Add(node.Id, mappedNode); 
                }
            }
            if (record.Keys.Contains(parentKey) && record[parentKey] is not null)
            {
                var node = (INode)record[parentKey];
                if (!nodes.ContainsKey(node.Id))
                {
                    var nodeType = relationshipsStructure.FirstOrDefault(t => t.Item3.Name.Equals(node.Labels.First())).Item3;
                    var mappedNode = MapNodeToObject((INode)record[parentKey], nodeType);
                    nodes.Add(node.Id, mappedNode);
                }
            }
        }
        // Add relationships between nodes in result
        foreach (var record in records)
        {
            if (record.Keys.Contains(relationshipKey) && record[relationshipKey] is not null)
            {
                var relationship = (IRelationship)record[relationshipKey];
                var relationshipType = relationshipsStructure.FirstOrDefault(t => t.Item2.Name.Equals(relationship.Type)).Item2;
                if(relationshipType is null)
                {
                    throw new InvalidOperationException($"Cannot map relationship type {relationship.Type} to a class!");
                }
                var mappedRelationship = MapRelationshipToObject(relationship, relationshipType);
                var parent = nodes[relationship.StartNodeId];
                var child = nodes[relationship.EndNodeId];
                Utils.AddRelationshipToNode(parent, mappedRelationship, child);
            }
        }

        return rootnodes.Values.ToList();
    }

    internal EntityNode MapNodeToObject(INode node, Type t)
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
            if (node.Properties.ContainsKey(propertyName)) {
                var propertyValue = Utils.Typify(property.PropertyType, node.Properties[Utils.GetNodePropertyName(property)]); //TODO: reuse propertyName
                property.SetValue(nodeInstance, propertyValue);
            }

        }
        nodeInstance.EntityNodeId = RetrieveNodeIdentifier(node);
        return nodeInstance;
    }

    internal RelationshipBase MapRelationshipToObject(IRelationship relationship, Type t)
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
            if (relationship.Properties.ContainsKey(propertyName))
            {
                var propertyValue = Utils.Typify(property.PropertyType, relationship.Properties[Utils.GetRelationshipPropertyName(property)]);
                property.SetValue(relationshipInstance, propertyValue);
            }

        }
        relationshipInstance.EntityRelationshipId = RetrieveRelationshipIdentifier(relationship);
        return relationshipInstance;
    }

    private Guid RetrieveNodeIdentifier(INode node)
    {
        if (node.Properties.ContainsKey("nodeIdentifier")) {
            var guidString = node.Properties["nodeIdentifier"].As<string>();
            return Utils.StringToGuid(guidString);
        } else
        {
            // This should not happen as the query should only find nodes with the nodeIdentifier property!
            throw new QueryResultException("Node should have nodeIdentifier property!");
        }
    }

    private Guid RetrieveRelationshipIdentifier(IRelationship relationship)
    {
        if (relationship.Properties.ContainsKey("relationshipIdentifier"))
        {
            var guidString = relationship.Properties["relationshipIdentifier"].As<string>();
            return Utils.StringToGuid(guidString);
        }
        else
        {
            // This should not happen as the query should only find relationships with the relationshipIdentifier property!
            throw new QueryResultException("Relationship should have relationshipIdentifier property!");
        }
    }
}
