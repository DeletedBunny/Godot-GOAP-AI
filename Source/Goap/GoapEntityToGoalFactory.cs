using System.Collections.Generic;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap;

public class GoapEntityToGoalFactory
{
    public static Dictionary<EntityType, GoapActionPreconditionComponent> EntityToGoal = new()
    {
        { EntityType.HomeA, new GoapActionPreconditionComponent()
        {
            Preconditions = new List<KeyValuePair<string, int>>()
            {
                new ("HomeAInWorld", 1)
            }
        } }
    };
}