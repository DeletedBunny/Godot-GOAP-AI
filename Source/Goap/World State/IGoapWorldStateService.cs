using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.World_State;

public interface IGoapWorldStateService
{
    GoapWorldStateModel<Node3D> GetWorldStateMemento();
    void ApplyWorldStateMemento();
}