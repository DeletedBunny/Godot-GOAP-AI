using System;
using Godot;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
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

    public void Plan(Agent3D agent)
    {
        // Hardcoded for now
        var worldStateMemento = GoapWorldStateService.Instance.GetWorldStateMemento();
        var pickupAxe = _goapActionsFactory.GetAction(GoapActionType.PickUpItem, new GoapActionParams(agent));
        var cutTreeAction = _goapActionsFactory.GetAction(GoapActionType.CutTree, new GoapActionParams(agent));
        pickupAxe.IsActionPreconditionsValid(worldStateMemento, new GoapActionResult());
        cutTreeAction.IsActionPreconditionsValid(worldStateMemento, pickupAxe.GetActionResult());
        
        _goapPlannerExecutionQueue.AddToQueue(cutTreeAction);
        _goapPlannerExecutionQueue.AddToQueue(pickupAxe);
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