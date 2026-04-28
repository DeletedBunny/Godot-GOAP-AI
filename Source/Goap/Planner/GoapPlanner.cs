using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionExecutable;
using GodotGOAPAI.Source.Goap.Actions.ActionsFactory;
using GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanner
{
    private readonly IGoapActionsFactory _goapActionsFactory = new GoapActionsFactory();
    private readonly GoapPlannerExecutionQueue _goapPlannerExecutionQueue = new();

    public GoapPlanner()
    {
        _goapActionsFactory.RegisterActions();
    }

    public void Plan(Agent3D agent, GoapActionPreconditionComponent neededItemsToHave)
    {
        var worldStateMemento = GoapWorldStateService.Instance.GetWorldStateMemento();
        var worldStatePlanningModel = new GoapWorldStatePlanningModel(worldStateMemento);
        agent.GetAgentWorldState().ForEach(x => worldStatePlanningModel.SetTrackedState("Has" + x.Item1, x.Item2));

        var planningTree = BuildTree(_goapActionsFactory.GetGoal(neededItemsToHave), worldStatePlanningModel, agent);

        var finalPlanResult = ValidateTreeAndCalculateCost(planningTree.Root, worldStatePlanningModel, worldStateMemento, null);
        if (finalPlanResult.Cost < float.MaxValue)
        {
            worldStateMemento.ApplyModificationsToWorldState();
            GoapWorldStateService.Instance.ApplyWorldStateMemento();
            BuildExecutionQueue(planningTree.Root);
        }
    }

    private GoapPlanningTree BuildTree(IGoapAction goal, GoapWorldStatePlanningModel worldStatePlanningModel, Agent3D agent)
    {
        var root = new GoapPlanningLeaf(goal);
        var planningTree = new GoapPlanningTree(root);

        var unvisitedLeafs = new Queue<GoapPlanningLeaf>();
        unvisitedLeafs.Enqueue(planningTree.Root);

        while (unvisitedLeafs.Count > 0)
        {
            var currentLeaf = unvisitedLeafs.Dequeue();

            var unmetPreconditions = currentLeaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                                .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value &&
                                                              !kvp.Key.Contains("Near"))
                                                .ToList();

            if (unmetPreconditions.Count == 0)
                continue;

            foreach (var precondition in unmetPreconditions)
            {
                var matchingActions = GetMatchingActionsWithAmount(precondition);

                foreach (var action in matchingActions)
                {
                    for (int i = 0; i < action.RepeatCount; i++)
                    {
                        var actionInstance = _goapActionsFactory.GetAction(action.Type, agent);
                        var newLeaf = CreateLeaf(actionInstance);
                        
                        if (newLeaf.ActionInstance.ActionPreconditionsComponent.NeedsEntityNearby())
                        {
                            var moveToAction = _goapActionsFactory.GetMoveToAction(newLeaf.ActionInstance.ActionPreconditionsComponent.RequiredEntity, agent);
                            var moveToLeaf = CreateLeaf(moveToAction);
                            newLeaf.AddChild("Near" + newLeaf.ActionInstance.ActionPreconditionsComponent.RequiredEntity, moveToLeaf);
                        }
                        
                        currentLeaf.AddChild(precondition.Key, newLeaf);
                        unvisitedLeafs.Enqueue(newLeaf);
                    }
                }
            }
        }

        return planningTree;
    }

    private GoapValidationResult ValidateTreeAndCalculateCost(
        GoapPlanningLeaf leaf, 
        GoapWorldStatePlanningModel worldStatePlanningModel,
        GoapWorldStateMemento worldStateMemento, 
        IGoapAction previousAction)
    {
        GD.Print("Validating leaf: " + leaf.ActionInstance.Type);
        var unmetPreconditions = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                     .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value)
                                     .Select(kvp => kvp.Key)
                                     .OrderBy(x => x.Contains("Near") ? 1 : 0)
                                     .ToList();

        var preconditionsTotalCost = 0f;
        IGoapAction rollingContextAction = previousAction;

        foreach (var precondition in unmetPreconditions)
        {
            GD.Print("Unmet precondition: " + precondition);
            if (!leaf.Children.TryGetValue(precondition, out var children))
                return new GoapValidationResult() { Cost = float.MaxValue };

            var amountNeeded = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions.First(kvp => kvp.Key.Equals(precondition)).Value;

            var groupedActions = children.GroupBy(x => x.ActionInstance.Type);
            var bestGroupResult = new GoapValidationResult() { Cost = float.MaxValue };
            GoapWorldStatePlanningModel bestGroupWorldStatePlanningModel = null;

            foreach (var group in groupedActions)
            {
                GD.Print("Group: " + group.Key);
                var sandboxWorldState = worldStatePlanningModel.GetCopy();
                var groupCost = 0f;
                var groupGatheredAmount = 0;
                IGoapAction groupContextAction = rollingContextAction;
                List<GoapPlanningLeaf> groupSelectedLeafs = new();
                
                foreach (var child in group)
                {
                    if (groupGatheredAmount >= amountNeeded)
                        break;
                    
                    GD.Print("Child: " + child.ActionInstance.Type);

                    var result = ValidateTreeAndCalculateCost(child, sandboxWorldState, worldStateMemento, groupContextAction);

                    if (result.Cost >= float.MaxValue)
                        continue;
                    
                    groupCost += result.Cost;
                    groupContextAction = result.Action;
                    groupGatheredAmount += child.ActionInstance.ActionEffectsComponent.Effects.First(kvp => kvp.Key.Equals(precondition)).Value;
                    groupSelectedLeafs.Add(child);
                }
                
                GD.Print("Group Cost: " + groupCost);
                GD.Print("Group Gathered Amount: " + groupGatheredAmount);

                if (groupGatheredAmount < amountNeeded || groupCost >= bestGroupResult.Cost) 
                    continue;
                
                bestGroupResult = new GoapValidationResult() { Cost = groupCost, Action = groupContextAction };
                bestGroupWorldStatePlanningModel = sandboxWorldState;
                children.ForEach(x => x.IsResolvable = false);
                groupSelectedLeafs.ForEach(x => x.IsResolvable = true);
                var selectedLeafsString = string.Join(", ", groupSelectedLeafs.Select(x => x.ActionInstance.Type));
                GD.Print("Group Selected Leafs: " + selectedLeafsString);
            }

            if (bestGroupWorldStatePlanningModel == null || bestGroupResult.Cost >= float.MaxValue)
                return new GoapValidationResult() { Cost = float.MaxValue };
            
            preconditionsTotalCost += bestGroupResult.Cost;
            rollingContextAction = bestGroupResult.Action;
            worldStatePlanningModel.SyncState(bestGroupWorldStatePlanningModel);
        }

        var parentActionRequiredEntity = leaf.Parent?.ActionInstance.ActionPreconditionsComponent.RequiredEntity;
        var moveToType = leaf.ActionInstance.Type == GoapActionType.MoveTo 
                             ? parentActionRequiredEntity
                             : EntityType.None;
        
        leaf.ActionInstance.InitializeTarget(worldStateMemento, rollingContextAction, moveToType ?? EntityType.None);

        foreach (var effect in leaf.ActionInstance.ActionEffectsComponent.Effects)
        {
            if (effect.Key.Contains("Near"))
                continue;
            
            worldStatePlanningModel.UpdateState(effect.Key, effect.Value);
        }
        
        leaf.CachedTotalCost = leaf.CalculatedCost + preconditionsTotalCost;
        leaf.IsResolvable = true;
        GD.Print("Leaf: " + leaf.ActionInstance.Type + " | Cost: " + leaf.CachedTotalCost);
        
        return new GoapValidationResult() { Cost = leaf.CachedTotalCost, Action = leaf.ActionInstance };
    }

    private void BuildExecutionQueue(GoapPlanningLeaf leaf)
    {
        var sortedKeys = leaf.Children.Keys.OrderBy(x => x.Contains("Near") ? 1 : 0).ToList();
        
        foreach (var key in sortedKeys)
        {
            var children = leaf.Children[key];
            var selectedChildren = children.Where(x => x.IsResolvable);

            foreach (var child in selectedChildren)
            {
                BuildExecutionQueue(child);
            }
        }

        if (leaf.ActionInstance.Type == GoapActionType.PlanningTreeRoot)
            return;
        
        _goapPlannerExecutionQueue.AddToQueue(leaf.ActionInstance);
    }

    private GoapPlanningLeaf CreateLeaf(IGoapAction action)
    {
        return new GoapPlanningLeaf(action);
    }

    private List<GoapPlanningAction> GetMatchingActionsWithAmount(KeyValuePair<string, int> actionPrecondition)
    {
        var matchingActions = _goapActionsFactory.GetMatchingActionsByEffect(actionPrecondition.Key);

        matchingActions.ForEach(action =>
        {
            var effect = action.EffectsComponent.Effects.First(kvp => kvp.Key.Equals(actionPrecondition.Key));
            action.RepeatCount = (int)Math.Ceiling(actionPrecondition.Value / (float)effect.Value);
        });

        return matchingActions;
    }

    public void Execute(double deltaTime)
    {
        try
        {
            if (_goapPlannerExecutionQueue.IsQueueEmpty)
                return;

            _goapPlannerExecutionQueue.ExecuteQueue(deltaTime);
        }
        catch (Exception ex)
        {
            GD.PrintErr(nameof(GoapPlanner) + " encountered an error executing action: " + ex);
        }
    }
}