using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
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
        var agentInternalState = agent.GetAgentWorldState();
        var simulationStateModel = GoapWorldStateService.Instance.GetWorldStateForSimulation(agentInternalState);

        var planningTree = BuildTree(_goapActionsFactory.GetGoal(neededItemsToHave), simulationStateModel, agent);
        BuildExecutionQueue(planningTree.Root);
    }

    private GoapPlanningTree BuildTree(IGoapAction goal, GoapWorldStateModel simulationStateModel, Agent3D agent)
    {
        var root = new GoapPlanningLeaf(goal);
        var planningTree = new GoapPlanningTree(root);

        var unvisitedLeafs = new Stack<GoapPlanningLeaf>();
        unvisitedLeafs.Push(planningTree.Root);
        
        RecursiveLeafDfs(unvisitedLeafs, simulationStateModel, agent, null);

        return planningTree;
    }

    private IGoapAction RecursiveLeafDfs(Stack<GoapPlanningLeaf> unvisitedLeafs, GoapWorldStateModel simulationStateModel, Agent3D agent, IGoapAction previousAction)
    {
        var currentLeaf = unvisitedLeafs.Pop();
        IGoapAction rollingContextAction = previousAction;
        
        var unmetPreconditions = currentLeaf.ActionInstance.PreconditionsComponent.Preconditions
                                            .Where(kvp => FilterPreconditions(kvp, simulationStateModel))
                                            .OrderBy(x => x.Key.StartsWith(GoapWorldStateConstants.HasModifierPrefix) ? 0 : 1)
                                            .ToList();

        foreach (var precondition in unmetPreconditions)
        {
            var matchingActions = GetMatchingActionsWithAmount(precondition, simulationStateModel, agent);

            var matchingActionCounter = -1;
            foreach (var action in matchingActions)
            {
                matchingActionCounter++;
                var rollingStateForBranchingActions = matchingActions.Count > 1 ? simulationStateModel.Clone() : simulationStateModel;
                for (int i = 0; i < action.RepeatCount; i++)
                {
                    var actionInstance = action.ActionInstance;
                    if (action.RepeatCount > 1 && i > 0)
                        actionInstance = _goapActionsFactory.GetAction(action.ActionInstance.Type, agent);
                    var newLeaf = CreateLeaf(actionInstance);
                    newLeaf.PathId = currentLeaf.PathId > matchingActionCounter ? currentLeaf.PathId : matchingActionCounter;
                    currentLeaf.AddChild(precondition.Key, newLeaf);
                    unvisitedLeafs.Push(newLeaf);
                    rollingContextAction = RecursiveLeafDfs(unvisitedLeafs, rollingStateForBranchingActions, agent, rollingContextAction);
                }
            }
        }
        
        if (!currentLeaf.ActionInstance.IsInitialized)
        {
            currentLeaf.IsResolvable = currentLeaf.CheckIsResolvable(simulationStateModel);
            var requiredEntity = currentLeaf.Parent?.ActionInstance.PreconditionsComponent.RequiredEntity ?? EntityType.None;
            var entityType = currentLeaf.ActionInstance.Type == GoapActionType.MoveTo ? requiredEntity : EntityType.None;
            currentLeaf.ActionInstance.InitializeTarget(simulationStateModel, rollingContextAction, entityType);
            rollingContextAction = currentLeaf.ActionInstance;
        }
                
        foreach (var effect in currentLeaf.ActionInstance.EffectsComponent.Effects)
        {
            simulationStateModel.UpdateState(effect.Key, effect.Value);
        }
        
        currentLeaf.WorldState = simulationStateModel;
        currentLeaf.CachedTotalCost = currentLeaf.CalculateTotalCost();
        return rollingContextAction;
    }

    private bool FilterPreconditions(KeyValuePair<string, int> precondition, GoapWorldStateModel simulationStateModel)
    {
        var stateValue = simulationStateModel.GetState(precondition.Key);
        return stateValue < precondition.Value;
    }

    private void BuildExecutionQueue(GoapPlanningLeaf leaf)
    {
        var allResolvableChildren = leaf.Children.Values
                                        .SelectMany(x => x)
                                        .Where(x => x.IsResolvable)
                                        .ToList();

        if (allResolvableChildren.Count > 0)
        {
            var bestPathId = allResolvableChildren
                             .GroupBy(x => x.PathId)
                             .OrderBy(x => x.Sum(y => y.CachedTotalCost))
                             .First()
                             .Key;

            var sortedKeys = leaf.Children.Keys
                                 .OrderBy(x => x.Contains(GoapWorldStateConstants.NearEntityKey) ? 1 : 0)
                                 .ToList();

            foreach (var key in sortedKeys)
            {
                var children = leaf.Children[key].Where(x => x.IsResolvable && x.PathId == bestPathId);

                foreach (var child in children)
                {
                    BuildExecutionQueue(child);
                }
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

    private List<GoapPlanningAction> GetMatchingActionsWithAmount(KeyValuePair<string, int> actionPrecondition, GoapWorldStateModel worldStateModel, Agent3D agent)
    {
        var emptyHandsString = GoapWorldStateConstants.HasModifierPrefix + GoapWorldStateConstants.AgentEmptyHandsKey;
        var entityType = EntityType.None;
        if (actionPrecondition.Key.Equals(emptyHandsString) && actionPrecondition.Value > 0)
            entityType = worldStateModel.GetEntityStringFromPartialKey(GoapWorldStateConstants.HasModifierPrefix, [emptyHandsString]);
        var matchingActions = _goapActionsFactory.GetMatchingActionsByEffect(actionPrecondition.Key, entityType, agent);

        var goapPlanningActions = matchingActions.Select<IGoapAction, GoapPlanningAction>(action =>
        {
            if (action.Type == GoapActionType.MoveTo)
                return new () { ActionInstance = action, RepeatCount = 1 };
            
            var effect = action.EffectsComponent.Effects.First(kvp => kvp.Key.Equals(actionPrecondition.Key));
            var repeatCount = (int)Math.Ceiling(actionPrecondition.Value / (float)effect.Value);
            return new() { ActionInstance = action, RepeatCount = repeatCount };
        }).ToList();

        return goapPlanningActions;
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
            _goapPlannerExecutionQueue.ClearQueue();
            GD.PrintErr(nameof(GoapPlanner) + " encountered an error executing action: " + ex);
        }
    }
}