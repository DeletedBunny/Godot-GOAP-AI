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
    public GoapActionDataComponent DataComponent { get; private set; }
    public GoapActionPreconditionComponent PreconditionsComponent { get; private set; }
    public GoapActionEffectComponent EffectsComponent { get; private set; }
    protected Node3D Target { get; set; }
    protected Agent3D Agent { get; private set; }

    public void Initialize(
        Agent3D agent, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions,
        GoapActionEffectComponent actionEffects)
    {
        DataComponent = actionData;
        PreconditionsComponent = actionPreconditions;
        EffectsComponent = actionEffects;
        Agent = agent;
    }

    public abstract void InitializeTarget(GoapWorldStateModel worldStateModel, IGoapAction previousAction, EntityType moveToType);
    public abstract void ExecuteAction(double deltaTime);
    public abstract bool IsCompletedConditionMet();

    protected void InitializeTargetInternal(
        GoapWorldStateModel worldStateModel, 
        Node3D previousTarget, 
        EntityType entityType,
        bool shouldConsumeResource)
    {
        var startPosition = previousTarget?.GlobalPosition ?? Agent.GlobalPosition;
            Target = shouldConsumeResource 
                          ? worldStateModel.GetClosestAndRemove(entityType, startPosition) 
                          : worldStateModel.GetClosest(entityType, startPosition);
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