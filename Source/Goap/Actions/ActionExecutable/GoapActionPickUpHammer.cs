using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.PickUpHammer)]
public class GoapActionPickUpHammer : GoapActionBase
{
    public override void InitializeTargetProvider(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isHammerInWorld = worldStateModel.GetState(PreconditionsComponent.RequiredEntity + GoapWorldStateConstants.InWorldModifierPostfix) > 0;

        if (!isHammerInWorld)
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
        
        var target = Target as IPickupEntity;
        Agent.AddItemToHand(target);
    }

    public override bool IsCompletedConditionMet()
    {
        return Agent.IsEntityTypeInHand(PreconditionsComponent.RequiredEntity);
    }
}