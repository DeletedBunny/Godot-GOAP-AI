using Godot;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    void Initialize(GoapActionParams goapActionParams);
    int CalculateCost();
    bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult);
    GoapActionResult GetActionResult();
    void ExecuteAction(double deltaTime);
    bool IsCompletedConditionMet();
}