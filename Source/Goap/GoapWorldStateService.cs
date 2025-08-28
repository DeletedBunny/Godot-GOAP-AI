using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.GOAP.Models;
using GodotGOAPAI.Source.Goap.World_State_Generator;

namespace GodotGOAPAI.Source.Goap;

public partial class GoapWorldStateService : Node
{
    public static GoapWorldStateService Instance { get; private set; }

    private readonly object _lock = new object();
    private readonly IGoapWorldStateGenerator<Node3D> _worldStateGenerator;
    
    public GoapWorldStateModel<Node3D> CurrentWorldStateModel { get; private set; }

    public GoapWorldStateService()
    {
        _worldStateGenerator = new GoapWorldStateGenerator<Node3D>();
    }
    
    public override void _Ready()
    {
        Instance = this;
    }

    public void GenerateWorldState(Node worldCollectionsRootNode)
    {
        CurrentWorldStateModel = _worldStateGenerator.GenerateWorldStateModel(worldCollectionsRootNode);
    }

    public Node3D GetClosestElementByType(GoapResourceType resourceType, Node3D agent)
    {
        CurrentWorldStateModel.ResourcesAmountByType.TryGetValue(resourceType, out List<Node3D> treeNodes);
		
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

    // Use this for planner so that you can modify the world state without affecting the real world when planning.
    public GoapWorldStateModel<Node3D> GetCopyOfCurrentWorldStateModel()
    {
        var resourcesAmountByType = new Dictionary<GoapResourceType, List<Node3D>>();
        foreach (var resourceType in CurrentWorldStateModel.ResourcesAmountByType)
        {
            resourcesAmountByType.Add(resourceType.Key, new List<Node3D>(resourceType.Value));
        }
        var worldStateCopy = new GoapWorldStateModel<Node3D>(resourcesAmountByType).WithEmptyInitialization();
        return worldStateCopy;
    }
}