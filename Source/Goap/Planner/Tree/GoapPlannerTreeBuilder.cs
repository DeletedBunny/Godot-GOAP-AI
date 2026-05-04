using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.GOAP.Planner.Helpers;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Planner.Tree;

public class GoapPlannerTreeBuilder
{
    private readonly IGoapActionsFactory _goapActionsFactory;

    public GoapPlannerTreeBuilder(IGoapActionsFactory goapActionsFactory)
    {
        _goapActionsFactory = goapActionsFactory;
    }

    public GoapPlanningTree BuildTree(IGoapAction goal, GoapWorldStateModel simulationStateModel, IAgentPlanner agent)
    {
        var root = new GoapPlanningLeaf(goal);
        var planningTree = new GoapPlanningTree(root);

        var unvisitedLeafs = new Stack<GoapPlanningLeaf>();
        unvisitedLeafs.Push(planningTree.Root);
        
        RecursiveLeafDfs(unvisitedLeafs, simulationStateModel, agent, null);

        return planningTree;
    }
    
    private IGoapAction RecursiveLeafDfs(
        Stack<GoapPlanningLeaf> unvisitedLeafs, 
        GoapWorldStateModel simulationStateModel, 
        IAgentPlanner agent, 
        IGoapAction previousAction)
    {
        var currentLeaf = unvisitedLeafs.Pop();
        var rollingContextAction = previousAction;
        
        var unmetPreconditions = currentLeaf.ActionInstance.PreconditionsComponent.Preconditions
                                            .OrderBy(GoapPlannerPreconditionProcessor.PreconditionOrdering)
                                            .ToList();

        foreach (var precondition in unmetPreconditions)
        {
            if (!GoapPlannerPreconditionProcessor.FilterPreconditions(precondition, simulationStateModel))
                continue;
            
            var matchingActions = _goapActionsFactory.GetMatchingActionsWithAmount(precondition, simulationStateModel, agent);

            var matchingActionCounter = -1;
            foreach (var action in matchingActions)
            {
                matchingActionCounter++;
                var rollingStateForBranchingActions = matchingActions.Count > 1 ? simulationStateModel.Clone() : simulationStateModel;
                for (int i = 0; i < action.RepeatCount; i++)
                {
                    var actionInstance = action.ActionInstance;
                    if (action.RepeatCount > 1 && i > 0)
                        actionInstance = _goapActionsFactory.GetMatchingActionsWithAmount(precondition, simulationStateModel, agent).First().ActionInstance;
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
            currentLeaf.ActionInstance.InitializeTargetProvider(simulationStateModel, rollingContextAction, entityType);
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
    
    
    private static GoapPlanningLeaf CreateLeaf(IGoapAction action)
    {
        return new GoapPlanningLeaf(action);
    }
}