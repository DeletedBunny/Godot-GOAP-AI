using System;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionRegistration
{
    private readonly Func<IAgentPlanner, GoapActionDataComponent, GoapActionPreconditionComponent, GoapActionEffectComponent, IGoapAction> _actionFunc;
    private readonly GoapActionPreconditionComponent _actionPreconditions;
    private readonly GoapActionEffectComponent _actionEffects;
    private readonly GoapActionDataComponent _actionData;
    
    public IGoapActionEffectComponent ActionEffects => _actionEffects;

    public GoapActionRegistration(
        Func<IAgentPlanner, GoapActionDataComponent, GoapActionPreconditionComponent, GoapActionEffectComponent, IGoapAction> actionFunc, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions, 
        GoapActionEffectComponent actionEffects)
    {
        _actionFunc = actionFunc;
        _actionPreconditions = actionPreconditions;
        _actionEffects = actionEffects;
        _actionData = actionData;
    }

    public IGoapAction CreateAction(IAgentPlanner agent)
    {
        return _actionFunc.Invoke(agent, _actionData, _actionPreconditions, _actionEffects);
    }
}