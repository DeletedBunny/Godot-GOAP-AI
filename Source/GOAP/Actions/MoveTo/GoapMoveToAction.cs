using System;
using Godot;
using GodotGOAPAI.Source.GOAP.Actions.Abstraction;

namespace GodotGOAPAI.Source.GOAP.Actions.MoveTo;

public class GoapMoveToAction : GoapActionBase
{
    private float _distanceCost;
    private Node3D _target;
    private Node3D _agent;
    private int _agentMoveSpeed;
    
    protected override int BaseCost => 1;

    public override void Initialize(IGoapActionParams goapActionParams)
    {
        if (goapActionParams is GoapMoveToActionParams actionParams)
        {
            _target = actionParams.Target;
            _agent = actionParams.Agent;
            _agentMoveSpeed = actionParams.AgentMoveSpeed;
            _distanceCost = _agent.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition);
        }
        else
        {
            throw new ArgumentException("IGoapActionParams must be of type GoapMoveToActionParams");
        }
    }

    public override int CalculateCost()
    {
        return base.CalculateCost() * (int)_distanceCost;
    }

    public override bool IsActionPreconditionsValid()
    {
        // Here is where we would use our path finding AI to determine if you can get there
        return true;
    }

    public override bool IsActionEffectsValid()
    {
        // The effect is we are next to the object we need so it's always true
        return true;
    }

    public override void ExecuteAction(float deltaTime)
    {
        var moveDirection = _agent.GlobalPosition.DirectionTo(_target.GlobalPosition);
        _agent.GlobalTranslate(_agentMoveSpeed * moveDirection * deltaTime);
    }

    public override bool IsCompletedConditionMet()
    {
        var positionDifference = _agent.GlobalPosition - _target.GlobalPosition;
        return positionDifference.Abs() <= new Vector3(0.2f, 0.2f, 0.2f);
    }
}