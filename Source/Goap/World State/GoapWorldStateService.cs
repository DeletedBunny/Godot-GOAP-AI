using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Event_System;
using GodotGOAPAI.Source.Goap.World_State.World_State_Events;
using GodotGOAPAI.Source.Goap.World_State.World_State_Generator;
using GodotGOAPAI.Source.Goap.World_State.World_State_Models;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.Goap.World_State;

public partial class GoapWorldStateService : Node, IGoapWorldStateService
{
	public static GoapWorldStateService Instance { get; set; }
	
	private readonly IGoapWorldStateGenerator<Node3D> _worldStateGenerator = new GoapWorldStateGenerator<Node3D>();
	private readonly List<GoapWorldStateMemento<Node3D>> _worldStateMementos = new List<GoapWorldStateMemento<Node3D>>();
	private readonly object _worldStateLock = new object();
	
	private Node _worldDataCollectionsNode;
	private Node _agentsCollectionNode;

	private GoapWorldStateModel<Node3D> _currentWorldStateModel;

	public override void _Ready()
	{
		Instance = this;
		EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.Subscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
	}
	
	private void OnWorldReady(IEvent _)
	{
		lock (_worldStateLock)
		{
			GenerateWorldState();
		}
	}

	private void OnRegisterWorldCollectionNodes(IEvent eventData)
	{
		if (eventData is not RegisterWorldCollectionNodesEvent registerWorldCollectionNodesEvent)
			return;
		
		EventBus.Instance.Unsubscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
		_worldDataCollectionsNode = registerWorldCollectionNodesEvent.WorldCollectionsRootNode;
		_agentsCollectionNode = registerWorldCollectionNodesEvent.AgentCollectionsRootNode;
	}
	
	private void GenerateWorldState()
	{
		_currentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(_worldDataCollectionsNode, _agentsCollectionNode);
	}

	public GoapWorldStateModel<Node3D> GetWorldStateMemento()
	{
		lock (_worldStateLock)
		{
			var worldStateMemento = _currentWorldStateModel.GetCopy();
			_worldStateMementos.Add(new GoapWorldStateMemento<Node3D>(worldStateMemento));
			return worldStateMemento;
		}
	}

	public void ApplyWorldStateMemento()
	{
		lock (_worldStateLock)
		{
			var worldStateMemento = _worldStateMementos.LastOrDefault();

			if (worldStateMemento == null)
			{
				GenerateWorldState();
				return;
			}

			worldStateMemento.ApplyModificationsToWorldState();
			_currentWorldStateModel = worldStateMemento.GetWorldStateModel();
			_worldStateMementos.RemoveAt(_worldStateMementos.Count - 1);
			EventBus.Instance.SendEvent<WorldStateChangedEvent>();
		}
	}
	
	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
		EventBus.Instance.Unsubscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
	}
}
