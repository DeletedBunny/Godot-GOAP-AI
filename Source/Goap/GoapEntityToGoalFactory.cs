using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap;

public static class GoapEntityToGoalFactory
{
    private static readonly Dictionary<EntityType, GoapActionPreconditionComponent> _entityToGoal = new()
    {
        { EntityType.HomeA, new GoapActionPreconditionComponent()
        {
            Preconditions = [new("HomeAInWorld", 1)]
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