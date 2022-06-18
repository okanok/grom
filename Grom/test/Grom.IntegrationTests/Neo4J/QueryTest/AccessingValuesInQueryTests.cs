using Grom.GromQuery;
using Grom.IntegrationTests.Models;

namespace Grom.IntegrationTests.Neo4J.QueryTest;

[Collection("neo4j-collection")]
public class AccessingValuesInQueryTests: IClassFixture<Neo4JTestBase>
{
    static AccessingValuesInQueryTests()
    {
        var personNode = new Person("Drogo", 30);
        personNode.Persist().Wait();
    }

    [Fact]
    public async Task CompareHardcodedValueStringTest()
    {
        var node = await Retrieve<Person>
            .Where(n => n.Name == "Drogo")
            .GetSingle();

        Assert.NotNull(node);
    }

    [Fact]
    public async Task CompareVariablealueTest()
    {
        var expectedName = "Drogo";
        var node = await Retrieve<Person>
            .Where(n => n.Name == expectedName)
            .GetSingle();


        Assert.NotNull(node);
    }

    [Fact]
    public async Task CompareClassFieldValueTest()
    {
        var fieldClass = new FieldClass();
        var node = await Retrieve<Person>
            .Where(n => n.Name == fieldClass.Name)
            .GetSingle();

        Assert.NotNull(node);
    }

    [Fact]
    public async Task CompareGetterValueTest()
    {
        var propertyClass = new PropertyClass();
        var node = await Retrieve<Person>
            .Where(n => n.Name == propertyClass.GetName())
            .GetSingle();

        Assert.NotNull(node);
    }

    [Fact]
    public async Task CompareStructValueTest()
    {
        var valueStruct = new ValueStruct();
        valueStruct.Name = "Drogo";
        var node = await Retrieve<Person>
            .Where(n => n.Name == valueStruct.Name)
            .GetSingle();

        Assert.NotNull(node);
    }
}


class FieldClass
{
    public string Name { get; set; } = "Drogo";
}

class PropertyClass
{
    public string name = "Drogo";

    public string GetName()
    {
        return name;
    }
}

struct ValueStruct
{
    public string Name;
}
