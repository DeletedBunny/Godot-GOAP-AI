using GodotGOAPAI.Source.Goap.Actions;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningAction
{
    public GoapActionType Type { get; set; }
    public GoapActionEffectComponent EffectsComponent { get; set; }
    public GoapActionPreconditionComponent PreconditionsComponent { get; set; }
    public int RepeatCount { get; set; }
}