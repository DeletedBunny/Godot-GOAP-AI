using GodotGOAPAI.Source.GOAP.Godot.Actions.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Godot.Actions.CutTree;

public class GoapActionCutTree : GoapActionBase
{
    protected override float DistanceCostMultiplier => 1;
    protected override float TimeCostMultiplier => 1;
    protected override float TimeCost => 5;
    
    public override bool IsActionPreconditionsValid()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsActionEffectsValid()
    {
        throw new System.NotImplementedException();
    }
}