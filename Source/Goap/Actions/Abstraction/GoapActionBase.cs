using System;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public abstract class GoapActionBase : IGoapAction
{
    protected GoapActionDataComponent ActionData;
    protected Node3D Target { get; set; }
    protected Agent3D Agent { get; set; }

    public virtual void Initialize(GoapActionParams actionParams)
    {
        ActionData = new GoapActionDataComponent();
        Agent = actionParams.Agent;
    }

    public int CalculateCost()
    {
        return ActionData.CalculatedCost;
    }

    public abstract bool IsActionPreconditionsValid(GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult);
    public abstract GoapActionResult GetActionResult();

    public virtual void ExecuteAction(double deltaTime)
    {
        if (Target == null)
        {
            throw new Exception("Target is null and action is not completed yet, breaking...");
        }
        
        Agent.MoveTo(Target, deltaTime);
    }

    protected bool ReachedTarget()
    {
        return Agent.ReachedPosition(Target);
    }

    public abstract bool IsCompletedConditionMet();
}