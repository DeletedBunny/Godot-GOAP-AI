using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapPlanningAction
{
    public IGoapAction ActionInstance { get; set; }
    public int RepeatCount { get; set; }
}