using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.PickUpAxe)]
public class GoapActionPickUpAxe : GoapActionBase
{
    public override bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        IGoapAction previousAction)
    {
        var isAxeInWorld = worldStateMemento.GetResource(EntityType.Axe).Count > 0;

        var success = InitializeTarget(worldStateMemento, previousAction?.GetTarget(), EntityType.Axe, isAxeInWorld);
        if (success)
        {
            
            worldStateMemento.AddModifiedResource(EntityType.Axe, [Target], true);
        }
        
        return isAxeInWorld;
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