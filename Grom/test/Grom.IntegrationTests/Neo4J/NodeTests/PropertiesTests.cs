using Grom.Entities;
using Grom.Entities.Attributes;
using Grom.GromQuery;
using Grom.IntegrationTests.Models;
using Grom.Util.Exceptions;
using System.Collections;

namespace Grom.IntegrationTests.Neo4J.NodeTests;

[Collection("neo4j-collection")]
public class PropertiesTests: Neo4JTestBase
{
    IEnumerable<SupportedPropertiesNode> nodes = new List<SupportedPropertiesNode>()
    {
        new SupportedPropertiesNode
        {
            StringProp = "Some String",
            IntProp = 55,
            BoolProp = true,
            FloatProp = 42.0F,
            LongProp = 12345678901235L
        },
        new SupportedPropertiesNode
        {
            StringProp = "",
            IntProp = 0,
            BoolProp = false,
            FloatProp = 0.00F,
            LongProp = 0L
        },
        new SupportedPropertiesNode
        {
            StringProp = "string123",
            IntProp = -55,
            BoolProp = false,
            FloatProp = -12.12F,
            LongProp = -10L
        },
        new SupportedPropertiesNode
        {
            StringProp = "line 1 \n And a new line",
            IntProp = 011,
            BoolProp = 1==1,
            FloatProp = 1.24F-0.5534F,
            LongProp = 00123124242L
        }
    };

    [Fact]
    public async Task CreatingNodeWithAllSupportedPropertyTypesShouldRun()
    {
        foreach(var node in nodes)
        {
            await node.Persist();

            var retrievedNode = await Retrieve<SupportedPropertiesNode>
                .Where(n => n.StringProp == node.StringProp)
                .GetSingle();

            Assert.NotNull(retrievedNode);
            Assert.Equal(node.StringProp, retrievedNode!.StringProp);
            Assert.Equal(node.IntProp, retrievedNode.IntProp);
            Assert.Equal(node.BoolProp, retrievedNode.BoolProp);
            Assert.Equal(node.FloatProp, retrievedNode.FloatProp);
            Assert.Equal(node.LongProp, retrievedNode.LongProp);
        }
    }

    [Fact]
    public async Task CreatingNodeWithNonPropPropertiesShouldRun()
    {
        var node = new NodeWithNonNodePropertyProperties
        {
            identifier = 12,
            idk = 1000
        };
        await node.Persist();

        var retrievedNode = await Retrieve<NodeWithNonNodePropertyProperties>
            .Where(n => n.identifier == 12)
            .GetSingle();

        Assert.NotNull(retrievedNode);
        Assert.Equal(node.identifier, retrievedNode!.identifier);
        Assert.NotEqual(node.idk, retrievedNode.idk);
    }

    [Fact]
    public void CreatingNodeWithUnsupportedTypeShouldThrowError()
    {
        Assert.Throws<NodePropertyTypeNotSupportedException>(() => new NodeWithDateTime
        {
            DateProperty = new DateTime()
        });
    }

    [Fact]
    public void CreatingNodeWithUnsupportedListTypeShouldThrowError()
    {
        Assert.Throws<NodePropertyTypeNotSupportedException>(() => new NodeWithList
        {
            IntArray = new int[] { 1, 2 }
        });
    }
}

public class Nodes : IEnumerable<SupportedPropertiesNode>
{
    private readonly List<SupportedPropertiesNode> _data = new List<SupportedPropertiesNode>()
    {
        new SupportedPropertiesNode
        {
            StringProp = "Some String",
            IntProp = 55,
            BoolProp = true,
            FloatProp = 42.0F,
            LongProp = 12345678901235L
        },
        new SupportedPropertiesNode
        {
            StringProp = "",
            IntProp = 0,
            BoolProp = false,
            FloatProp = 0.00F,
            LongProp = 0L
        },
        new SupportedPropertiesNode
        {
            StringProp = "string123",
            IntProp = -55,
            BoolProp = false,
            FloatProp = -12.12F,
            LongProp = -10L
        },
        new SupportedPropertiesNode
        {
            StringProp = "line 1 \n And a new line",
            IntProp = 011,
            BoolProp = 1==1,
            FloatProp = 1.24F-0.5534F,
            LongProp = 00123124242L
        }
    };

    public IEnumerator<SupportedPropertiesNode> GetEnumerator()
    {
        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

//TODO: add test for this
public class NodeWithForbiddenPropName : EntityNode
{
    [NodeProperty]
    public int id { get; set; }
}

public class NodeWithNonNodePropertyProperties : EntityNode
{
    [NodeProperty]
    public int identifier { get; set; }

    public int idk { get; set; }
}

public class NodeWithDateTime : EntityNode
{
    [NodeProperty]
    public DateTime DateProperty { get; set; }
}

public class NodeWithList : EntityNode
{
    [NodeProperty]
    public int[] IntArray { get; set; } = Array.Empty<int>();
}
