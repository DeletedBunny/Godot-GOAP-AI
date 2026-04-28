using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.PickUpAxe)]
public class GoapActionPickUpAxe : GoapActionBase
{
    public override void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        var isAxeInWorld = worldStateModel.GetState(EntityType.Axe) > 0;

        if (!isAxeInWorld)
            return;
        
        InitializeTargetInternal(worldStateModel, previousAction?.GetTarget(), EntityType.Axe, true);
    }

    public override void ExecuteAction(double deltaTime)
    {
        if (!IsNearPosition())
        {
            return;
        }
        
        var target = Target as IPickupEntity;
        Agent.AddItemToHand(target);
    }

    public override bool IsCompletedConditionMet()
    {
        return Agent.HasEntityTypeInHand(EntityType.Axe);
    }
}