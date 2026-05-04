using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Actions.ActionsFactory;

public interface IGoapActionsFactory
{
    void RegisterActions();
    IGoapAction GetGoal(GoapActionPreconditionComponent preconditionComponent);

    List<GoapPlanningAction> GetMatchingActionsWithAmount(
        KeyValuePair<string, int> actionPrecondition,
        GoapWorldStateModel worldStateModel,
        IAgentPlanner agent);
}