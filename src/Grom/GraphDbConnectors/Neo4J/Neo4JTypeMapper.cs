using Grom.Util;
using System.Globalization;

namespace Grom.GraphDbConnectors.Neo4J;

internal class Neo4JTypeMapper: ITypeMapper
{
    string ITypeMapper.StringifyBool(object o)
    {
        return (bool)o ? "1" : "0";
    }

    string ITypeMapper.StringifyDateOnly(object o)
    {
        return string.Format("date('{0}')", ((DateOnly)o).ToString("yyyy'-'MM'-'dd"));
    }

    string ITypeMapper.StringifyDateTime(object o)
    {
        return string.Format("datetime('{0}')", ((DateTime)o).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK"));
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
        return string.Format("'{0}'", o);
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
        return Convert.ToDateTime(o.ToString());
    }

    DateOnly ITypeMapper.TypifyDateOnly(object o)
    {
        return DateOnly.Parse(o.ToString());
    }
}
