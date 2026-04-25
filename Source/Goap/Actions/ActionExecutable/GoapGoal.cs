using System;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

[GoapAction(GoapActionType.PlanningTreeRoot)]
public class GoapGoal : IGoapAction
{
    public GoapActionType Type 
    { 
        get
        {
            var attribute = Attribute.GetCustomAttribute(GetType(), typeof(GoapActionAttribute));
            if (attribute is GoapActionAttribute goapActionAttribute)
                return goapActionAttribute.Type;
            return GoapActionType.Unknown;
        } 
    }

    public GoapActionDataComponent ActionDataComponent { get; } = new();
    public GoapActionPreconditionComponent ActionPreconditionsComponent { get; private set; }
    public GoapActionEffectComponent ActionEffectsComponent { get; } = new();

    public void Initialize(Agent3D agent, GoapActionDataComponent actionData, GoapActionPreconditionComponent actionPreconditions,
        GoapActionEffectComponent actionEffects)
    {
        ActionPreconditionsComponent = actionPreconditions;
    }

    public bool InitializeTarget(GoapWorldStateMemento worldStateMemento, IGoapAction previousAction)
    {
        throw new System.NotImplementedException();
    }

    public Node3D GetTarget()
    {
        throw new System.NotImplementedException();
    }

    public int CalculateCost()
    {
        throw new NotImplementedException();
    }

    public void ExecuteAction(double deltaTime)
    {
        throw new System.NotImplementedException();
    }

    public bool IsCompletedConditionMet()
    {
        throw new System.NotImplementedException();
    }
}