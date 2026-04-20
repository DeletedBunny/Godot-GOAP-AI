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
    private EntityType _dynamincTargetType = EntityType.Unknown;
    
    public override bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        IGoapAction previousAction)
    {
        if (previousAction != null && previousAction.ActionPreconditions.RequiredEntity != EntityType.None)
        {
            _dynamincTargetType = previousAction.ActionPreconditions.RequiredEntity;
            
            var closestNode = worldStateMemento.GetClosestElementByType(_dynamincTargetType, Agent);
            if(closestNode == null)
                return false;
        
            InitializeTarget(worldStateMemento, previousAction.GetTarget(), _dynamincTargetType, true);
            ActionData.TimeCostInSeconds = Agent.GlobalPosition.DistanceSquaredTo(Target.GlobalPosition);
            ActionEffects.Effects.Add(new KeyValuePair<string, int>("Near" + _dynamincTargetType, 1));
            
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