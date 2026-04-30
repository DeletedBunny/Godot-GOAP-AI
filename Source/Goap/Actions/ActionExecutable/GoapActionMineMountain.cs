using System;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.MineMountain)]
public class GoapActionMineMountain : GoapActionBase
{
    public override void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isMountainInWorld = worldStateModel.GetPhysicalState(PreconditionsComponent.RequiredEntity) > 0;

        if (!isMountainInWorld)
            return;
        
        InitializeTargetInternal(worldStateModel, previousAction?.GetTarget(), PreconditionsComponent.RequiredEntity, true);
        IsInitialized = true;
    }

    public override void ExecuteAction(double deltaTime)
    {
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
            return target.IsEntityDestroyed;
        }

        throw new Exception("Target is not an interactable entity, breaking...");
    }
}