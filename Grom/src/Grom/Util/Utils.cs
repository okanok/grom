﻿using System.Globalization;
using System.Reflection;
using Grom.Entities.Attributes;

namespace Grom.Util;

internal class Utils
{
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

    internal static IEnumerable<PropertyInfo> GetEntityProperties(Type t)
    {
        PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        return properties.Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(NodeProperty)));
    }
}
