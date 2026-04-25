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
    private readonly GoapPlannerExecutionQueue _goapPlannerExecutionQueue = new GoapPlannerExecutionQueue();
    
    public GoapPlanner()
    {
        _goapActionsFactory.RegisterActions();
    }

    public void Plan(Agent3D agent, GoapActionPreconditionComponent neededItemsToHave)
    {
        var worldStateMemento = GoapWorldStateService.Instance.GetWorldStateMemento();
        var worldStatePlanningModel = new GoapWorldStatePlanningModel(worldStateMemento);
        agent.GetAgentWorldState().ForEach(x => worldStatePlanningModel.SetTrackedState("Has" + x.Item1, x.Item2));
        
        

        if (ValidateTreeResolvability(planningTree.Root, worldStatePlanningModel))
        {
            GD.Print("Tree is resolvable");
            
        }
        
        GD.Print("Planning tree created");
        var leafs = new Queue<GoapPlanningLeaf>();
        leafs.Enqueue(planningTree.Root);
        while (leafs.Count > 0)
        {
            var currentLeaf = leafs.Dequeue();
            GD.Print(currentLeaf.ActionInstance.Type + " | IsResolvable: " + currentLeaf.IsResolvable);
            var children = currentLeaf.Children.Values.SelectMany(x => x);
            foreach (var child in children)
            {
                leafs.Enqueue(child);
            }
        }

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
        GoapPlanningLeaf root = new GoapPlanningLeaf(goal);
        GoapPlanningTree planningTree = new GoapPlanningTree(root);
        
        // Start planning
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
                .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value && !kvp.Key.Contains("Near"))
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
                {
                    currentLeaf.IsResolvable = false;
                }
                
                foreach (var action in matchingActions)
                {
                    var actionInstance = _goapActionsFactory.GetAction(action.Type, agent);
                    var newLeaf = CreateLeaf(actionInstance);
                    currentLeaf.AddChild(precondition.Key, newLeaf);
                    unvisitedLeafs.Enqueue(newLeaf);
                }
            }
        }
        
        r
    }

    private bool ValidateTreeResolvability(GoapPlanningLeaf leaf, GoapWorldStatePlanningModel worldStatePlanningModel)
    {
        var unmetPreconditions = leaf.ActionInstance.ActionPreconditionsComponent.Preconditions
            .Where(kvp => worldStatePlanningModel.GetTrackedState(kvp.Key) < kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();
        
        if (unmetPreconditions.Count == 0)
            return leaf.IsResolvable = true;

        foreach (var precondition in unmetPreconditions)
        {
            if (!leaf.Children.TryGetValue(precondition, out var children))
                return leaf.IsResolvable = false;

            bool anyChildValid = false;
            foreach (var child in children)
            {
                if (ValidateTreeResolvability(child, worldStatePlanningModel))
                {
                    anyChildValid = true;
                    break;
                }
            }
            
            if (!anyChildValid)
                return leaf.IsResolvable = false;
        }
        
        return leaf.IsResolvable = true;
    }

    private float InitializeAndCalculateCost(
        GoapPlanningLeaf leaf, 
        IGoapAction parentAction,
        GoapWorldStateMemento worldStateMemento)
    {
        leaf.ActionInstance.InitializeTarget(worldStateMemento, parentAction);
        float currentActionCost = leaf.ActionInstance.CalculateCost();
        
        
    }
    
    private GoapPlanningLeaf CreateLeaf(IGoapAction action) => new(action);

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