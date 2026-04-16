using Godot;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;

namespace GodotGOAPAI.Source.Goap.Actions.Pick_Up_Item;

public class GoapActionPickUpItem : GoapActionBase
{
    protected override float DistanceCostMultiplier => 0.5f;
    protected override float TimeCostMultiplier => 0.8f;
    protected override float TimeCostInSeconds => 5;
    
    public override bool IsActionPreconditionsValid(Node3D agent, GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult)
    {
        throw new System.NotImplementedException();
    }

    public override GoapActionResult GetActionResult()
    {
        throw new System.NotImplementedException();
    }
}