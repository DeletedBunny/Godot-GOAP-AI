using System;
using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.MoveTo)]
public class GoapActionMove : GoapActionBase
{
    public override bool InitializeTarget(GoapWorldStateMemento worldStateMemento, IGoapAction previousAction)
    {
        if (previousAction != null && previousAction.ActionPreconditionsComponent.RequiredEntity != EntityType.None)
        {
            var dynamincTargetType = previousAction.ActionPreconditionsComponent.RequiredEntity;
            
            var closestNode = worldStateMemento.GetClosestElementByType(dynamincTargetType, Agent);
            if(closestNode == null)
                return false;
        
            InitializeTargetInternal(worldStateMemento, previousAction.GetTarget(), dynamincTargetType, true);
            ActionDataComponent.TimeCostInSeconds = Agent.GlobalPosition.DistanceSquaredTo(Target.GlobalPosition);
            ActionEffectsComponent.Effects.Add(new KeyValuePair<string, int>("Near" + dynamincTargetType, 1));
            
            return true;
        }
        
        return false;
    }

    public override void ExecuteAction(double deltaTime)
    {
        if (IsNearPosition()) 
            return;
        
        if (Target == null)
        {
            throw new Exception("Target is null and action is not completed yet, breaking...");
        }
        
        Agent.MoveTo(Target, deltaTime);
    }

    public override bool IsCompletedConditionMet() => IsNearPosition();
}