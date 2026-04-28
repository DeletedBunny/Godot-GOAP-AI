using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateGenerator;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.WorldState;

public partial class GoapWorldStateService : Node
{
	public static GoapWorldStateService Instance { get; private set; }
	
	private readonly IGoapWorldStateGenerator _worldStateGenerator = new GoapWorldStateGenerator();
	private readonly object _worldStateLock = new object();
	
	private Node _worldDataCollectionsNode;
	private Node _agentsCollectionNode;

	private GoapWorldStateModel _currentWorldStateModelModel;

	public Node WorldTreesCollectionNode => _worldDataCollectionsNode.GetNode("Trees");
	public Node WorldMountainsCollectionNode => _worldDataCollectionsNode.GetNode("Mountains");
	public Node WorldBushesCollectionNode => _worldDataCollectionsNode.GetNode("Bushes");
	public Node WorldItemsCollectionNode => _worldDataCollectionsNode.GetNode("Items");

	public override void _Ready()
	{
		Instance = this;
		EventBus.Instance.Subscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
		EventBus.Instance.Subscribe<WorldStateChangedEvent>(OnWorldStateChanged);
	}

	private void OnRegisterWorldCollectionNodes(IEvent eventData)
	{
		if (eventData is not RegisterWorldCollectionNodesEvent registerWorldCollectionNodesEvent)
			return;
		
		EventBus.Instance.Unsubscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
		_worldDataCollectionsNode = registerWorldCollectionNodesEvent.WorldCollectionsRootNode;
		_agentsCollectionNode = registerWorldCollectionNodesEvent.AgentCollectionsRootNode;
		lock (_worldStateLock)
		{
			GenerateWorldState();
		}
	}

	private void OnWorldStateChanged(IEvent eventData)
	{
		if (eventData is not WorldStateChangedEvent worldStateChangedEvent)
			return;

		lock (_worldStateLock)
		{
			foreach (var kvp in worldStateChangedEvent.ChangedNodes)
			{
				if (worldStateChangedEvent.IsRemoved)
				{
					_currentWorldStateModelModel.RemoveItems(kvp.Key, kvp.Value);
				}
				else
				{
					_currentWorldStateModelModel.AddItems(kvp.Key, kvp.Value);
				}
			}
		}
	}
	
	private void GenerateWorldState()
	{
		_currentWorldStateModelModel = _worldStateGenerator.GenerateWorldStateModel(_worldDataCollectionsNode, _agentsCollectionNode);
	}

	public GoapWorldStateModel GetWorldStateForSimulation(IEnumerable<(string, int)> agentInternalState)
	{
		lock (_worldStateLock)
		{
			return _currentWorldStateModelModel.GetSimulationCopy(agentInternalState);
		}
	}

	public void EntityPickedUp(Node3D entity)
	{
		if (entity is IEntity entityInterface)
		{
			lock (_worldStateLock)
			{
				_currentWorldStateModelModel.RemoveItems(entityInterface.EntityType, [entity]);
			}

			return;
		}

		GD.PrintErr($"EntityPickedUp: Entity {entity} is not an IEntity");
	}

	public void EntityDropped(Node3D entity)
	{
		if (entity is IEntity entityInterface)
		{
			lock (_worldStateLock)
			{
				_currentWorldStateModelModel.AddItems(entityInterface.EntityType, [entity]);
			}

			return;
		}
		
		GD.PrintErr($"EntityDropped: Entity {entity} is not an IEntity");
	}

	public void ReserveEntity(EntityType entityType, Node3D entityNode)
	{
		lock (_worldStateLock)
		{
			_currentWorldStateModelModel.RemoveItems(entityType, [entityNode]);
		}
	}
	
	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
	}
}
