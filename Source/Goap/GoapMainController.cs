using System;
using Godot;
using GodotGOAPAI.Source.Event_System;
using GodotGOAPAI.Source.Goap.Planner;
using GodotGOAPAI.Source.Goap.World_State;
using GodotGOAPAI.Source.Goap.World_State.World_State_Events;
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
		
		//if (_start && _agentsCollectionNode.GetChildren().FirstOrDefault() is Node3D agent)
		{
			//var target = GoapGoapWorldStateService.Instance.GetClosestElementByType(GoapResourceType.Tree, agent);
			//if (target != null)
			{
				//_tempActionTest = new GoapMoveToAction();
				//_tempActionTest.Initialize(new GoapMoveToActionParams(target, agent, 3));
			}
		}
		
		//if (!(_tempActionTest?.IsCompletedConditionMet() ?? true))
			//_tempActionTest.ExecuteAction((float)delta);
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		base._ExitTree();
	}
}
