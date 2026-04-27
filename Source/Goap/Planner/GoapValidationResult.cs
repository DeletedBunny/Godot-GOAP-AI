using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Planner;

public class GoapValidationResult
{
    public float Cost { get; set; }
    public IGoapAction Action { get; set; }
}