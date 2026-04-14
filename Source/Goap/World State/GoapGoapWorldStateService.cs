using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.Event_System;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State.World_State_Generator;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.Goap.World_State;

public partial class GoapGoapWorldStateService : Node, IGoapWorldStateService
{
    public static GoapGoapWorldStateService Instance { get; set; }
    
    private readonly object _lock = new object();
    private readonly IGoapWorldStateGenerator<Node3D> _worldStateGenerator = new GoapWorldStateGenerator<Node3D>();
    private readonly List<GoapWorldStateModel<Node3D>> _worldStateMementos = new List<GoapWorldStateModel<Node3D>>();
    private readonly object _worldStateLock = new object();
    
    [Export] private Node _worldDataCollectionsNode;
    [Export] private Node _agentsCollectionNode;

    private GoapWorldStateModel<Node3D> _currentWorldStateModel;

    public override void _Ready()
    {
        Instance = this;
        EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
    }
    
    private void OnWorldReady(IEvent _)
    {
        lock (_worldStateLock)
        {
            GenerateWorldState();
        }
    }
    
    private void GenerateWorldState()
    {
        _currentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(_worldDataCollectionsNode, _agentsCollectionNode);
    }

    public GoapWorldStateModel<Node3D> GetWorldStateMemento()
    {
        lock (_worldStateLock)
        {
            var worldStateMemento = GetCopyOfCurrentWorldStateModel();
            _worldStateMementos.Add(worldStateMemento);
            return worldStateMemento;
        }
    }

    public void ApplyWorldStateMemento()
    {
        lock (_worldStateLock)
        {
            _currentWorldStateModel = _worldStateMementos.LastOrDefault();
            if(_currentWorldStateModel == null)
                GenerateWorldState();
        }
    }
    
    private GoapWorldStateModel<Node3D> GetCopyOfCurrentWorldStateModel()
    {
        var resourcesAmountByType = new Dictionary<GoapResourceType, List<Node3D>>();
        foreach (var resourceType in _currentWorldStateModel.ResourcesAmountByType)
        {
            resourcesAmountByType.Add(resourceType.Key, new List<Node3D>(resourceType.Value));
        }
        var worldStateCopy = new GoapWorldStateModel<Node3D>(resourcesAmountByType).WithEmptyInitialization();
        return worldStateCopy;
    }

    //TODO move this because it makes no sense here
    public Node3D GetClosestElementByType(GoapResourceType resourceType, Node3D agent)
    {
        _currentWorldStateModel.ResourcesAmountByType.TryGetValue(resourceType, out List<Node3D> treeNodes);
		
        treeNodes ??= new List<Node3D>();
        Node3D closestTree = null;
		
        foreach (var tree in treeNodes)
        {
            closestTree ??= tree;
			
            var distanceToTree = agent.GlobalPosition.DistanceSquaredTo(tree.GlobalPosition);
            var distanceToClosestTree = agent.GlobalPosition.DistanceSquaredTo(closestTree.GlobalPosition);
            if(distanceToTree < distanceToClosestTree)
                closestTree = tree;
        }
		
        return closestTree;
    }
    
    public override void _ExitTree()
    {
        EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
    }
}