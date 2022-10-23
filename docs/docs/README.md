# Grom
Grom, a .NET graph database object relational mapper.

Grom is an easy to use, low config ORM that lets you map your C# classes to nodes and relationships in various graph databases.
This project is currently not production ready as it's still in beta. Version 1.0 is expected to release in Q4 2022. 

Since this project is open source and currently only worked on by the maintainer a 'beta' phase was needed in which a pre-production version is released so bugs can be found and fixed and important missing features can be requested before an actual production ready version is released.

Feel free to try it out and if some feature is missing don't hesitate to open an issue!  

# Quick start

## Connecting Grom to your database

Configuring Grom is easy, it only requires a database connection to be given to GromGraph.CreateConnection(...). You don't need to instantiate this class or call it anywhere after running CreateConnection once.

### Neo4J
To configure Grom for Neo4J simply use:
```
GromGraph.CreateConnection(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "test")));
```
Any valid instance of Neo4J IDriver can be passed, so you are not restricted to username/password authentication.

### Gremlin Server
To configure Grom for any database that has Gremlin Server running use:

```
var gremlinServer = new GremlinServer(
    hostname: "localhost",
    port: 8182,
    username: "root",
    password: "test"
);
gremlinClient = new GremlinClient(
    gremlinServer: gremlinServer
);

GromGraph.CreateConnection(gremlinClient);
```
Simply provide any valid instance of GremlinClient to GromGraph.CreateConnection() and Grom will take care of the rest.
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

## Persisting a node

To persist a node simply call Persist() on any of your objects that inherit from EntityNode.
```
var personNode = new Person("John", 25);
await personNode.Persist();
```
Grom is fully asynchronous so make sure you await when required. 

## Updating a node

Updating a node is also done by calling Persist(). Grom will figure out if the node is allready created or not. 
```
var personNode = new Person("John", 25);
await personNode.Persist();

personNode.Age = 30;
await personNode.Persist();
```
Do note that Grom only knows that a node exists if you have called Persist() on it or have retrieved it with Retrieve. It wont check if a node exists with the same properties in the database! 

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

To delete a relationship you can use the Remove, RemoveAt or RemoveRange methods on RelationshipCollection. They work the same as in a List\<T>. 

## Supported Databases

Currently Grom supports Neo4J and any database with Gremlin Server >= 3.4.0.

Grom's test suite tests against the following databases:
* Neo4J
* OrientDB

# More info

## Docs

Our docs can be found [here](https://okanok.github.io/grom/)

## Contribute & Bugs

Grom is open source so feel free to checkout (pun intended) [the repo](https://github.com/okanok/grom) to see the code, features being worked on and maybe even to create a PR yourself! We also have an [issue board](https://github.com/okanok/grom/projects/1) to have a nice overview of the issues we have and the ones being worked on. Bugs can be reported on the discussions page [here](https://github.com/okanok/grom/discussions).