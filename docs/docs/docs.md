# Features

## Connecting Grom to your database

Configuring Grom is easy, it only requires a database connection to be given to GromGraph.CreateConnection(...). You don't need to instantiate this class or call it anywhere after running CreateConnection once.

### Neo4J
To configure Grom for Neo4J simply use:
```
GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test")));
```
Any valid instance of Neo4J IDriver can be passed, so you are not restricted to username/password authentication.

## Mapping a class
To map a class as a node you have to do two things: inherit from EntityNode and annotate each property you want to map with NodeProperty. Grom does also require an empty constructor for all nodes.

A mapped class can be as simple as:
```
public class Person : EntityNode
{
    [NodeProperty]
    public string Name { get; set; }

    [NodeProperty]
    public int Age { get; set; }
}
```
Currently integer, boolean, string, float, long, DateTime and DateOnly are supported. 
If a property can be null you can also specify this with the ? operator.

Any other property without the NodeProperty attribute will be ignored by Grom.

If you want a property to have a specific name in the database you can use the dbPropertyName parameter the NodeProperty attribute.
```
public class Person : EntityNode
{
    [NodeProperty(dbPropertyName: "personName")]
    public string Name { get; set; }

    [NodeProperty]
    public int Age { get; set; }
}
```

## Persisting and updating a node

To persist a node simply call Persist() on any of your objects that inherit from EntityNode.
```
var personNode = new Person("John", 25);
await personNode.Persist();
```
Persist() will also check if the object has relationships and recursively create the relationships and related nodes.

Updating a node is also done by calling Persist().
As with persisting all related nodes and relationships will also be updated.
Grom will figure out if the node is allready created or not. 
```
var personNode = new Person("John", 25);
await personNode.Persist();

personNode.Age = 30;
await personNode.Persist();
```
To keep in mind:

* Grom only knows that a node exists if you have called Persist() on it or have retrieved it with Retrieve. It wont check if a node exists with the same properties in the database!
* The entire node is updated with Persist() i.e. all the properties are overwritten.

If you want to only persist or update the root node and are sure nothing else is changed than you can use Persist() with the Degree parameter.
```
var personNode = new Person("John", 25);
await personNode.Persist(Degree: 0);
```

You can set Degree to any number of 'jumps' you want to update from the root node. Degree 1 means persisting the root node and its directly related nodes and relationships, Degree 2 their own directly related nodes and relationships, etc.
```
------------------------ Degree: 2
   node4         node5
     |             |
relationship3 relationship4
     |             |
------------------------ Degree: 1
   node2         node3
     |             |
relationship1 relationship2
        \         /
------------------------ Degree: 0
           node1
```
## Deleting a node

A node can be deleted by calling DeleteNode().
```
await personNode.DeleteNode();
```
The actual object in your code will still exist but the node and all its relationships to other nodes will be deleted in the database.

## Retrieving a node

Nodes can be retrieved using Retrieve\<T>. Nodes can be filtered by simply giving a lambda function that has a single parameter (the root node) and returns a boolean. Grom turns the lambda function into a query for you.
```
var personNode = await Retrieve<Person>
    .Where(p => p.Name == "John")
    .GetSingle();
```    
Boolean operators such as &&, ||, !, ==, !=, >, <, >= and <= are supported. Properties can be compared to constants, variables, method calls with no parameters and properties or fields in objects. Do note however that Grom can't turn everything a lambda can do into a query. Try to keep the lambda simple.

If you do not want to retrieve a node with all its relationships .IgnoreRelatedNodes() can be added to the Retrieve query.

## Relationships

For now only directed relationships are supported. Release 1.0 will also include support for undirected relationships.

To define a relationship between nodes we first need to create a relationship entity. The entity needs to inherit from RelationshipBase and needs an empty constructor. Each property you want to map can be annotated with RelationshipProperty. A relationship entity will look something like this:
```
public class Knows : RelationshipBase
{
    [RelationshipProperty]
    public int ForYears { get; set; }

    public Knows()
    {
    }

    public Knows(int forYears)
    {
        ForYears = forYears;

    }
}
```
RelationshipProperty also supports a dbPropertyName parameter to specify a name for the property in the database.

To define a relationship between nodes simply add a new property with type RelationshipCollection. This collection needs two arguments: a relationship type and a target node type. 
```
public class Person : EntityNode
{
    [NodeProperty]
    public string Name { get; set; }

    [NodeProperty]
    public int Age { get; set; }

    public RelationshipCollection<Knows, Person> knownPeople { get; set; } = new();

    public Person()
    {
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }
}
```

A relationship is added by adding an item to this collection: 
```
var person1 = new Person("John", 25);
var person2 = new Person("Doe", 26);
person1.knownPeople.Add(new Knows(5), person2);
await person1.Persist();
```
Calling Persist() will, in adition to the node, also persist or update any descendant nodes and relationships. Updating works the same way as with nodes, change the property and call Persist() again on any ancestor node. 
If you are sure that you only changed properties in one relationship you can use UpdateRelationshipOnly(). This will make sure that only the relationship is updated on which UpdateRelationshipOnly() is called.
```
var person1 = new Person("John", 25);
var person2 = new Person("Doe", 26);
person1.knownPeople.Add(new Knows(5), person2);
await person1.Persist();

person1.knownPeople.First().Relationship.ForYears = 10;
await person1.knownPeople.First().Relationship.UpdateRelationshipOnly();
```
UpdateRelationshipOnly() does not work on relationships that are not persisted yet.

To delete a relationship you can use the Remove, RemoveAt or RemoveRange methods on RelationshipCollection. They work the same as in a List\<T>. 

# About Grom

## How Grom works

Grom works, as any ORM, by mapping C# concepts to concepts in graph databases. 
A class become a node, a class property becomes a node property and a property that is a list of another class becomes a relationship. 
Grom links objects to nodes with the internal property EntityNodeId in EntityNode. 
This way Grom knows if the object already exists in the database or not and thus if a node needs to be created or updated.

The rest of Grom is simply an exercise of translating C# and .NET to the right database in a good way.


## Philosophy of Grom

The idea of Grom is to leverage as many C# and .NET features as possible to create a simple, modern, reliable and feature rich ORM for graph databases.
Other graph database ORMs use a fluent API where you can define a node and its properties. 
This makes the ORM much simpler and lighter but you also lose a lot of the benefits that C# and .NET have.
For example Grom gives you inheritance out of the box even though this is not a concept in most graph databases. 
Querying nodes is also simply done by writing a lambda expression instead of having to learn an intermediate language specific to the ORM.

Another important concept of Grom is to map C# to graph databases not graph databases to C#. 
This means that we don't translate all features of graph databases to C# but see whats usefull in C# and try to make that work in graph databases. 