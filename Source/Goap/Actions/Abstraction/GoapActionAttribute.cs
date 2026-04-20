using System;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

[AttributeUsage(AttributeTargets.Class)]
public class GoapActionAttribute : Attribute
{
    public GoapActionType Type { get; }
    
    public GoapActionAttribute(GoapActionType type)
    {
        Type = type;
    }
}