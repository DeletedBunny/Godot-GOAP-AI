using System.Collections.Generic;
using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningLeaf
{
    public GoapActionType ActionType { get; private set; }
    public GoapActionEffectComponent ActionEffectComponent { get; private set; }
    public GoapActionPreconditionComponent ActionPreconditionComponent { get; private set; }
    public GoapPlanningLeaf MoveToActionLeaf { get; set; }
    
    public GoapPlanningLeaf Parent { get; set; }
    public List<GoapPlanningLeaf> Children { get; private set; }

    public GoapPlanningLeaf(GoapActionType actionType, GoapActionEffectComponent actionEffectComponent, GoapActionPreconditionComponent actionPreconditionComponent)
    {
        ActionType = actionType;
        ActionEffectComponent = actionEffectComponent;
        ActionPreconditionComponent = actionPreconditionComponent;
    }

    public void AddChild(GoapPlanningLeaf child)
    {
        child.Parent = this;
        Children.Add(child);
    }
    
    public void RemoveChild(GoapPlanningLeaf child)
    {
        child.Parent = null;
        Children.Remove(child);
    }
}