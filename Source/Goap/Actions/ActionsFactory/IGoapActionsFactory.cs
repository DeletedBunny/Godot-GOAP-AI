using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Actions.ActionsFactory;

public interface IGoapActionsFactory
{
    void RegisterActions();
    IGoapAction GetAction(GoapActionType type, Agent3D agent);
    IGoapAction GetGoal(GoapActionPreconditionComponent preconditionComponent);
    List<IGoapAction> GetMatchingActionsByEffect(string actionResultKey, EntityType requiredEntity, Agent3D agent);
}