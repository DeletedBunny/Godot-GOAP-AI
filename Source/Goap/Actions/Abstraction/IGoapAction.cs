using Godot;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    GoapActionDataComponent ActionData { get; }
    GoapActionPreconditionComponent ActionPreconditions { get; }
    GoapActionEffectComponent ActionEffects { get; }
    bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento, IGoapAction previousAction);
    Node3D GetTarget();
    void ExecuteAction(double deltaTime);
    bool IsCompletedConditionMet();
}