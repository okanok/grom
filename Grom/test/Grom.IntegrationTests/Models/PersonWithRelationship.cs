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
