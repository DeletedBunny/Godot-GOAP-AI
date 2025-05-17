using Godot;

namespace GodotGOAPAI.Source.GOAP.Godot.Actions.Abstraction;

public abstract class GoapActionBase : IGoapAction
{
    private readonly Vector3 _reachedPositionThreshold = new Vector3(0.2f, 0.2f, 0.2f);
    
    private float _distanceCost;
    private Node3D _target;
    private Node3D _agent;
    private int _agentMoveSpeed;
    
    protected abstract float DistanceCostMultiplier { get; }
    protected abstract float TimeCostMultiplier { get; }
    protected abstract float TimeCost { get; }

    public virtual void Initialize(GoapActionParamsBase actionParams)
    {
        _target = actionParams.Target;
        _agent = actionParams.Agent;
        _agentMoveSpeed = actionParams.AgentMoveSpeed;
        _distanceCost = _agent.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition);
    }
    
    public virtual int CalculateCost()
    {
        return (int)(DistanceCostMultiplier * _distanceCost + TimeCostMultiplier * TimeCost);
    }

    public abstract bool IsActionPreconditionsValid();
    public abstract bool IsActionEffectsValid();

    public virtual void ExecuteAction(float deltaTime)
    {
        var moveDirection = _agent.GlobalPosition.DirectionTo(_target.GlobalPosition);
        _agent.GlobalTranslate(_agentMoveSpeed * moveDirection * deltaTime);
    }

    public virtual bool IsCompletedConditionMet()
    {
        var positionDifference = _agent.GlobalPosition - _target.GlobalPosition;
        return positionDifference.Abs() <= _reachedPositionThreshold;
    }
}