using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace GromPerformanceTest.benchmaksl;

[MemoryDiagnoser]
public class TypifyBenchmarks
{

    long a = 1;
    int b = 0;

    [Benchmark]
    public void TypifyWithType()
    {
        var c = Typify(b.GetType(), a);
    }

    [Benchmark]
    public void TypifyWithoutCast()
    {
        var c = TypifyOp(b, a);

    }

    object Typify(Type expectedType, object o)
    {
        if (expectedType == typeof(int))
        {
            return Convert.ToInt32(o);
        }
        return o;
    }

    object TypifyOp(object? expectedType, object o)
    {
        if (expectedType is int)
        {
            return Convert.ToInt32(o);
        }
        return o;
    }
}
