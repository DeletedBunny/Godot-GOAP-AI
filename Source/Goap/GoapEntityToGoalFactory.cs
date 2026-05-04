using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap;

public class GoapEntityToGoalFactory
{
    private static Dictionary<EntityType, GoapActionPreconditionComponent> _entityToGoal = new()
    {
        { EntityType.HomeA, new GoapActionPreconditionComponent()
        {
            Preconditions = new List<KeyValuePair<string, int>>()
            {
                new ("HomeAInWorld", 1)
            }
        } }
    };

    public static void GetGoal(EntityType entityType, out GoapActionPreconditionComponent goal)
    {
        goal = new GoapActionPreconditionComponent
        {
            Preconditions = _entityToGoal[entityType].Preconditions.ToList()
        };
    }
}