using Godot;
using GodotGOAPAI.Source.GOAP.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Godot;

public class GodotNode : IGameObject
{
    public Node Self;

    public GodotNode(Node node)
    {
        Self = node;
    }
}