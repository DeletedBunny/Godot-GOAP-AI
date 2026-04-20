namespace GodotGOAPAI.Source.Goap.Actions.ActionData;

public class GoapActionDataComponent
{
    public float DistanceCost { get; set; }
    public float DistanceCostMultiplier { get; set; }
    public float TimeCostMultiplier { get; set; }
    public float TimeCostInSeconds { get; set; }
    public int CalculatedCost => (int) (DistanceCost * DistanceCostMultiplier + TimeCostMultiplier * TimeCostInSeconds);
}