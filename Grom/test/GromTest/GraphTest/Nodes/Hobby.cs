using Grom.Entities;
using Grom.Entities.Attributes;

namespace GromTest.GraphTest.Nodes;

public class Hobby : EntityNode
{
    [NodeProperty]
    public bool IsFun { get; set; }

    public Hobby()
    {
    }

    public Hobby(bool isFun)
    {
        IsFun = isFun;
    }
}
