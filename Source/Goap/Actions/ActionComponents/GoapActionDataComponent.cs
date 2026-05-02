namespace GodotGOAPAI.Source.Goap.Actions.ActionComponents;

public class GoapActionDataComponent
{
    public float TimeCostMultiplier { get; init; }
    public float TimeCostInSeconds { get; set; }
    public float CalculatedCost => TimeCostMultiplier * TimeCostInSeconds;
    
    public GoapActionDataComponent Clone() => new()
    {
        TimeCostMultiplier = TimeCostMultiplier, 
        TimeCostInSeconds = TimeCostInSeconds
    };
}