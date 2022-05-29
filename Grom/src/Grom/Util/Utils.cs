using System.Reflection;
using Grom.Entities.Attributes;

namespace Grom.Util;

// TODO: make internal again
public class Utils
{
    internal static string TypeStringify(object o)
    {
        return o switch
        {
            string => string.Format("'{0}'", o),
            bool => (bool)o ? "1" : "0",
            _ => o?.ToString() ?? String.Empty,
        };
    }

    internal static object Typify(Type expectedType, object o)
    {
        // TODO: support other types aswell
        // TODO: handle unsuported types
        if (expectedType == typeof(int))
        {
            return Convert.ToInt32(o);
        }
        return o;
    }

    internal static IEnumerable<PropertyInfo> GetEntityProperties(Type t)
    {
        PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(NodeProperty)));
    }
}
