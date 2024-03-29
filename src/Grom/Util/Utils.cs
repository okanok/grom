﻿using System.Globalization;
using System.Reflection;
using Grom.Entities;
using Grom.Entities.Attributes;
using Grom.Entities.Relationships;
using Grom.Util.Exceptions;

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
    internal static string StringifyType(object? o)
    {
        return o switch
        {
            null        => GromGraph.GetTypeMapper().StringifyNull(o),
            int         => GromGraph.GetTypeMapper().StringifyInt(o),
            string      => GromGraph.GetTypeMapper().StringifyString(o),
            bool        => GromGraph.GetTypeMapper().StringifyBool(o),
            float       => GromGraph.GetTypeMapper().StringifyFloat(o),
            long        => GromGraph.GetTypeMapper().StringifyLong(o),
            DateTime    => GromGraph.GetTypeMapper().StringifyDateTime(o),
            DateOnly    => GromGraph.GetTypeMapper().StringifyDateOnly(o),
            _           => throw new PropertyTypeNotSupportedException(o.GetType().Name)
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
        if (o is null)
        {
            return null;
        }
        else if (expectedType == typeof(int))
        {
            return GromGraph.GetTypeMapper().TypifyInt(o);
        }
        else if (expectedType == typeof(bool))
        {
            return GromGraph.GetTypeMapper().TypifyBoolean(o);
        }
        else if (expectedType == typeof(float))
        {
            return GromGraph.GetTypeMapper().TypifyFloat(o);
        }
        else if (expectedType == typeof(long))
        {
            return GromGraph.GetTypeMapper().TypifyLong(o);
        }
        else if (expectedType == typeof(string))
        {
            return GromGraph.GetTypeMapper().TypifyString(o);
        }
        else if (expectedType == typeof(DateTime))
        {
            return GromGraph.GetTypeMapper().TypifyDateTime(o);
        }
        else if (expectedType == typeof(DateOnly))
        {
            return GromGraph.GetTypeMapper().TypifyDateOnly(o);
        }
        throw new PropertyTypeNotSupportedException(o.GetType().Name);
    }

    /// <summary>
    /// Will create a string representation of a property with the name and value seperated by the given delimiter
    /// If NullabilityCheckOn() is called then will throw exception when value is null but property is not nullable
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static string StringifyProperty(string template, PropertyInfo property, EntityNode node)
    {
        var value = StringifyType(property.GetValue(node));
        var name = GetNodePropertyName(property);
       
        return string.Format(template, name, value);

    }

    /// <summary>
    /// Overload for relationships
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal static string StringifyProperty(string template, PropertyInfo property, RelationshipBase relationship)
    {
        var value = StringifyType(property.GetValue(relationship));
        var name = GetRelationshipPropertyName(property);
        return string.Format(template, name, value);
    }

    /// <summary>
    /// Gets the list of properties from an entity class that are tagged with NodeProperty
    /// </summary>
    /// <param name="nodeClass">the Type class of the entity class</param>
    /// <returns>list of PropertyInfo objects for each property</returns>
    internal static IEnumerable<PropertyInfo> GetEntityProperties(Type nodeClass)
    {
        PropertyInfo[] properties = nodeClass.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(NodeProperty)));
    }

    internal static IEnumerable<PropertyInfo> GetRelationshipProperties(Type relationshipClass)
    {
        PropertyInfo[] properties = relationshipClass.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(RelationshipProperty)));
    }

    /// <summary>
    /// Retrieves what the name of the property should be in the database
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    internal static string GetNodePropertyName(PropertyInfo property)
    {
        var attributePropertyName = property.GetCustomAttributes<NodeProperty>().First().dbPropertyName;
        return attributePropertyName is not null ? attributePropertyName : property.Name;
    }

    /// <summary>
    /// Retrieves what the name of the property should be in the database
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    internal static string GetRelationshipPropertyName(PropertyInfo property)
    {
        var attributePropertyName = property.GetCustomAttributes<RelationshipProperty>().First().dbPropertyName;
        return attributePropertyName is not null ? attributePropertyName : property.Name;
    }

    /// <summary>
    /// Retrieves the name of the property from a query Expression to compare in the database
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    internal static string GetMemberPropertyName(MemberInfo member)
    {
        var prop = member.GetCustomAttribute<NodeProperty>();
        if (prop is null)
        {
            throw new InvalidOperationException($"Property {member.Name} not marked with NodeProperty attribute used in query!");
        }
        return prop.dbPropertyName is not null ? prop.dbPropertyName : member.Name;
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

    internal static void AddRelationshipToNode(EntityNode parent, RelationshipBase relationship, EntityNode child)
    {
        var relationshipProperty = GetNodeRelationshipProperty(parent, relationship.GetType(), child.GetType());
        if (relationshipProperty is null)
        {
            throw new InvalidOperationException($"Cannot find relationship collection in class {nameof(parent)} with relationhship type {nameof(relationship)} and child {nameof(child)}");
        }
        var relationshipCollection = relationshipProperty.GetValue(parent, null) as IRelationshipCollection;
        if (relationshipCollection is null)
        {
            throw new InvalidOperationException($"Could not retrieve reference to collection {relationshipProperty.Name} from class {nameof(parent)}");
        }
        relationshipCollection.Add(relationship, child);
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

    internal static Guid StringToGuid(string? guidString)
    {
        var parsed = Guid.TryParse(guidString, out Guid parsedGuid);

        if (parsed)
        {
            return parsedGuid;
        }
        else
        {
            throw new FormatException($"Failed to parse Guid! value was {guidString}");
        }
    }
}
