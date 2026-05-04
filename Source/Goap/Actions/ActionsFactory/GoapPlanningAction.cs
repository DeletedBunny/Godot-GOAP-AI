using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Actions.ActionsFactory;

public class GoapPlanningAction
{
    public IGoapAction ActionInstance { get; init; }
    public int RepeatCount { get; init; }
}