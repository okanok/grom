using System.Globalization;
using System.Reflection;
using Grom.Entities;
using Grom.Entities.Attributes;

namespace Grom.Util;

//TODO: split into logical classes
internal class Utils
{
    /// <summary>
    /// Turns a given property value into a string that is correctly formatted
    /// so it can be used in queries.
    /// </summary>
    /// <param name="o">the propertie that will be stringified</param>
    /// <returns>the object in a formatted string form</returns>
    internal static string TypeStringify(object o)
    {
        return o switch
        {
            string  => string.Format("'{0}'", o),
            bool    => (bool)o ? "1" : "0",
            float   => ((float)o).ToString(CultureInfo.InvariantCulture),
            _       => o?.ToString() ?? String.Empty,
        };
    }

    /// <summary>
    /// Turns the given object into the given type
    /// </summary>
    /// <param name="expectedType">the expected type</param>
    /// <param name="o">the object that will be turned into a type</param>
    /// <returns></returns>
    internal static object Typify(Type expectedType, object o)
    {
        if (expectedType == typeof(int))
        {
            return Convert.ToInt32(o);
        } else if (expectedType == typeof(bool))
        {
            return Convert.ToBoolean(o);
        } else if (expectedType == typeof(float))
        {
            return Convert.ToSingle(o);
        }
        return o;
    }

    /// <summary>
    /// Gets the list of properties from an entity class that are tagged with NodeProperty
    /// </summary>
    /// <param name="t">the Type class of the entity class</param>
    /// <returns>list of PropertyInfo objects for each property</returns>
    internal static IEnumerable<PropertyInfo> GetEntityProperties(Type t)
    {
        PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(NodeProperty)));
    }

    internal static IEnumerable<PropertyInfo> GetRelationshipProperties(Type t)
    {
        PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(RelationshipProperty)));
    }

    internal static IEnumerable<(Type, Type, Type)> getAllRelatedTypesFromType(Type root, List<Type> discoveredTypes)
    {
        if(discoveredTypes.Contains(root))
        {
            // node allready discovered
            return Enumerable.Empty<(Type, Type, Type)>();
        }
        var relationshipCollections = root
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType.Name == "RelationshipCollection`2");
        var relationships = relationshipCollections
            .Select(r => ((Type)root, r.PropertyType.GenericTypeArguments[0], r.PropertyType.GenericTypeArguments[1]));
        discoveredTypes.Add(root);
        //Discover relationships of related classes
        foreach(var relationship in relationships)
        {
            relationships.Concat(getAllRelatedTypesFromType(relationship.Item1, discoveredTypes));
        }
        return relationships;
    }

    internal static PropertyInfo? GetNodeRelationshipProperty(EntityNode node, Type relationshipType, Type childType)
    {
        return node.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(p => 
                p.PropertyType.Name == "RelationshipCollection`2" 
                && p.PropertyType.GenericTypeArguments.All(ta => ta.Equals(relationshipType) || ta.Equals(childType))
            );
    }
}
