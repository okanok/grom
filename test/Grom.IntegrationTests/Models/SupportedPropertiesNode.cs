using Grom.Entities;
using Grom.Entities.Attributes;

namespace Grom.IntegrationTests.Models;

public class SupportedPropertiesNode : EntityNode
{
    [NodeProperty]
    public string StringProp { get; set; }

    [NodeProperty]
    public int IntProp { get; set; }

    [NodeProperty]
    public bool BoolProp { get; set; }

    [NodeProperty]
    public float FloatProp { get; set; }

    [NodeProperty]
    public long LongProp { get; set; }

    [NodeProperty]
    public DateTime DateTimeProp { get; set; }

    [NodeProperty]
    public DateOnly DateOnlyProp { get; set; }
}
