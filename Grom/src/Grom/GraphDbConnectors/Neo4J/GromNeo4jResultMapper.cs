using Grom.Entities;
using Grom.Util;
using Neo4j.Driver;

namespace Grom.GraphDbConnectors.Neo4J;

internal class GromNeo4jResultMapper
{
    internal T Map<T>(IRecord record, string nodeKey = "n") where T : EntityNode
    {
        var node = (INode)record[nodeKey];
        var nodeInstance = (T)Activator.CreateInstance(typeof(T));
        var properties = Utils.GetEntityProperties(nodeInstance.GetType()).ToList();
        foreach (var property in properties)
        {
            var propertyValue = Utils.Typify(property.PropertyType, node.Properties[property.Name]);
            property.SetValue(nodeInstance, propertyValue);
        }
        nodeInstance.EntityNodeId = node.Id;
        return nodeInstance;
    }
}
