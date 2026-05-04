using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionsFactory;
using GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.GOAP.Planner.Helpers;
using GodotGOAPAI.Source.GOAP.Planner.Tree;
using GodotGOAPAI.Source.Goap.WorldState;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanner
{
    private readonly IGoapActionsFactory _goapActionsFactory = new GoapActionsFactory();
    private readonly GoapPlannerTreeBuilder _goapPlannerTreeBuilder;

    public GoapPlanner()
    {
        _goapPlannerTreeBuilder = new GoapPlannerTreeBuilder(_goapActionsFactory);
        _goapActionsFactory.RegisterActions();
    }

    public void Plan(IAgentPlanner agent, GoapActionPreconditionComponent neededItemsToHave)
    {
        var agentInternalState = agent.GetAgentWorldState();
        var simulationStateModel = GoapWorldStateService.Instance.GetWorldStateForSimulation(agentInternalState);

        List<KeyValuePair<string, int>> modifiedPreconditions = new();
        foreach (var item in neededItemsToHave.Preconditions)
        {
            var modifiedValue = simulationStateModel.GetState(item.Key) + item.Value;
            modifiedPreconditions.Add(new KeyValuePair<string, int>(item.Key, modifiedValue));
        }
        neededItemsToHave.Preconditions = modifiedPreconditions;

        var planningTree = _goapPlannerTreeBuilder.BuildTree(_goapActionsFactory.GetGoal(neededItemsToHave), simulationStateModel, agent);
        GD.Print(planningTree);
        GoapPlannerExecutionBuilder.BuildExecutionQueue(planningTree.Root, agent);
        agent.ExecuteActionQueue();
    }
}