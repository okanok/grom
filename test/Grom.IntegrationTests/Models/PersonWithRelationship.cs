using Grom.Entities.Relationships;

namespace Grom.IntegrationTests.Models;

public class PersonWithRelationship: Person
{
    public PersonWithRelationship()
    {
    }

    public PersonWithRelationship(string name, int age) : base(name, age)
    {
    }

    public RelationshipCollection<KnowsRelationship, PersonWithRelationship> KnownPeople { get; set; } = new();
}

public class PersonWithCustomRelationship : Person
{
    public PersonWithCustomRelationship()
    {
    }

    public PersonWithCustomRelationship(string name, int age) : base(name, age)
    {
    }

    public RelationshipCollection<KnowsCustomNameRelationship, PersonWithCustomRelationship> KnownPeople { get; set; } = new();
}
