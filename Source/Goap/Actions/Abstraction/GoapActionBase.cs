using System;
using Godot;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public abstract class GoapActionBase : IGoapAction
{
    public GoapActionDataComponent ActionData { get; private set; }
    public GoapActionPreconditionComponent ActionPreconditions { get; private set; }
    public GoapActionEffectComponent ActionEffects { get; private set; }
    protected Node3D Target { get; private set; }
    protected Agent3D Agent { get; private set; }

    public void Initialize(
        Agent3D agent, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions,
        GoapActionEffectComponent actionEffects)
    {
        ActionData = actionData;
        ActionPreconditions = actionPreconditions;
        ActionEffects = actionEffects;
        Agent = agent;
    }

    public abstract bool IsActionPreconditionsValid(GoapWorldStateMemento worldStateMemento,
        IGoapAction previousAction);
    public abstract void ExecuteAction(double deltaTime);
    public abstract bool IsCompletedConditionMet();

    protected bool InitializeTarget(GoapWorldStateMemento worldStateMemento, Node3D previousTarget, EntityType entityType,  bool conditionToMeet)
    {
        if (conditionToMeet)
        {
            var startNode = previousTarget ?? Agent;
            var closestNode = worldStateMemento.GetClosestElementByType(entityType, startNode);
            Target = closestNode;
        }
        return conditionToMeet;
    }
    
    public int CalculateCost()
    {
        return ActionData.CalculatedCost;
    }
    
    public Node3D GetTarget()
    {
        return Target;
    }
    
    protected bool IsNearPosition()
    {
        return Agent.IsNearPosition(Target);
    }
}