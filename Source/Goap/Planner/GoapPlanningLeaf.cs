using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningLeaf
{
    public GoapActionType ActionType { get; private set; }
    public GoapActionEffectComponent ActionEffectComponent { get; private set; }
    
    public List<GoapPlanningLeaf> Children { get; private set; }

    public GoapPlanningLeaf(GoapActionType actionType, GoapActionEffectComponent actionEffectComponent)
    {
        ActionType = actionType;
        ActionEffectComponent = actionEffectComponent;
    }

    public void AddChild(GoapPlanningLeaf child)
    {
        Children.Add(child);
    }
    
    public void RemoveChild(GoapPlanningLeaf child)
    {
        Children.Remove(child);
    }
}