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
		EventBus.Instance.Subscribe<GatherResourcesEvent>(OnGatherResourcesEvent);
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
		var agent = _agentsCollectionNode.GetChild(0);

		if (agent is not IAgentPlanner agent3D)
			return;
		
		if (buildEvent is not PlanBuildEvent planBuildEvent || _planningStarted || !agent3D.IsReadyToPlan)
			return;

		_planningStarted = true;
		GoapEntityToGoalFactory.GetGoal(planBuildEvent.BuildingType, out var goal);
		_planner.Plan(agent3D, goal);
		_planningStarted = false;
	}

	private void OnGatherResourcesEvent(IEvent gatherEvent)
	{
		var agent = _agentsCollectionNode.GetChild(0);

		if (agent is not IAgentPlanner agent3D)
			return;
		
		if (gatherEvent is not GatherResourcesEvent gatherResourcesEvent || _planningStarted || !agent3D.IsReadyToPlan)
			return;

		_planningStarted = true;
		
		var goal = new GoapActionPreconditionComponent()
		{
			Preconditions = 
			[
				new("LogInWorld", gatherResourcesEvent.LogAmount), 
				new("StoneInWorld", gatherResourcesEvent.StoneAmount)
			]
		};
		
		_planner.Plan(agent3D, goal);
		_planningStarted = false;
	}

	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.Unsubscribe<PlanBuildEvent>(OnBuildEvent);
		EventBus.Instance.Unsubscribe<GatherResourcesEvent>(OnGatherResourcesEvent);
		base._ExitTree();
	}
}
