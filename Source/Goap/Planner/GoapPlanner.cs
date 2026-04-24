using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
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

        GoapPlanningLeaf root = new GoapPlanningLeaf(GoapActionType.PlanningTreeRoot, null, neededItemsToHave);
        GoapPlanningTree planningTree = new GoapPlanningTree(root);
        
        // Start planning
        var unvisitedLeafs = new Queue<GoapPlanningLeaf>();
        unvisitedLeafs.Enqueue(planningTree.Root);
        while (unvisitedLeafs.Count > 0)
        {
            var currentLeaf = unvisitedLeafs.Dequeue();
            
            foreach (var precondition in currentLeaf.ActionPreconditionComponent.Preconditions)
            {
                if (worldStatePlanningModel.GetTrackedState(precondition.Key) >= precondition.Value)
                {
                    continue;
                }

                if (currentLeaf.ActionPreconditionComponent.NeedsEntityNearby())
                {
                    var moveToAction = GetMoveToAction(currentLeaf.ActionPreconditionComponent.RequiredEntity);
                    currentLeaf.MoveToActionLeaf = CreateLeaf(moveToAction);
                }
                
                var matchingActions = GetMatchingActionsWithAmount(precondition);
                
                foreach (var action in matchingActions)
                {
                    var newLeaf = CreateLeaf(action);
                    currentLeaf.AddChild(newLeaf);
                    unvisitedLeafs.Enqueue(newLeaf);
                }
            }
        }
        
        // Hardcoded for now
        //var pickupAxe = _goapActionsFactory.GetAction(GoapActionType.PickUpAxe, new GoapActionParams(agent));
        //var cutTreeAction = _goapActionsFactory.GetAction(GoapActionType.CutTree, new GoapActionParams(agent));
        //pickupAxe.IsActionPreconditionsValid(worldStateMemento, null);
        //cutTreeAction.IsActionPreconditionsValid(worldStateMemento, pickupAxe);
        
        //_goapPlannerExecutionQueue.AddToQueue(cutTreeAction);
        //_goapPlannerExecutionQueue.AddToQueue(pickupAxe);
    }
    
    private GoapPlanningLeaf CreateLeaf(GoapPlanningAction action) => new(action.Type, action.EffectsComponent, action.PreconditionsComponent);

    private List<GoapPlanningAction> GetMatchingActionsWithAmount(KeyValuePair<string, int> actionPrecondition)
    {
        var matchingActions = _goapActionsFactory.GetMatchingActionsByEffect(actionPrecondition.Key);
        
        matchingActions.ForEach(action =>
        {
            var effect = action.EffectsComponent.Effects.First(item => item.Key.Equals(actionPrecondition.Key));
            action.RepeatCount = (int)Math.Ceiling(actionPrecondition.Value / (float)effect.Value);
        });
        
        return matchingActions;
    }

    private GoapPlanningAction GetMoveToAction(EntityType typeToMoveTo) => _goapActionsFactory.GetMoveToAction(typeToMoveTo);
    
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