using System;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

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
    
    public bool IsInitialized { get; protected set; }

    public GoapActionDataComponent DataComponent { get; } = new();
    public GoapActionPreconditionComponent PreconditionsComponent { get; private set; }
    public GoapActionEffectComponent EffectsComponent { get; } = new();

    public void Initialize(Agent3D agent, GoapActionDataComponent actionData, GoapActionPreconditionComponent actionPreconditions,
        GoapActionEffectComponent actionEffects)
    {
        PreconditionsComponent = actionPreconditions;
    }

    public void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType)
    {
        IsInitialized = true;
    }

    public Node3D GetTarget()
    {
        return null;
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