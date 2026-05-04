using Godot;
using GodotGOAPAI.Source.Goap.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public interface IGoapAction
{
    GoapActionType Type { get; }
    bool IsInitialized { get; }
    GoapActionDataComponent DataComponent { get; }
    GoapActionPreconditionComponent PreconditionsComponent { get; }
    GoapActionEffectComponent EffectsComponent { get; }
    void Initialize(
        IAgentActionable agent, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions, 
        GoapActionEffectComponent actionEffects);
    void InitializeTargetProvider(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType);
    Node3D GetTarget();
    void ExecuteAction(double deltaTime);
    bool IsCompletedConditionMet();
}