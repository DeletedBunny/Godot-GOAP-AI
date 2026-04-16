using Godot;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    void Initialize(GoapActionParamsBase goapActionParams);
    int CalculateCost();
    bool IsActionPreconditionsValid(Node3D agent, GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult);
    GoapActionResult GetActionResult();
    void ExecuteAction(float deltaTime);
    bool IsCompletedConditionMet();
}