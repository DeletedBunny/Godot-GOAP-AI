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

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanner
{
    private readonly IGoapActionsFactory _goapActionsFactory = new GoapActionsFactory();
    private readonly GoapPlannerExecutionQueue _goapPlannerExecutionQueue = new GoapPlannerExecutionQueue();
    
    public GoapPlanner()
    {
        _goapActionsFactory.RegisterActions();
    }

    public void Plan(Agent3D agent, GoapActionEffectComponent neededEffectComponent)
    {
        var worldStateMemento = GoapWorldStateService.Instance.GetWorldStateMemento();

        GoapPlanningLeaf root = new GoapPlanningLeaf(GoapActionType.PlanningTreeRoot, neededEffectComponent);
        GoapPlanningTree planningTree = new GoapPlanningTree(root);

        List<IGoapAction> possibleSolutions;
        
        foreach (var actionResult in  planningTree.Root.ActionEffectComponent.GetActionResults())
        {
            var matchingActions = GetMatchingActionsWithAmount(actionResult);
                
            if (!matchingActions.Any()) 
                continue;
                
            foreach (var action in matchingActions)
            {
                for (int i = 0; i < action.Item3; i++)
                {
                    planningTree.Root.AddChild(new GoapPlanningLeaf(action.Item1, action.Item2));
                }
            }
        }
        
        
        
        
        // Hardcoded for now
        var pickupAxe = _goapActionsFactory.GetAction(GoapActionType.PickUpAxe, new GoapActionParams(agent));
        var cutTreeAction = _goapActionsFactory.GetAction(GoapActionType.CutTree, new GoapActionParams(agent));
        pickupAxe.IsActionPreconditionsValid(worldStateMemento, null);
        cutTreeAction.IsActionPreconditionsValid(worldStateMemento, pickupAxe);
        
        _goapPlannerExecutionQueue.AddToQueue(cutTreeAction);
        _goapPlannerExecutionQueue.AddToQueue(pickupAxe);
    }

    private List<(GoapActionType, GoapActionEffectComponent, int)> GetMatchingActionsWithAmount(KeyValuePair<string, int> actionResult)
    {
        var matchingActions = _goapActionsFactory.GetMatchingActions(actionResult.Key);
        var result = new List<(GoapActionType, GoapActionEffectComponent, int)>();
        if (matchingActions.Count == 0)
            return result;
        
        foreach (var action in matchingActions)
        {
            var matchingActionResult = action.Item2.GetActionResults().First(item => item.Key.Equals(actionResult.Key));
            var actionTimesNeededToRepeat = (int)Math.Ceiling(actionResult.Value / (float)matchingActionResult.Value);
            result.Add((action.Item1, action.Item2, actionTimesNeededToRepeat));
        }
        
        return result;
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