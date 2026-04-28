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
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

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
        var agentInternalState = agent.GetAgentWorldState().Select(x => ("Has" + x.Item1, x.Item2));
        var simulationStateModel = GoapWorldStateService.Instance.GetWorldStateForSimulation(agentInternalState);

        var planningTree = BuildTree(_goapActionsFactory.GetGoal(neededItemsToHave), simulationStateModel, agent);

        var finalPlanResult = ValidateTreeAndCalculateCost(planningTree.Root, simulationStateModel, null);
        if (finalPlanResult.Cost < float.MaxValue)
        {
            BuildExecutionQueue(planningTree.Root);
        }
    }

    private GoapPlanningTree BuildTree(IGoapAction goal, GoapWorldStateModel simulationStateModel, Agent3D agent)
    {
        var root = new GoapPlanningLeaf(goal);
        var planningTree = new GoapPlanningTree(root);

        var unvisitedLeafs = new Queue<GoapPlanningLeaf>();
        unvisitedLeafs.Enqueue(planningTree.Root);

        while (unvisitedLeafs.Count > 0)
        {
            var currentLeaf = unvisitedLeafs.Dequeue();

            var unmetPreconditions = currentLeaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                                .Where(kvp => simulationStateModel.GetState(kvp.Key) < kvp.Value &&
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
        GoapWorldStateModel simulationStateModel,
        IGoapAction previousAction)
    {
        GD.Print("Validating leaf: " + leaf.ActionInstance.Type);
        var unmetPreconditions = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                     .Where(kvp => simulationStateModel.GetState(kvp.Key) < kvp.Value)
                                     .Select(kvp => kvp.Key)
                                     .OrderBy(x => x.Contains("Near") ? 1 : 0)
                                     .ToList();

        var preconditionsTotalCost = 0f;
        IGoapAction rollingContextAction = previousAction;

        foreach (var precondition in unmetPreconditions)
        {
            if (!leaf.Children.TryGetValue(precondition, out var children))
                return new GoapValidationResult() { Cost = float.MaxValue };

            var amountNeeded = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions.First(kvp => kvp.Key.Equals(precondition)).Value;

            var groupedActions = children.GroupBy(x => x.ActionInstance.Type);
            var bestGroupResult = new GoapValidationResult() { Cost = float.MaxValue };
            GoapWorldStateModel bestGroupWorldStateModel = null;

            foreach (var group in groupedActions)
            {
                var sandboxWorldStateModel = simulationStateModel.Clone();
                var groupCost = 0f;
                var groupGatheredAmount = 0;
                IGoapAction groupContextAction = rollingContextAction;
                List<GoapPlanningLeaf> groupSelectedLeafs = new();
                
                foreach (var child in group)
                {
                    if (groupGatheredAmount >= amountNeeded)
                        break;

                    var result = ValidateTreeAndCalculateCost(child, sandboxWorldStateModel, groupContextAction);

                    if (result.Cost >= float.MaxValue)
                        continue;
                    
                    groupCost += result.Cost;
                    groupContextAction = result.Action;
                    groupGatheredAmount += child.ActionInstance.ActionEffectsComponent.Effects.First(kvp => kvp.Key.Equals(precondition)).Value;
                    groupSelectedLeafs.Add(child);
                }

                if (groupGatheredAmount < amountNeeded || groupCost >= bestGroupResult.Cost) 
                    continue;
                
                bestGroupResult = new GoapValidationResult() { Cost = groupCost, Action = groupContextAction };
                bestGroupWorldStateModel = sandboxWorldStateModel;
                children.ForEach(x => x.IsResolvable = false);
                groupSelectedLeafs.ForEach(x => x.IsResolvable = true);
            }

            if (bestGroupWorldStateModel == null || bestGroupResult.Cost >= float.MaxValue)
                return new GoapValidationResult() { Cost = float.MaxValue };
            
            preconditionsTotalCost += bestGroupResult.Cost;
            rollingContextAction = bestGroupResult.Action;
            simulationStateModel.SyncState(bestGroupWorldStateModel);
        }

        var parentActionRequiredEntity = leaf.Parent?.ActionInstance.ActionPreconditionsComponent.RequiredEntity;
        var moveToType = leaf.ActionInstance.Type == GoapActionType.MoveTo 
                             ? parentActionRequiredEntity
                             : EntityType.None;
        
        leaf.ActionInstance.InitializeTarget(simulationStateModel, rollingContextAction, moveToType ?? EntityType.None);

        foreach (var effect in leaf.ActionInstance.ActionEffectsComponent.Effects)
        {
            if (effect.Key.Contains("Near"))
                continue;
            
            simulationStateModel.UpdateState(effect.Key, effect.Value);
        }
        
        leaf.CachedTotalCost = leaf.CalculatedCost + preconditionsTotalCost;
        leaf.IsResolvable = true;
        
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
        
        if (leaf.ActionInstance.GetTarget() is IEntity entity)
            GoapWorldStateService.Instance.ReserveEntity(entity.EntityType, leaf.ActionInstance.GetTarget());
        
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