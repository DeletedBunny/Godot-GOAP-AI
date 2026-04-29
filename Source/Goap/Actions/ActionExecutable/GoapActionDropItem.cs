using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.DropItem)]
public class GoapActionDropItem : GoapActionBase
{
    public override void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        IsInitialized = true;
    }

    public override void ExecuteAction(double deltaTime)
    {
        if (!Agent.IsHoldingAnyItemInHand())
        {
            return;
        }
        
        Agent.RemoveItemFromHand();
    }

    public override bool IsCompletedConditionMet()
    {
        return !Agent.IsHoldingAnyItemInHand();
    }
}