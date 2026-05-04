using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.GeneralDeliverResourceToBuildZone)]
public class GoapActionDeliverResourceToBuildingZone : GoapActionBase
{
    public override void InitializeTargetProvider(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isBuildingZoneInWorld = worldStateModel.GetPhysicalState(PreconditionsComponent.RequiredEntity) > 0;

        if (!isBuildingZoneInWorld)
            return;
        
        InitializeTargetProviderInternal(worldStateModel, previousAction?.GetTarget(), PreconditionsComponent.RequiredEntity, false);
        IsInitialized = true;
    }

    public override void ExecuteAction(double deltaTime)
    {
        base.ExecuteAction(deltaTime);
        
        if (!IsNearPosition() || !Agent.IsHoldingAnyItemInHand())
        {
            return;
        }
        
        Agent.TransferItemToTarget(Target);
    }

    public override bool IsCompletedConditionMet()
    {
        return !Agent.IsHoldingAnyItemInHand();
    }
}