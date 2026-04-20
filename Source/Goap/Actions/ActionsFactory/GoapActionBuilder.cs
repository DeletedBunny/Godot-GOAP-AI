using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Actions.ActionData;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionBuilder
{
    private GoapActionType _actionType;
    private GoapActionDataComponent _actionData = new();
    private GoapActionPreconditionComponent _actionPreconditions = new();
    private GoapActionEffectComponent _actionEffects = new();
        
    public static GoapActionBuilder Create(GoapActionType actionType) => new() { _actionType = actionType };

    public GoapActionBuilder AddData(GoapActionDataComponent actionData)
    {
        _actionData = actionData;
        return this;
    }

    public GoapActionBuilder AddRequiredLocationType(EntityType entityType)
    {
        _actionPreconditions.RequiredEntity = entityType;
        return this;
    }
    
    public GoapActionBuilder AddPrecondition(string actionPreConditionKey, int actionCondition)
    {
        _actionPreconditions.Preconditions.Add(new KeyValuePair<string, int>(actionPreConditionKey, actionCondition));
        return this;
    }

    public GoapActionBuilder AddEffect(string actionEffectKey, int actionEffectValue)
    {
        _actionEffects.Effects.Add(new KeyValuePair<string, int>(actionEffectKey, actionEffectValue));
        return this;
    }

    public void Build<TAction>(GoapActionsFactory factory) where TAction : GoapActionBase, new()
    {
        factory.AddAction(_actionType, new GoapActionRegistration(
            factory.CreateAction<TAction>,
            _actionData,
            _actionPreconditions,
            _actionEffects
        ));
    }
}