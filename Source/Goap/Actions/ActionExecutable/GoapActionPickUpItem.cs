using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

public class GoapActionPickUpItem : GoapActionBase
{
    
    public override void Initialize(GoapActionParams actionParams)
    {
        base.Initialize(actionParams);
        ActionData.DistanceCostMultiplier = 0.5f;
        ActionData.TimeCostMultiplier = 0.8f;
        ActionData.TimeCostInSeconds = 5;
    }
    
    public override bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult)
    {
        var isAxeInWorld = worldStateMemento.GetResource(EntityType.Axe).Count > 0;
        var isAxeAsResult = actionResult.GetActionResults().FirstOrDefault(x => x.Key == "axe").Value > 0;

        if (isAxeInWorld)
        {
            var closestNode = worldStateMemento.GetClosestElementByType(EntityType.Axe, Agent);
            Target = closestNode;
            worldStateMemento.AddModifiedResource(EntityType.Axe, [closestNode], true);
        }
        
        return isAxeInWorld || isAxeAsResult;
    }

    public override GoapActionResult GetActionResult()
    {
        var actionResult = new GoapActionResult();
        actionResult.AddActionResult("axe", 1);
        return actionResult;
    }

    public override void ExecuteAction(double deltaTime)
    {
        if (!ReachedTarget())
        {
            base.ExecuteAction(deltaTime);
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