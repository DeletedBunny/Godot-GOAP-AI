using Godot;

namespace GodotGOAPAI.Source.GOAP.Actions.Abstraction;

public abstract class GoapActionBase : IGoapAction
{
    protected abstract int BaseCost { get; }

    public abstract void Initialize(IGoapActionParams goapActionParams);
    
    public virtual int CalculateCost()
    {
        return BaseCost;
    }

    public abstract bool IsActionPreconditionsValid();
    public abstract bool IsActionEffectsValid();
    
    public abstract void ExecuteAction(float deltaTime);
    public abstract bool IsCompletedConditionMet();
}