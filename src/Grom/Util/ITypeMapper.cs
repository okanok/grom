namespace Grom.Util;

internal interface ITypeMapper
{
    internal string StringifyNull(object? o);
    internal string StringifyInt(object o);
    internal string StringifyString(object o);
    internal string StringifyBool(object o);
    internal string StringifyFloat(object o);
    internal string StringifyLong(object o);
    internal string StringifyDateTime(object o);
    internal string StringifyDateOnly(object o);

    internal int TypifyInt(object o);
    internal bool TypifyBoolean(object o);
    internal float TypifyFloat(object o);
    internal long TypifyLong(object o);
    internal string TypifyString(object o);
    internal DateTime TypifyDateTime(object o);
    internal DateOnly TypifyDateOnly(object o);
}
