using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Tests.OrientDB.QueryTest;

[Collection("orientdb-collection")]
public class PropertyTypesComparisonTests : IClassFixture<TestBase>
{
    static PropertyTypesComparisonTests()
    {
        var node = new SupportedPropertiesNode
        {
            StringProp = "Some String",
            IntProp = 55,
            BoolProp = true,
            FloatProp = 42.0F,
            LongProp = 12345678901235L
        };
        node.Persist().Wait();
    }

    [Fact]
    public async Task EqComparisonInQueryToString()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.StringProp == "Some String")
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal("Some String", node!.StringProp);
    }

    [Fact]
    public async Task EqComparisonInQueryToInt()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.IntProp == 55)
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal(55, node!.IntProp);
    }

    [Fact]
    public async Task EqComparisonInQueryToBoolean()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.BoolProp == true)
            .GetSingle();

        Assert.NotNull(node);
        Assert.True(node!.BoolProp);
    }

    [Fact]
    public async Task EqComparisonInQueryToFloat()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.FloatProp == 42.0F)
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal(42.0F, node!.FloatProp);
    }

    [Fact]
    public async Task EqComparisonInQueryToLong()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.LongProp == 12345678901235L)
            .GetSingle();

        Assert.NotNull(node);
        Assert.Equal(12345678901235L, node!.LongProp);
    }

    [Fact]
    public async Task NeqComparisonInQueryToString()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.StringProp != "Some String")
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task NeqComparisonInQueryToInt()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.IntProp != 55)
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task NeqComparisonInQueryToBoolean()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.BoolProp != true)
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task NeqComparisonInQueryToFloat()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.FloatProp != 42.0F)
            .GetSingle();

        Assert.Null(node);
    }

    [Fact]
    public async Task NeqComparisonInQueryToLong()
    {
        var node = await Retrieve<SupportedPropertiesNode>
            .Where(n => n.LongProp != 12345678901235L)
            .GetSingle();

        Assert.Null(node);
    }
}
