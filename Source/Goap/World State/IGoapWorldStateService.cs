using Godot;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;

namespace GodotGOAPAI.Source.Goap.World_State;

public interface IGoapWorldStateService
{
    GoapWorldStateModel<Node3D> GetWorldStateMemento();
    void ApplyWorldStateMemento();
}