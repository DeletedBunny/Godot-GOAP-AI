using System;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.Actions.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.ActionComponents;
using GodotGOAPAI.Source.Goap.Agent;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.Goap;

public partial class GoapMainController : Node
{
	private readonly GoapPlanner _planner = new GoapPlanner();
	[Export] private Node _agentsCollectionNode;
	[Export] private Node _worldDataCollectionsNode;
	
	private bool _start;

	public override void _Ready()
	{
		EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
		base._Ready();
	}

	private void OnWorldReady(IEvent _)
	{
		if (_worldDataCollectionsNode == null || _agentsCollectionNode == null)
			throw new Exception("World data collections node or agents collection node is null");
		
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.SendEvent(new RegisterWorldCollectionNodesEvent(_worldDataCollectionsNode, _agentsCollectionNode));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_up"))
			_start = true;

		if (_start)
		{
			var agent = _agentsCollectionNode.GetChild(0);
			var goal = new GoapActionEffectComponent();
			goal.AddActionResult("log", 4);
			_planner.Plan(agent as Agent3D, goal);
			_start = false;
		}
		
		_planner.Execute(delta);
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		base._ExitTree();
	}
}
