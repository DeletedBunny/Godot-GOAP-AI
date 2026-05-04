using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public interface IGoapActionsFactoryBuilding
{
    void AddAction(GoapActionType actionType, GoapActionRegistration actionRegistration);

    IGoapAction CreateAction<TAction>(
        IAgentPlanner agent,
        GoapActionDataComponent actionData,
        GoapActionPreconditionComponent actionPreconditions,
        GoapActionEffectComponent actionEffects)
        where TAction : IGoapAction, new();
}