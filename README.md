# Grom
Grom, a .NET graph database object relational mapper

Grom is an easy to use, low config ORM that lets you map your C# classes to nodes and relationships in various graph databases.
This project is currently not production ready as its still in beta. Version 1.0 is expected to be released Q4 2022. 

Since this project is open source and currently worked on by only the maintainer a 'stabilizing' phase was needed in which a beta version is released so bugs can be found and fixed and major missing features can be requested before an actual production ready version is released.

## Getting started

Configuring Grom is easy, it only requires a database connection to be given to GromGraph.CreateConnection(). This class will act as a singleton and use the given connection in the background. You don't need to instantiate this class or call it anywhere.

### Neo4J
To configure Grom for Neo4J simply use:
```
GromGraph.CreateConnection(new GromNeo4jConnector("bolt://localhost:7687", "neo4j", "test"));
```
Any valid instance of GromNeo4jConnector can be passed, so you are not restricted to username/password authentication.

## Features

### mapping a class
To map a class as a node you have to do two things: inherit from EntityNode and annotate each property you want to map with NodeProperty.

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
Currently integers, booleans, strings, floats and longs are supported. Version 1.0 will also support Dates and DateTimes.

### persisting an object

To persist a node simply call Persist() on any class you mapped.
```
var personNode = new Person("John", 25);
await personNode.Persist();
```
Grom is fully asynchronous so make sure you await when required. 

### updating an object

Updating an node is also done by calling Persist(). Grom will figure out if the node is allready created or not. 
```
var personNode = new Person("John", 25);
await personNode.Persist();

personNode.Age = 30;
await personNode.Persist();
```
Note that Grom only knows that an node exists if you have called Persist() on it or have retrieved it with Retrieve. It wont check if a node exists with the same properties in the database! 

### deleting a node

A node can be deleted by calling DeleteNode().
```
await personNode.DeleteNode();
```
The actual object in your code will still exist but the node and all its relationships to other nodes will be deleted in the database. So you could call Persist() again. and keep using this object.

### retrieving a node

Nodes can be retrieved using Retrieve\<T>. Nodes can be filtered by simply giving a lambda function that has a single parameter (the source node) and returns a boolean. Grom turns the lambda function into a query for you.
```
var personNode = await Retrieve<Person>
    .Where(p => p.Name == "John")
    .GetSingle();
```    
Boolean operators such as &&, ||, !, ==, !=, >, <, >= and <= are supported. Properties can be compared to constants, references to variables, method calls and properties in objects. Do note however that Grom can't turn everything a lambda can do into a query. Try to keep the lambda simple.

### relationships

## Supported Databases

Currently only Neo4J is supported. With the release of version 1.0 Azure Cosmos DB will also be supported.

## Docs

Comming soon.

## Contribute & Bugs

Grom is open source so feel free to checkout (pun intended) [the repo](https://github.com/okanok/grom) to see the code, features being worked on and maybe even create a PR yourself! We also have an [issue board](https://github.com/okanok/grom/projects/1) to have a nice overview of the issues we have. Bugs can also be reported in that GitHub project.