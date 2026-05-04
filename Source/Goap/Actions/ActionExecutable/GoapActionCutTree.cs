using System;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.CutTree)]
public class GoapActionCutTree : GoapActionBase
{
    public override void InitializeTargetProvider(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isTreeInWorld = worldStateModel.GetPhysicalState(PreconditionsComponent.RequiredEntity) > 0;

        if (!isTreeInWorld)
            return;
        
        InitializeTargetProviderInternal(worldStateModel, previousAction?.GetTarget(), PreconditionsComponent.RequiredEntity, true);
        IsInitialized = true;
    }

    public override void ExecuteAction(double deltaTime)
    {
        base.ExecuteAction(deltaTime);
        
        if (!IsNearPosition())
        {
            return;
        }
        
        var target = Target as IInteractableEntity;
        Agent.InteractOn(target, deltaTime);
    }
    
    public override bool IsCompletedConditionMet()
    {
        if (Target is IInteractableEntity target)
        {
            return target.IsEntityInteractionFinished;
        }

        throw new Exception("Target is not an interactable entity, breaking...");
    }
}