using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.Actions.ActionComponents;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapActionBuilder
{
    private readonly GoapActionPreconditionComponent _actionPreconditions = new();
    private readonly GoapActionEffectComponent _actionEffects = new();
    private GoapActionDataComponent _actionData;
    private GoapActionType _actionType;
        
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

    public void Build<TAction>(IGoapActionsFactoryBuilding factory) where TAction : IGoapAction, new()
    {
        factory.AddAction(_actionType, new GoapActionRegistration(
            factory.CreateAction<TAction>,
            _actionData,
            _actionPreconditions,
            _actionEffects
        ));
    }
}