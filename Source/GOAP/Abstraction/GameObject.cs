using Godot;

namespace GodotGOAPAI.Source.GOAP.Abstraction;

public class GameObject<T>
{
    public T Self;

    public GameObject(T node)
    {
        Self = node;
    }
}