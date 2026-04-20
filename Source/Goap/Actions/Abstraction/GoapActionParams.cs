using GodotGOAPAI.Source.Goap.Agent;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public class GoapActionParams
{
    public Agent3D Agent { get; private set; }
    
    public GoapActionParams(Agent3D agent)
    {
        Agent = agent;
    }
}