using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;

namespace GodotGOAPAI.Source.Goap.Actions;

public interface IGoapActionsFactory
{
    void RegisterActions();
    IGoapAction GetAction(GoapActionType type, Agent3D agent);
    List<(GoapActionType, GoapActionEffectComponent)> GetMatchingActions(string actionResultKey);
}