using System.Linq;
using Godot;
using GodotGOAPAI.Source.Event_System;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.Goap;

public partial class GoapMainController : Node
{
	
	[Export] private Node _worldDataCollectionsNode;
	[Export] private Node _agentsCollectionNode;
	
	//private GoapMoveToAction _tempActionTest;
	private bool _start;

	public GoapMainController()
	{
	}
	
	public override void _Ready()
	{
		EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
	}

	private void OnWorldReady(IEvent _)
	{
		GoapWorldStateService.Instance.GenerateWorldState(_worldDataCollectionsNode);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_up"))
			_start = true;
		
		if (_start && _agentsCollectionNode.GetChildren().FirstOrDefault() is Node3D agent)
		{
			var target = GoapWorldStateService.Instance.GetClosestElementByType(GoapResourceType.Tree, agent);
			if (target != null)
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
	}
}
