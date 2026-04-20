using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Actions.ActionData;

public class GoapActionDataComponent
{
    public float TimeCostMultiplier { get; init; }
    public float TimeCostInSeconds { get; set; }
    public int CalculatedCost => (int) (TimeCostMultiplier * TimeCostInSeconds);
}