using System.Linq;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.GOAP.Planner.Helpers;

public static class GoapPlannerExecutionBuilder
{
    public static void BuildExecutionQueue(GoapPlanningLeaf leaf, IAgentPlanner agent)
    {
        var allResolvableChildren = leaf.Children.Values
            .SelectMany(x => x)
            .Where(x => x.IsResolvable)
            .ToList();

        if (allResolvableChildren.Count > 0)
        {
            var bestPathId = allResolvableChildren
                .GroupBy(x => x.PathId)
                .OrderBy(x => x.Sum(y => y.CachedTotalCost))
                .First()
                .Key;

            var sortedKeys = leaf.Children.Keys
                .OrderBy(x => x.Contains(GoapWorldStateConstants.NearEntityKey) ? 1 : 0)
                .ToList();

            foreach (var key in sortedKeys)
            {
                var children = leaf.Children[key].Where(x => x.IsResolvable && x.PathId == bestPathId);

                foreach (var child in children)
                {
                    BuildExecutionQueue(child, agent);
                }
            }
        }

        if (leaf.ActionInstance.Type == GoapActionType.PlanningTreeRoot)
            return;
        
        if (leaf.ActionInstance.GetTarget() is IEntity entity)
            GoapWorldStateService.Instance.ReserveEntity(entity.EntityType, leaf.ActionInstance.GetTarget());
        
        agent.AddActionToExecutionQueue(leaf.ActionInstance);
    }
}