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
        return true;
    }

    public Node3D GetTarget()
    {
        return null;
    }

    public float CalculateCost()
    {
        return 0f;
    }

    public void ExecuteAction(double deltaTime)
    {
        throw new Exception("This action is not executable");
    }

    public bool IsCompletedConditionMet()
    {
        return true;
    }
}