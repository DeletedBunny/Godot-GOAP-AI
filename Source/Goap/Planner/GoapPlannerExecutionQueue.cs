using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlannerExecutionQueue
{
    private readonly List<IGoapAction> _executionQueue = new List<IGoapAction>();
    private readonly object _executionQueueLock = new object();
    
    private IGoapAction _runningAction;
    
    public bool IsQueueEmpty => _runningAction == null && _executionQueue.Count == 0;
    
    public void AddToQueue(IGoapAction action)
    {
        lock (_executionQueueLock)
        {
            _executionQueue.Add(action);
        }
    }

    private void Dequeue()
    {
        lock (_executionQueueLock)
        {
            _runningAction = _executionQueue.First();
            _executionQueue.RemoveAt(0);
        }
    }

    private bool HasActions()
    {
        lock (_executionQueueLock)
        {
            return _executionQueue.Any();
        }
    }

    public void ClearQueue()
    {
        lock (_executionQueueLock)
        {
            _executionQueue.Clear();
        }
    }

    private void ExecuteRunningActionFrame(double deltaTime)
    {
        try
        {
            _runningAction.ExecuteAction(deltaTime);
        }
        catch (Exception ex)
        {
            GD.PrintErr(nameof(GoapPlannerExecutionQueue) + " encountered an error executing running action: " + ex);
        }
    }

    public void ExecuteQueue(double deltaTime)
    {
        try
        {
            var isQueueReadyToExecute = _runningAction == null && HasActions();
            var isQueueCompleted = _runningAction?.IsCompletedConditionMet() ?? true;
            if (isQueueReadyToExecute || isQueueCompleted)
            {
                if (HasActions())
                {
                    Dequeue();
                }
                else
                {
                    _runningAction = null;
                    return;
                }
            }
            
            ExecuteRunningActionFrame(deltaTime);
        }
        catch (Exception ex)
        {
            ex.Data.Add("Action", nameof(_runningAction.GetType));
            ClearQueue();
            _runningAction = null;
            throw;
        }
    }
}