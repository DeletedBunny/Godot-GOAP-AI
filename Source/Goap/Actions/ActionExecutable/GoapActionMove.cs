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
    public override void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        if (moveToType == EntityType.None) 
            return;
        
        InitializeTargetInternal(worldStateModel, previousAction?.GetTarget(), moveToType, false);

        if (Target == null) 
            return;
        
        ActionDataComponent.TimeCostInSeconds = Agent.GlobalPosition.DistanceSquaredTo(Target.GlobalPosition);
        ActionEffectsComponent.Effects.Add(new KeyValuePair<string, int>("Near" + moveToType, 1));
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