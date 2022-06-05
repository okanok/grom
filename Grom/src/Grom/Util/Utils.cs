using System.Globalization;
using System.Reflection;
using Grom.Entities.Attributes;

namespace Grom.Util;

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
}
