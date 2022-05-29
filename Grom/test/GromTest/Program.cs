﻿// See https://aka.ms/new-console-template for more information

using Grom;
using Grom.GraphDbConnectors.Neo4J;
using Grom.GromQuery;
using Grom.Util;
using GromTest.GraphTest.Nodes;

// Create connection to db using Neo4J connector
GromGraph.CreateConnection(new GromNeo4jConnector("bolt://localhost:7687", "neo4j", "test"));

////Create some nodes
//var personNode = new Person("John", 25);
//var hobbyNode = new Hobby(true);

////Persist nodes in the database
//await personNode.Persist();
//await hobbyNode.Persist();

//// Update node values
//personNode.Age = 30;
//hobbyNode.IsFun = false;

//// Save updates
//await personNode.Persist();
//await hobbyNode.Persist();

////Delete nodes
//await personNode.DeleteNode();
//await hobbyNode.DeleteNode();

////Create a relationship
//var personNode = new Person("John", 25);
//var personFriend = new Person("Doe", 26);
//personNode.knownPeople.Add(new Knows(personFriend, 10));
//await personNode.Persist();

////Updating relationship
//personNode.knownPeople.First().ForYears = 11;
//await personNode.Persist();

//// Deleting relationship
//await personNode.knownPeople.First().Delete();
//personNode.knownPeople.Remove(personNode.knownPeople.First());

//Retrieving a node from the database
//var personNode = new Person("John", 25);
//await personNode.Persist();

//var c1 = await Retrieve<Person>
//    .Where(
//        Logical.And(
//            Constraint.Property("Name", Comparison.Eq, "John")
//            , 
//            Constraint.Property("Age", Comparison.Eq, 25)
//        )
//    )
//    .GetSingle();

//var vari = 25;
var personNode = await Retrieve <Person>
    .Where(p => p.Name == "John")
//    //.Where(p => p.Age >= testClass.getA(24))
//    //.Where(p => p.Age >= testClass.getA())
//    //.Where(p => p.Age >= personNode.Age)
//    //.Where(p => p.Age >= vari)
//    //.Where(p => !(p.Name == "John"))
//    .Where(p => p.Age >= vari && p.Name == "John")
    .GetSingle();

//Supported types
//var typeTestNode = new PropertiesTestNode()
//{
//    StringProp = "test",
//    IntProp = 1,
//    BoolProp = false,
//    FloatProp = 1.001f,
//    LongProp = 1000000000000,
//};

//await typeTestNode.Persist();

var typeTestNodeRetrieved = await Retrieve<PropertiesTestNode>
    //.Where(n => n.IntProp == 1)
    //.Where(n => n.StringProp == "test")
    //.Where(n => n.BoolProp == false)
    //.Where(n => n.FloatProp == 1.001f)
    .Where(n => n.LongProp == 10)
    .GetSingle();

// TODO: add support for class property access in where expression
// TODO: add support for call to getter in where expression
// TODO: implement Neo4J Parser for query state
// TODO: add docs
// TODO: implement retrieving node with relationships
// TODO: support int, string, bool, double, long, char
// TODO: add logging
// TODO: force empty constructor
// TODO: check all features for release
// TODO: support directed and undirected nodes?
// TODO: finish polymorphic relationship implementation ?? might not be needed anymore

//QueryBase = Empty || Constr
//Constr = PrefixConstr || InfixConstr || PropertyConstr
//PrefixConstr = Op1 + Constr
//InfixConstr = Constr + Op2 + Constr
//PropertyConstr = NodeProperty + EqOp + ValueAccess || ValueAccess + EqOp + NodeProperty
//ValueAccess = NodeProperty || Constant || Var || Property || MethodallNoParams
//EqOp = Eq || Neq || Geq || Leq || Ge || Le
//Op2 = OR || AND
//Op1 = NOT


var d = 1;

//class Test
//{
//    private int a = 10;

//    public int getA()
//    {
//        return a;
//    }

//    public int getA(int b)
//    {
//        return b;
//    }
//}