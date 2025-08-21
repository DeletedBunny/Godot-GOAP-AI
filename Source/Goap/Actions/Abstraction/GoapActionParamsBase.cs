using Godot;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public abstract class GoapActionParamsBase
{
    public Node3D Target { get; private set; }
    public Node3D Agent { get; private set; }
    public int AgentMoveSpeed  { get; private set; }
    
    public GoapActionParamsBase(Node3D target, Node3D agent, int agentMoveSpeed)
    {
        Target = target;
        Agent = agent;
        AgentMoveSpeed = agentMoveSpeed;
    }
}