using System.Text;
using BenchmarkDotNet.Attributes;

namespace GromPerformanceTest.benchmarks;

[MemoryDiagnoser]
public class QueryBuildingBenchmarks
{
    public string queryBase = "SELECT * FROM {0} a WHERE a IN ({1})";
    public string comma = ", ";
    public string name = "person";
    List<Tuple<string, string>> elems = new List<Tuple<string, string>>() { new Tuple<string, string>("a", "1"), new Tuple<string, string>("b", "2"), new Tuple<string, string>("c", "3"), new Tuple<string, string>("d", "4"), new Tuple<string, string>("e", "5") };
    StringBuilder StringBuilder = new StringBuilder();
    public Tuple<string, string, string> queryBaseSb = new Tuple<string, string, string>("SELECT * FROM ", " a WHERE a IN (", ")");

    public QueryBuildingBenchmarks()
    {
        StringBuilder = new StringBuilder();

    }

    [Benchmark]
    public void BuildWithJoinAndAdd()
    {
        var a = string.Join(comma, elems.Select(t => t.Item1 + ": " + t.Item2));
        var b = string.Format(queryBase, name, a);
    }

    [Benchmark]
    public void BuildWithChar()
    {
        var chars = new List<char>();
        foreach (var t in elems)
        {
            chars.AddRange(t.Item1);
            chars.AddRange(t.Item1);
            chars.AddRange(": ");
            if (!elems.Last().Equals(t))
            {
                chars.AddRange(comma);
            }
        }
        var a = chars.ToString();
        var b = string.Format(queryBase, name, a);
    }

    [Benchmark]
    public void BuildWithSb()
    {
        StringBuilder.Append(queryBaseSb.Item1);
        StringBuilder.Append(name);
        StringBuilder.Append(queryBaseSb.Item2);
        foreach (var t in elems)
        {
            StringBuilder.Append(t.Item1);
            StringBuilder.Append(": ");
            StringBuilder.Append(t.Item2);
            if (!elems.Last().Equals(t))
            {
                StringBuilder.Append(", ");
            }
        }
        StringBuilder.Append(queryBaseSb.Item2);
        var a = StringBuilder.ToString();
        StringBuilder.Clear();
    }


    [Benchmark]
    public void BuildWithSbAndFormat()
    {
        //StringBuilder.Append(queryBaseSb.Item1);
        foreach (var t in elems)
        {
            StringBuilder.Append(t.Item1);
            StringBuilder.Append(": ");
            StringBuilder.Append(t.Item2);
            if (!elems.Last().Equals(t))
            {
                StringBuilder.Append(", ");
            }
        }
        //StringBuilder.Append(queryBaseSb.Item2);
        var a = StringBuilder.ToString();
        var b = string.Format(queryBase, name, StringBuilder);
        StringBuilder.Clear();
    }

    // 3 elements
    //|               Method |     Mean |   Error |  StdDev |  Gen 0 | Allocated |
    //|--------------------- |---------:|--------:|--------:|-------:|----------:|
    //|  BuildWithJoinAndAdd | 254.1 ns | 4.45 ns | 6.24 ns | 0.0401 |     336 B |
    //|        BuildWithChar | 588.3 ns | 6.86 ns | 6.08 ns | 0.0820 |     688 B |
    //|          BuildWithSb | 192.1 ns | 3.63 ns | 3.72 ns | 0.0134 |     112 B |
    //| BuildWithSbAndFormat | 264.5 ns | 5.31 ns | 5.90 ns | 0.0267 |     224 B |

    // 5 elements
    //|               Method |     Mean |    Error |   StdDev |  Gen 0 | Allocated |
    //|--------------------- |---------:|---------:|---------:|-------:|----------:|
    //|  BuildWithJoinAndAdd | 365.7 ns |  6.57 ns |  5.49 ns | 0.0553 |     464 B |
    //|        BuildWithChar | 921.8 ns | 18.32 ns | 20.37 ns | 0.1249 |   1,048 B |
    //|          BuildWithSb | 327.9 ns |  3.54 ns |  3.31 ns | 0.0219 |     184 B |
    //| BuildWithSbAndFormat | 420.3 ns |  4.31 ns |  4.03 ns | 0.0372 |     312 B |

}
