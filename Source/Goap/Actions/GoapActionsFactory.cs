using System;
using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionExecutable;

namespace GodotGOAPAI.Source.Goap.Actions;

public class GoapActionsFactory : IGoapActionsFactory
{
    private readonly Dictionary<GoapActionType, Func<GoapActionParams, IGoapAction>> _goapActions = new();
    
    // Use this like a factory, register all actions here once and then use them in the planner
    public void RegisterActions()
    {
        _goapActions.Add(GoapActionType.CutTree, CreateAction<GoapActionCutTree>);
        _goapActions.Add(GoapActionType.PickUpItem, CreateAction<GoapActionPickUpItem>);
    }

    public IGoapAction GetAction(GoapActionType type, GoapActionParams actionParams)
    {
        var successfulActionCreation = _goapActions.TryGetValue(type, out var actionCreationFunc);
        if (!successfulActionCreation)
            throw new Exception($"Action of type {type} not found");
        
        return actionCreationFunc(actionParams);
    }

    private IGoapAction CreateAction<TAction>(GoapActionParams actionParams) where TAction : GoapActionBase, new()
    {
        var action = new TAction();
        action.Initialize(actionParams);
        return action;
    }
}