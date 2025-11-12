using GodotGOAPAI.Source.Goap.Actions.Abstraction;

namespace GodotGOAPAI.Source.Goap.Actions.Cut_Tree;

public class GoapActionCutTree : GoapActionBase
{
    protected override float DistanceCostMultiplier => 1;
    protected override float TimeCostMultiplier => 1;
    protected override float TimeCost => 5;
    
    public override bool IsActionPreconditionsValid()
    {
        GoapWorldStateService.Instance.CurrentWorldStateModel.
    }

    public override bool IsActionEffectsValid()
    {
        throw new System.NotImplementedException();
    }
}