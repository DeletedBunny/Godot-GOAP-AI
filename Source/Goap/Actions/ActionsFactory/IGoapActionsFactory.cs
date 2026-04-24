using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions;

public interface IGoapActionsFactory
{
    void RegisterActions();
    IGoapAction GetAction(GoapActionType type, Agent3D agent);
    List<GoapPlanningAction> GetMatchingActionsByEffect(string actionResultKey);
    GoapPlanningAction GetMoveToAction(EntityType typeToMoveTo);
}