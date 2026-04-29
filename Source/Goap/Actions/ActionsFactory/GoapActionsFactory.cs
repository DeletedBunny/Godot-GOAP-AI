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
using GodotGOAPAI.Source.Goap.WorldState;
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
                         .AddEffect("NearEntity", 1)
                         .Build<GoapActionMove>(this);
        GoapActionBuilder.Create(GoapActionType.CutTree)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 1,
                             TimeCostInSeconds = 4,
                         })
                         .AddRequiredLocationType(EntityType.Tree)
                         .AddPrecondition("HasAxe", 1)
                         .AddPrecondition("TreeInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("LogInWorld", 2)
                         .AddEffect("TreeInWorld", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionCutTree>(this);
        GoapActionBuilder.Create(GoapActionType.MineMountain)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 1,
                             TimeCostInSeconds = 6
                         })
                         .AddRequiredLocationType(EntityType.Mountain)
                         .AddPrecondition("HasHammer", 1)
                         .AddPrecondition("MountainInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("StoneInWorld", 4)
                         .AddEffect("MountainInWorld", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionMineMountain>(this);
        GoapActionBuilder.Create(GoapActionType.DropItem)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.1f,
                             TimeCostInSeconds = 0.1f
                         })
                         .AddPrecondition("HasEmptyHands", 0)
                         .AddEffect("HasEmptyHands", 1)
                         .Build<GoapActionDropItem>(this);
        GoapActionBuilder.Create(GoapActionType.PickUpAxe)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.8f,
                             TimeCostInSeconds = 2
                         })
                         .AddRequiredLocationType(EntityType.Axe)
                         .AddPrecondition("HasEmptyHands", 1)
                         .AddPrecondition("AxeInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasAxe", 1)
                         .AddEffect("AxeInWorld", -1)
                         .AddEffect("HasEmptyHands", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionPickUpAxe>(this);
        GoapActionBuilder.Create(GoapActionType.PickUpHammer)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.8f,
                             TimeCostInSeconds = 2
                         })
                         .AddRequiredLocationType(EntityType.Hammer)
                         .AddPrecondition("HasEmptyHands", 1)
                         .AddPrecondition("HammerInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasHammer", 1)
                         .AddEffect("HammerInWorld", -1)
                         .AddEffect("HasEmptyHands", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionPickUpHammer>(this);
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

    public List<IGoapAction> GetMatchingActionsByEffect(string actionResultKey, EntityType requiredEntity, Agent3D agent)
    {
        var actionMap = _goapActions.Where(item => 
                item.Value.ActionEffects.ContainsEffect(actionResultKey) && item.Value.ActionEffects.Effects.Find(kvp => kvp.Key.Equals(actionResultKey)).Value >= 0)
            .Select(item => GetAction(item.Key, agent))
            .ToList();
        
        var actionDropItem = actionMap.FirstOrDefault(action => action.Type == GoapActionType.DropItem);
        if (actionDropItem != null)
        {
            actionDropItem.EffectsComponent.Effects.Add(new (GoapWorldStateConstants.HasModifierPrefix + requiredEntity, -1));
            actionDropItem.EffectsComponent.Effects.Add(new (requiredEntity + GoapWorldStateConstants.InWorldModifierPostfix, 1));
        }
        
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
        actionInstance.EffectsComponent.Effects.Add(new(GoapWorldStateConstants.NearEntityKey + typeToMoveTo, 1));
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