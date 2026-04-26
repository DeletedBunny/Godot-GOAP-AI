using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningLeaf
{
    public IGoapAction ActionInstance { get; set; }
    
    public GoapPlanningLeaf Parent { get; set; }
    public Dictionary<string, List<GoapPlanningLeaf>> Children { get; } = new();
    
    public float CalculatedCost => ActionInstance.ActionDataComponent.CalculatedCost;
    public float CachedTotalCost { get; set; }
    public bool IsResolvable { get; set; }

    public GoapPlanningLeaf(IGoapAction actionInstance)
    {
        ActionInstance = actionInstance;
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