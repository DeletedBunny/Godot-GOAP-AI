using Godot;
using GodotGOAPAI.Source.GOAP.Actions.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Actions.MoveTo;

public class GoapMoveToActionParams : IGoapActionParams
{
    public Node3D Target { get; private set; }
    public Node3D Agent { get; private set; }
    public int AgentMoveSpeed  { get; private set; }
    
    public GoapMoveToActionParams(Node3D target, Node3D agent, int agentMoveSpeed)
    {
        Target = target;
        Agent = agent;
        AgentMoveSpeed = agentMoveSpeed;
    }
}