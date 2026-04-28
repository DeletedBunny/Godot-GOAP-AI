using System;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.CutTree)]
public class GoapActionCutTree : GoapActionBase
{
    public override void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isTreeInWorld = worldStateModel.GetState(EntityType.Tree) > 0;

        if (!isTreeInWorld)
            return;
        
        InitializeTargetInternal(worldStateModel, previousAction?.GetTarget(), EntityType.Tree, true);
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