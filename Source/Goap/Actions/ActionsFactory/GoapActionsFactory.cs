using System;
using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionExecutable;
using GodotGOAPAI.Source.Goap.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionsFactory : IGoapActionsFactory, IGoapActionsFactoryBuilding
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
                             TimeCostMultiplier = 20f,
                             TimeCostInSeconds = 1f
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
        GoapActionBuilder.Create(GoapActionType.DeliverLogToBuildZone)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.1f,
                             TimeCostInSeconds = 0.1f
                         })
                         .AddRequiredLocationType(EntityType.BuildingZone)
                         .AddPrecondition("HasLog", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasLog", -1)
                         .AddEffect("LogInBuildZone", 1)
                         .AddEffect("HasEmptyHands", 1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionDeliverResourceToBuildingZone>(this);
        GoapActionBuilder.Create(GoapActionType.DeliverStoneToBuildZone)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.1f,
                             TimeCostInSeconds = 0.1f
                         })
                         .AddRequiredLocationType(EntityType.BuildingZone)
                         .AddPrecondition("HasStone", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasStone", -1)
                         .AddEffect("StoneInBuildZone", 1)
                         .AddEffect("HasEmptyHands", 1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionDeliverResourceToBuildingZone>(this);
        GoapActionBuilder.Create(GoapActionType.PickupLog)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.8f,
                             TimeCostInSeconds = 2
                         })
                         .AddRequiredLocationType(EntityType.Log)
                         .AddPrecondition("HasEmptyHands", 1)
                         .AddPrecondition("LogInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasLog", 1)
                         .AddEffect("LogInWorld", -1)
                         .AddEffect("HasEmptyHands", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionPickupLog>(this);
        GoapActionBuilder.Create(GoapActionType.PickupStone)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 0.8f,
                             TimeCostInSeconds = 2
                         })
                         .AddRequiredLocationType(EntityType.Stone)
                         .AddPrecondition("HasEmptyHands", 1)
                         .AddPrecondition("StoneInWorld", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HasStone", 1)
                         .AddEffect("LogInWorld", -1)
                         .AddEffect("HasEmptyHands", -1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionPickupStone>(this);
        GoapActionBuilder.Create(GoapActionType.BuildHomeA)
                         .AddData(new()
                         {
                             TimeCostMultiplier = 1f,
                             TimeCostInSeconds = 10
                         })
                         .AddRequiredLocationType(EntityType.BuildingZone)
                         .AddPrecondition("LogInBuildZone", 3)
                         .AddPrecondition("StoneInBuildZone", 2)
                         .AddPrecondition("HasHammer", 1)
                         .AddPrecondition("NearEntity", 1)
                         .AddEffect("HomeAInWorld", 1)
                         .AddEffect("NearEntity", -1)
                         .Build<GoapActionBuildZone>(this);
    }

    public void AddAction(GoapActionType actionType, GoapActionRegistration actionRegistration)
    {
        _goapActions.Add(actionType, actionRegistration);
    }

    private IGoapAction GetAction(GoapActionType type, IAgentPlanner agent)
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
    
    public List<GoapPlanningAction> GetMatchingActionsWithAmount(
        KeyValuePair<string, int> actionPrecondition, 
        GoapWorldStateModel worldStateModel, 
        IAgentPlanner agent)
    {
        var entityType = EntityType.None;
        if (actionPrecondition.Key.Equals(GoapWorldStateConstants.AgentHasEmptyHandsKey) && actionPrecondition.Value > 0)
            entityType = worldStateModel.GetEntityStringFromPartialKey(GoapWorldStateConstants.HasModifierPrefix, [GoapWorldStateConstants.AgentHasEmptyHandsKey]);
        var matchingActions = GetMatchingActionsByEffect(actionPrecondition.Key, entityType, agent);

        var goapPlanningActions = matchingActions.Select<IGoapAction, GoapPlanningAction>(action =>
        {
            if (action.Type == GoapActionType.MoveTo)
                return new () { ActionInstance = action, RepeatCount = 1 };
            
            var effect = action.EffectsComponent.Effects.First(kvp => kvp.Key.Equals(actionPrecondition.Key));
            var neededValue = actionPrecondition.Value - worldStateModel.GetState(actionPrecondition.Key);
            var repeatCount = (int)Math.Ceiling(neededValue / (float)effect.Value);
            return new() { ActionInstance = action, RepeatCount = repeatCount };
        }).ToList();

        return goapPlanningActions;
    }

    private List<IGoapAction> GetMatchingActionsByEffect(string actionResultKey, EntityType requiredEntity, IAgentPlanner agent)
    {
        var actionMap = _goapActions
                        .Where(a => FilterActionWithSpecialCases(a, actionResultKey))
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

    public IGoapAction CreateAction<TAction>(
        IAgentPlanner agent, 
        GoapActionDataComponent actionData, 
        GoapActionPreconditionComponent actionPreconditions, 
        GoapActionEffectComponent actionEffects) 
        where TAction : IGoapAction, new()
    {
        var action = new TAction();
        var agentActionable = agent as IAgentActionable;
        action.Initialize(agentActionable, actionData, actionPreconditions, actionEffects);
        return action;
    }

    private bool FilterActionWithSpecialCases(KeyValuePair<GoapActionType, GoapActionRegistration> action, string actionResultKey)
    {
        var isActionContainsEffect = action.Value.ActionEffects.ContainsEffect(actionResultKey);
        var isEffectValuePositive = action.Value.ActionEffects.Effects.Find(kvp => kvp.Key.Equals(actionResultKey)).Value >= 0;
        var isActionSpecialCase = false;
        if (actionResultKey.Equals(GoapWorldStateConstants.AgentHasEmptyHandsKey))
        {
            isActionSpecialCase = action.Key is GoapActionType.DeliverLogToBuildZone or GoapActionType.DeliverStoneToBuildZone;
        }
        
        return isActionContainsEffect && isEffectValuePositive && !isActionSpecialCase;
    }
}