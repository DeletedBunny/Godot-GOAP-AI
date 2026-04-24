using System;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.CutTree)]
public class GoapActionCutTree : GoapActionBase
{
    public override bool IsActionPreconditionsValid(GoapWorldStateMemento worldStateMemento,
        IGoapAction previousAction)
    {
        var isTreeInWorld = worldStateMemento.GetResource(EntityType.Tree).Count > 0;
        var previousActionResults = previousAction?.ActionEffects.Effects;
        var isAxeAsResult = previousActionResults?.FirstOrDefault(x => x.Key == "HasAxe").Value > 0;
        var isAxeOnAgent = Agent.HasEntityTypeInHand(EntityType.Axe);

        var success = InitializeTarget(worldStateMemento, previousAction?.GetTarget(), EntityType.Tree, isTreeInWorld && (isAxeAsResult || isAxeOnAgent));
        if (success)
        {
            worldStateMemento.AddModifiedResource(EntityType.Tree, [Target], true);
        }
        
        return isTreeInWorld && (isAxeAsResult || isAxeOnAgent);
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