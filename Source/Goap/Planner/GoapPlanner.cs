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

        /*GD.Print("Planning tree created");
        var leafs = new Queue<GoapPlanningLeaf>();
        leafs.Enqueue(planningTree.Root);
        while (leafs.Count > 0)
        {
            var currentLeaf = leafs.Dequeue();
            GD.Print(currentLeaf.ActionInstance.Type + " | IsResolvable: " + currentLeaf.IsResolvable);
            var children = currentLeaf.Children.Values.SelectMany(x => x);
            foreach (var child in children) leafs.Enqueue(child);
        }*/

        // Hardcoded for now
        //var pickupAxe = _goapActionsFactory.GetAction(GoapActionType.PickUpAxe, agent);
        //var cutTreeAction = _goapActionsFactory.GetAction(GoapActionType.CutTree, agent);
        //pickupAxe.InitializeTarget(worldStateMemento, null);
        //cutTreeAction.InitializeTarget(worldStateMemento, pickupAxe);

        //_goapPlannerExecutionQueue.AddToQueue(cutTreeAction);
        //_goapPlannerExecutionQueue.AddToQueue(pickupAxe);
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

            if (currentLeaf.ActionInstance.ActionPreconditionsComponent.NeedsEntityNearby())
            {
                var moveToAction = _goapActionsFactory.GetMoveToAction(currentLeaf.ActionInstance.ActionPreconditionsComponent.RequiredEntity, agent);
                var moveToLeaf = CreateLeaf(moveToAction);
                moveToLeaf.IsResolvable = true;
                currentLeaf.AddChild("Near" + currentLeaf.ActionInstance.ActionPreconditionsComponent.RequiredEntity, moveToLeaf);
            }

            var unmetPreconditions = currentLeaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                                .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value &&
                                                              !kvp.Key.Contains("Near"))
                                                .ToList();

            if (unmetPreconditions.Count == 0)
            {
                currentLeaf.IsResolvable = true;
                continue;
            }

            foreach (var precondition in unmetPreconditions)
            {
                var matchingActions = GetMatchingActionsWithAmount(precondition);

                if (matchingActions.Count == 0) 
                    currentLeaf.IsResolvable = false;

                foreach (var action in matchingActions)
                {
                    var actionInstance = _goapActionsFactory.GetAction(action.Type, agent);
                    var newLeaf = CreateLeaf(actionInstance);
                    currentLeaf.AddChild(precondition.Key, newLeaf);
                    unvisitedLeafs.Enqueue(newLeaf);
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
        var unmetPreconditions = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions
                                     .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value)
                                     .Select(kvp => kvp.Key)
                                     .ToList();

        var preconditionsTotalCost = 0f;
        IGoapAction cheapestChild = previousAction;

        foreach (var precondition in unmetPreconditions)
        {
            if (!leaf.Children.TryGetValue(precondition, out var children))
                return new GoapValidationResult() { Cost = float.MaxValue };

            var cheapestChildResult = new GoapValidationResult() { Cost = float.MaxValue };

            foreach (var child in children)
            {
                var result = ValidateTreeAndCalculateCost(child, worldStatePlanningModel, worldStateMemento, cheapestChild);
                
                if (result.Cost < cheapestChildResult.Cost)
                    cheapestChildResult = result;
            }

            if (cheapestChildResult.Cost >= float.MaxValue)
                return new GoapValidationResult() { Cost = float.MaxValue };


            preconditionsTotalCost += cheapestChildResult.Cost;
            cheapestChild = cheapestChildResult.Action;
        }

        var moveToType = leaf.ActionInstance.Type == GoapActionType.MoveTo ? leaf.Parent.ActionInstance.ActionPreconditionsComponent.RequiredEntity : EntityType.None;
        leaf.ActionInstance.InitializeTarget(worldStateMemento, cheapestChild, moveToType);
        leaf.CachedTotalCost = leaf.CalculatedCost + preconditionsTotalCost;
        leaf.IsResolvable = true;
        return new GoapValidationResult() { Cost = leaf.CachedTotalCost, Action = leaf.ActionInstance };
    }

    private void BuildExecutionQueue(GoapPlanningLeaf leaf)
    {
        var itemActions = new List<GoapPlanningLeaf>();
        GoapPlanningLeaf moveAction = null;
        
        foreach (var child in leaf.Children)
        {
            var bestChild = child.Value
                                 .Where(x => x.IsResolvable)
                                 .OrderBy(x => x.CachedTotalCost)
                                 .FirstOrDefault();

            if (bestChild == null)
                continue;

            if (child.Key.Contains("Near"))
            {
                moveAction = bestChild;
            }
            else
            {
                itemActions.Add(bestChild);
            }
        }

        foreach (var action in itemActions)
        {
            BuildExecutionQueue(action);
        }

        if (moveAction != null)
        {
            BuildExecutionQueue(moveAction);
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