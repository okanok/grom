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
        if(nodeInstance is null)
        {
            throw new ArgumentException($"Cant create an instance of {nameof(T)}!");
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

    internal IEnumerable<T> MapMultiple<T>(List<IRecord> records, string nodeKey = "n") where T : EntityNode
    {
        var result = new List<T>();
        foreach (var record in records)
        {
            var resultItem = Map<T>(record);
            if (resultItem is not null)
            {
                result.Add(resultItem);
            }
        }
        return result;
    }
}
