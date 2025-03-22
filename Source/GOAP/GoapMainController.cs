using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotGOAPAI.Source.EventBus;
using GodotGOAPAI.Source.GOAP.Abstraction;
using GodotGOAPAI.Source.GOAP.Actions.MoveTo;
using GodotGOAPAI.Source.GOAP.Godot;
using GodotGOAPAI.Source.GOAP.Godot.WorldStateGenerator;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.GOAP.WorldStateGenerator;
using GodotGOAPAI.Source.World;

namespace GodotGOAPAI.Source.GOAP;

public partial class GoapMainController : Node
{
    private readonly IGoapWorldStateGenerator<Node, Node3D> _worldStateGenerator;
    
    [Export] private Node _worldDataCollectionsNode;
    [Export] private Node _agentsCollectionNode;

    private GoapMoveToAction _tempActionTest;
    private bool _start;
    
    public GoapWorldStateModel<Node3D> CurrentWorldStateModel { get; private set; }

    public GoapMainController()
    {
        _worldStateGenerator = new GodotGoapWorldStateGenerator();
    }
    
    public override void _Ready()
    {
        EventBus.EventBus.Instance.Subscribe<WorldReadyEvent>(OnWorldReady);
    }

    private void OnWorldReady(IEvent _)
    {
        var worldDataCollectionsGodotNode = new GameObject<Node>(_worldDataCollectionsNode);
        CurrentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(worldDataCollectionsGodotNode);
    }

    private Node3D ClosestTreeTest(Node3D agent)
    {
        CurrentWorldStateModel.ResourcesAmountByType.TryGetValue(GoapResourceType.Tree, out List<Node3D> treeNodes);
        
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

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_up"))
            _start = true;
        
        if (_start && _agentsCollectionNode.GetChildren().FirstOrDefault() is Node3D agent)
        {
            var target = ClosestTreeTest(agent);
            if (target != null)
            {
                _tempActionTest = new GoapMoveToAction();
                _tempActionTest.Initialize(new GoapMoveToActionParams(target, agent, 3));
            }
        }
        
        if (!(_tempActionTest?.IsCompletedConditionMet() ?? true))
            _tempActionTest.ExecuteAction((float)delta);
    }

    public override void _ExitTree()
    {
        EventBus.EventBus.Instance.Unsubscribe<WorldReadyEvent>(OnWorldReady);
    }
}