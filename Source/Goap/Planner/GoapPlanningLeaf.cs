using System.Collections.Generic;
using System.Linq;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningLeaf
{
    public int PathId { get; set; }
    public IGoapAction ActionInstance { get; }
    
    public GoapPlanningLeaf Parent { get; set; }
    public Dictionary<string, List<GoapPlanningLeaf>> Children { get; } = new();
    public GoapWorldStateModel WorldState { get; set; }
    
    public float CachedTotalCost { get; set; }
    public bool IsResolvable { get; set; }

    public GoapPlanningLeaf(IGoapAction actionInstance)
    {
        ActionInstance = actionInstance;
    }

    public bool CheckIsResolvable(GoapWorldStateModel worldStateModel)
    {
        var preconditions = ActionInstance.PreconditionsComponent.Preconditions;
        var isResolvableChildren = preconditions.All(kvp =>
        {
            var isResolvable = false;
            if (Children.TryGetValue(kvp.Key, out var children))
                isResolvable = children.Any(x => x.IsResolvable);
            else
                isResolvable = worldStateModel.GetState(kvp.Key) >= kvp.Value;
            return isResolvable;
        });
        return isResolvableChildren;
    }

    public float CalculateTotalCost()
    {
        var leafCost = ActionInstance.DataComponent.CalculatedCost;
        var childrenGrouped = Children.Values.SelectMany(x => x)
                                      .GroupBy(x => x.PathId)
                                      .ToDictionary(x => x.Key, x => x.ToList());
        
        if (childrenGrouped.Count == 0)
            return leafCost;
        
        return childrenGrouped.OrderBy(x => x.Value.Sum(y => y.CachedTotalCost))
                              .First()
                              .Value
                              .Sum(x => x.CachedTotalCost) + leafCost;
    }

    public void AddChild(string preconditionMetKey, GoapPlanningLeaf child)
    {
        child.Parent = this;
        if (Children.TryGetValue(preconditionMetKey, out var children))
        {
            children.Add(child);
        }
        else
        {
            Children.Add(preconditionMetKey, new List<GoapPlanningLeaf>() { child });
        }
    }
    
    public void RemoveChild(GoapPlanningLeaf child)
    {
        child.Parent = null;
        foreach (var (key, children) in Children)
        {
            children.Remove(child);
            if (children.Count == 0)
                Children.Remove(key);
        }
    }
}