using Grom.Entities;
using Grom.Entities.Relationships;
using Grom.Util;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Neo4J;

internal class GromNeo4jResultMapper
{
    internal T? Map<T>(List<IRecord> records, string nodeKey = "n", string relationshipKey = "r", string parentKey = "p") where T : EntityNode
    {
        return MapMultiple<T>(records, nodeKey, relationshipKey, parentKey).FirstOrDefault(); //TODO: maybe give error when multiple nodes are found? 

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
            if (record[parentKey] is not null)
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
            if (record[relationshipKey] is not null)
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
                AddRelationshipToNode(parent, mappedRelationship, child);
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
            var propertyValue = Utils.Typify(property.PropertyType, node.Properties[property.Name]);
            property.SetValue(nodeInstance, propertyValue);
        }
        nodeInstance.EntityNodeId = node.Id;
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
            var propertyValue = Utils.Typify(property.PropertyType, relationship.Properties[property.Name]);
            property.SetValue(relationshipInstance, propertyValue);
        }
        relationshipInstance.EntityRelationshipId = relationship.Id;
        return relationshipInstance;
    }

    //TODO: add to util
    internal void AddRelationshipToNode(EntityNode parent, RelationshipBase relationship, EntityNode child)
     {
        var relationshipProperty = Utils.GetNodeRelationshipProperty(parent, relationship.GetType(), child.GetType());
        if(relationshipProperty is null)
        {
            throw new InvalidOperationException($"Cannot find relationship collection in class {nameof(parent)} with relationhship type {nameof(relationship)} and child {nameof(child)}");
        }
        var relationshipCollection = relationshipProperty.GetValue(parent, null) as IRelationshipCollection;
        if(relationshipCollection is null)
        {
            throw new InvalidOperationException($"Could not retrieve reference to collection {relationshipProperty.Name} from class {nameof(parent)}");
        }
        relationshipCollection.Add(relationship, child); 
    }
}
