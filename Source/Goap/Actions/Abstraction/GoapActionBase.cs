using Godot;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;

namespace GodotGOAPAI.Source.Goap.Actions.Abstraction;

public abstract class GoapActionBase : IGoapAction
{
    private readonly Vector3 _reachedPositionThreshold = new Vector3(0.2f, 0.2f, 0.2f);
    
    private float _distanceCost;
    private Node3D _target;
    private Node3D _agent;
    private int _agentMoveSpeed;
    
    protected abstract float DistanceCostMultiplier { get; }
    protected abstract float TimeCostMultiplier { get; }
    protected abstract float TimeCostInSeconds { get; }

    public virtual void Initialize(GoapActionParamsBase actionParams)
    {
        _target = actionParams.Target;
        _agent = actionParams.Agent;
        _agentMoveSpeed = actionParams.AgentMoveSpeed;
        _distanceCost = _agent.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition);
    }
    
    public virtual int CalculateCost()
    {
        return (int)(DistanceCostMultiplier * _distanceCost + TimeCostMultiplier * TimeCostInSeconds);
    }

    public abstract bool IsActionPreconditionsValid(Node3D agent, GoapWorldStateMemento<Node3D> worldStateMemento,
        GoapActionResult actionResult);
    public abstract GoapActionResult GetActionResult();

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