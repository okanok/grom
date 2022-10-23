using Grom.Util;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Grom.GraphDbConnectors.Gremlin;

internal class GremlinTypeMapper : ITypeMapper
{
    string ITypeMapper.StringifyBool(object o)
    {
        return (bool)o ? "true" : "false";
    }

    string ITypeMapper.StringifyDateOnly(object o)
    {
        return string.Format("'{0}'", ((DateOnly)o).ToString("yyyy'-'MM'-'dd")); // TODO: support datetime()
    }

    string ITypeMapper.StringifyDateTime(object o)
    {
        return string.Format("'{0}'", ((DateTime)o).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff")); // TODO: support datetime()
    }

    string ITypeMapper.StringifyFloat(object o)
    {
        return ((float)o).ToString(CultureInfo.InvariantCulture);
    }

    string ITypeMapper.StringifyInt(object o)
    {
        return o.ToString() ?? string.Empty;
    }

    string ITypeMapper.StringifyLong(object o)
    {
        return ((long)o).ToString(CultureInfo.InvariantCulture);
    }

    string ITypeMapper.StringifyNull(object? o)
    {
        return "null";
    }

    string ITypeMapper.StringifyString(object o)
    {
        // Replace lines escape the escape sequences
        // TODO: import with something like: find \ and replace with \\
        return string.Format("'{0}'", (string)o)
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\v", "\\v");
    }

    int ITypeMapper.TypifyInt(object o)
    {
        return Convert.ToInt32(o);
    }

    bool ITypeMapper.TypifyBoolean(object o)
    {
        return Convert.ToBoolean(o);
    }

    float ITypeMapper.TypifyFloat(object o)
    {
        return Convert.ToSingle(o);
    }

    long ITypeMapper.TypifyLong(object o)
    {
        return Convert.ToInt64(o);
    }

    string ITypeMapper.TypifyString(object o)
    {
        return Convert.ToString(o);
    }

    DateTime ITypeMapper.TypifyDateTime(object o)
    {
        return (DateTime)o;
    }

    DateOnly ITypeMapper.TypifyDateOnly(object o)
    {
        return DateOnly.Parse(o.ToString());
    }
}
