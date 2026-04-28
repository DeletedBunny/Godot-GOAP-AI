using System;
using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.Goap.Actions.ActionExecutable;
using GodotGOAPAI.Source.Goap.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionsFactory : IGoapActionsFactory
{
    private readonly Dictionary<GoapActionType, GoapActionRegistration> _goapActions = new();
    
    // Use this like a factory, register all actions here once and then use them in the planner
    public void RegisterActions()
    {
        GoapActionBuilder.Create(GoapActionType.PlanningTreeRoot)
            .Build<GoapGoal>(this);
        GoapActionBuilder.Create(GoapActionType.MoveTo)
            .AddData(new()
            {
                TimeCostMultiplier = 1,
                TimeCostInSeconds = 0
            })
            .Build<GoapActionMove>(this);
        GoapActionBuilder.Create(GoapActionType.CutTree)
            .AddData(new()
            {
                TimeCostMultiplier = 1,
                TimeCostInSeconds = 5,
            })
            .AddRequiredLocationType(EntityType.Tree)
            .AddPrecondition("HasAxe", 1)
            .AddPrecondition("TreeInWorld", 1)
            .AddPrecondition("NearTree", 1)
            .AddEffect("LogInWorld", 2)
            .AddEffect("TreeInWorld", -1)
            .Build<GoapActionCutTree>(this);
        GoapActionBuilder.Create(GoapActionType.PickUpAxe)
            .AddData(new()
            {
                TimeCostMultiplier = 0.8f,
                TimeCostInSeconds = 2
            })
            .AddRequiredLocationType(EntityType.Axe)
            .AddPrecondition("AxeInWorld", 1)
            .AddPrecondition("NearAxe", 1)
            .AddEffect("HasAxe", 1)
            .AddEffect("AxeInWorld", -1)
            .Build<GoapActionPickUpAxe>(this);
    }

    public void AddAction(GoapActionType actionType, GoapActionRegistration actionRegistration)
    {
        _goapActions.Add(actionType, actionRegistration);
    }

    public IGoapAction GetAction(GoapActionType type, Agent3D agent)
    {
        if (type == GoapActionType.PlanningTreeRoot)
            throw new Exception($"{type} action cannot be retrieved, use {nameof(GetGoal)} instead.");
        
        var actionExists = _goapActions.TryGetValue(type, out var actionRegistration);
        
        if (!actionExists)
            throw new Exception($"Action of type {type} not found");
        
        return actionRegistration.CreateAction(agent);
    }

    public IGoapAction GetGoal(GoapActionPreconditionComponent preconditionComponent)
    {
        var actionRegistration = _goapActions[GoapActionType.PlanningTreeRoot];
        var actionInstance = actionRegistration.CreateAction(null);
        actionInstance.Initialize(null, null, preconditionComponent, null);
        return actionInstance;
    }

    public List<GoapPlanningAction> GetMatchingActionsByEffect(string actionResultKey)
    {
        var actionMap = _goapActions.Where(item => 
                item.Value.ActionEffects.ContainsEffect(actionResultKey))
            .Select(item => new GoapPlanningAction()
            {
                Type = item.Key,
                EffectsComponent = item.Value.ActionEffects,
                PreconditionsComponent = item.Value.ActionPreconditions,
                RepeatCount = 0
            })
            .ToList();
        
        if (actionMap.Count == 0)
            throw new Exception($"Action with result key {actionResultKey} not found");
        
        return actionMap;
    }

    public IGoapAction GetMoveToAction(EntityType typeToMoveTo, Agent3D agent)
    {
        _goapActions.TryGetValue(GoapActionType.MoveTo, out var actionRegistration);
        
        if (actionRegistration == null)
            throw new Exception("MoveTo action not found");
        
        var actionInstance = actionRegistration.CreateAction(agent);
        actionInstance.ActionEffectsComponent.Effects.Add(new("Near" + typeToMoveTo, 1));
        return actionInstance;
    }

    public IGoapAction CreateAction<TAction>(
        Agent3D agent, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions, 
        GoapActionEffectComponent actionEffects) 
        where TAction : IGoapAction, new()
    {
        var action = new TAction();
        action.Initialize(agent, actionData, actionPreconditions, actionEffects);
        return action;
    }
}