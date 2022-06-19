using Grom.Entities;
using Grom.Entities.Attributes;

namespace Grom.IntegrationTests.Models;

public class Person : EntityNode
{
    [NodeProperty]
    public string Name { get; set; }

    [NodeProperty]
    public int Age { get; set; }

    public Person()
    {
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
