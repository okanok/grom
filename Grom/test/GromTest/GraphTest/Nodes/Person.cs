using Grom.Entities.Relationships;
using Grom.Entities;
using Grom.Entities.Attributes;
using GromTest.GraphTest.Relationships;

namespace GromTest.GraphTest.Nodes;

public class Person : EntityNode
{
    [NodeProperty]
    public string Name { get; set; }

    [NodeProperty]
    public int Age { get; set; }

    public DirectedRelationships<Knows> knownPeople { get; set; } = new();

    public Person()
    {
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
