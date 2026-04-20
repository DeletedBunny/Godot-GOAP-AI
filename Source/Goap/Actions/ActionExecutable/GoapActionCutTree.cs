using System;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

public class GoapActionCutTree : GoapActionBase
{
    public override void Initialize(GoapActionParams actionParams)
    {
        base.Initialize(actionParams);
        ActionData.DistanceCostMultiplier = 1;
        ActionData.TimeCostMultiplier = 1;
        ActionData.TimeCostInSeconds = 5;
    }

    public override bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult)
    {
        var isTreeInWorld = worldStateMemento.GetResource(EntityType.Tree).Count > 0;
        var isAxeAsResult = actionResult.GetActionResults().FirstOrDefault(x => x.Key == "axe").Value > 0;
        var isAxeOnAgent = Agent.HasEntityTypeInHand(EntityType.Axe);

        if (isTreeInWorld && (isAxeAsResult || isAxeOnAgent))
        {
            var closestNode = worldStateMemento.GetClosestElementByType(EntityType.Tree, Agent);
            Target = closestNode;
            worldStateMemento.AddModifiedResource(EntityType.Tree, [closestNode], true);
        }
        
        return isTreeInWorld && (isAxeAsResult || isAxeOnAgent);
    }
    
    public override GoapActionResult GetActionResult()
    {
        var actionResult = new GoapActionResult();
        actionResult.AddActionResult("log", 2);
        return actionResult;
    }

    public override void ExecuteAction(double deltaTime)
    {
        if (!ReachedTarget())
        {
            base.ExecuteAction(deltaTime);
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