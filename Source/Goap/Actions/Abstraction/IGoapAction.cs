using Godot;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    GoapActionType Type { get; }
    GoapActionDataComponent ActionDataComponent { get; }
    GoapActionPreconditionComponent ActionPreconditionsComponent { get; }
    GoapActionEffectComponent ActionEffectsComponent { get; }
    void Initialize(Agent3D agent, GoapActionDataComponent actionData, GoapActionPreconditionComponent actionPreconditions, GoapActionEffectComponent actionEffects);
    void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType);
    Node3D GetTarget();
    float CalculateCost();
    void ExecuteAction(double deltaTime);
    bool IsCompletedConditionMet();
}