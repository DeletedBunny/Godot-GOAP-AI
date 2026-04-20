using System;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Agent;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionRegistration
{
    private Func<Agent3D, GoapActionDataComponent, GoapActionPreconditionComponent, GoapActionEffectComponent, IGoapAction> _actionFunc;
    public readonly GoapActionPreconditionComponent ActionPreconditions;
    public readonly GoapActionEffectComponent ActionEffects;
    public readonly GoapActionDataComponent ActionData;

    public GoapActionRegistration(
        Func<Agent3D, GoapActionDataComponent, GoapActionPreconditionComponent, GoapActionEffectComponent, IGoapAction> actionFunc, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions, 
        GoapActionEffectComponent actionEffects)
    {
        _actionFunc = actionFunc;
        ActionPreconditions = actionPreconditions;
        ActionEffects = actionEffects;
        ActionData = actionData;
    }

    public IGoapAction CreateAction(Agent3D agent)
    {
        return _actionFunc.Invoke(agent, ActionData, ActionPreconditions, ActionEffects);
    }
}