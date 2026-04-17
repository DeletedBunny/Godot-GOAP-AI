using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateGenerator;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.Goap.WorldState;

public partial class GoapWorldStateService : Node
{
	public static GoapWorldStateService Instance { get; set; }
	
	private readonly IGoapWorldStateGenerator<Node3D> _worldStateGenerator = new GoapWorldStateGenerator<Node3D>();
	private readonly List<GoapWorldStateMemento<Node3D>> _worldStateMementos = new List<GoapWorldStateMemento<Node3D>>();
	private readonly object _worldStateLock = new object();
	
	private Node _worldDataCollectionsNode;
	private Node _agentsCollectionNode;

	private GoapWorldStateModel<Node3D> _currentWorldStateModel;

	public Node WorldTreesCollectionNode => _worldDataCollectionsNode.GetNode("Trees");
	public Node WorldMountainsCollectionNode => _worldDataCollectionsNode.GetNode("Mountains");
	public Node WorldBushesCollectionNode => _worldDataCollectionsNode.GetNode("Bushes");
	public Node WorldItemsCollectionNode => _worldDataCollectionsNode.GetNode("Items");

	public override void _Ready()
	{
		Instance = this;
		EventBus.Instance.Subscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
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
	
	private void GenerateWorldState()
	{
		_currentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(_worldDataCollectionsNode, _agentsCollectionNode);
	}

	public GoapWorldStateMemento<Node3D> GetWorldStateMemento()
	{
		lock (_worldStateLock)
		{
			var worldStateModelCopy = _currentWorldStateModel.GetCopy();
			var worldStateMemento = new GoapWorldStateMemento<Node3D>(worldStateModelCopy);
			_worldStateMementos.Add(worldStateMemento);
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

	public void EntityPickedUp(Node3D entity)
	{
		lock (_worldStateLock)
		{
			if (entity is IEntity entityInterface)
			{
				_currentWorldStateModel.RemoveItems(entityInterface.EntityType, [entity]);
			}
			else
			{
				GD.PrintErr($"EntityPickedUp: Entity {entity} is not an IEntity");
				return;
			}

			EventBus.Instance.SendEvent<WorldStateChangedEvent>();
		}
	}

	public void EntityDropped(Node3D entity)
	{
		lock (_worldStateLock)
		{
			if (entity is IEntity entityInterface)
			{
				_currentWorldStateModel.AddItems(entityInterface.EntityType, [entity]);
			}
			else
			{
				GD.PrintErr($"EntityDropped: Entity {entity} is not an IEntity");
			}
			
			EventBus.Instance.SendEvent<WorldStateChangedEvent>();
		}
	}
	
	public override void _ExitTree()
	{
		EventBus.Instance.Unsubscribe<RegisterWorldCollectionNodesEvent>(OnRegisterWorldCollectionNodes);
	}
}
