using System;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.UI;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.Goap;

public partial class GoapMainController : Node
{
	private readonly GoapPlanner _planner = new();
	[Export] private Node _agentsCollectionNode;
	[Export] private Node _worldDataCollectionsNode;

	private bool _planningStarted;

	public override void _Ready()
	{
		EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.Subscribe<PlanBuildEvent>(OnBuildEvent);
		base._Ready();
	}

	private void OnWorldReady(IEvent _)
	{
		if (_worldDataCollectionsNode == null || _agentsCollectionNode == null)
			throw new Exception("World data collections node or agents collection node is null");
		
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.SendEvent(new RegisterWorldCollectionNodesEvent(_worldDataCollectionsNode, _agentsCollectionNode));
	}

	private void OnBuildEvent(IEvent buildEvent)
	{
		if (buildEvent is not PlanBuildEvent planBuildEvent || _planningStarted || _planner.IsExecuting)
			return;

		_planningStarted = true;
		GoapEntityToGoalFactory.EntityToGoal.TryGetValue(planBuildEvent.BuildingType, out var goal);
		var agent = _agentsCollectionNode.GetChild(0);
		_planner.Plan(agent as Agent3D, goal);
		_planningStarted = false;
	}

	public override void _Process(double delta)
	{
		_planner.Execute(delta);
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		base._ExitTree();
	}
}
