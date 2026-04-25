using Godot;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    GoapActionType Type { get; }
    GoapActionDataComponent ActionDataComponent { get; }
    GoapActionPreconditionComponent ActionPreconditionsComponent { get; }
    GoapActionEffectComponent ActionEffectsComponent { get; }
    void Initialize(Agent3D agent, GoapActionDataComponent actionData, GoapActionPreconditionComponent actionPreconditions, GoapActionEffectComponent actionEffects);
    bool InitializeTarget(GoapWorldStateMemento worldStateMemento, IGoapAction previousAction);
    Node3D GetTarget();
    int CalculateCost();
    void ExecuteAction(double deltaTime);
    bool IsCompletedConditionMet();
}