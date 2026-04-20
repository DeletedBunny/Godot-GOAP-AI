using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStateMemento<TNode> where TNode : Node
{
    private const string TrackedWorldStateModifier = "InWorld";
    private readonly GoapWorldStateModel<TNode> _worldStateModel;
    private readonly Dictionary<EntityType, List<TNode>> _removedResources = new();
    private readonly Dictionary<EntityType, List<TNode>> _addedResources = new();
    private readonly Dictionary<string, int> _trackedWorldStates = new();
    
    public bool IsWorldStateModified { get; private set; }
    
    public GoapWorldStateMemento(GoapWorldStateModel<TNode> worldStateModel)
    {
        _worldStateModel = worldStateModel;
        GenerateTrackedWorldStates();
    }

    private void GenerateTrackedWorldStates()
    {
        _trackedWorldStates.Clear();
        foreach (var resourceInWorld in _worldStateModel.ResourcesAmountByType)
        {
            SetTrackedState(resourceInWorld.Key.ToString(), resourceInWorld.Value.Count);
        }
    }
    
    public int GetTrackedState(string key) => _trackedWorldStates.GetValueOrDefault(key + TrackedWorldStateModifier, 0);
    public void SetTrackedState(string key, int value) => _trackedWorldStates[key + TrackedWorldStateModifier] = value;

    public void ApplyModificationsToWorldState()
    {
        var isWorldStateModified = false;
        foreach (var (key, removedResourceNodes) in _removedResources)
        {
            _worldStateModel.RemoveItems(key, removedResourceNodes);
            isWorldStateModified = true;
        }

        foreach (var (key, addedResourceNodes) in _addedResources)
        {
            _worldStateModel.AddItems(key, addedResourceNodes);
            isWorldStateModified = true;
        }
        
        IsWorldStateModified = isWorldStateModified;
    }
    
    public void AddModifiedResource(EntityType entityType, List<TNode> nodes, bool isRemoved)
    {
        if (isRemoved)
        {
            if (_removedResources.TryGetValue(entityType, out var resource))
            {
                resource.AddRange(nodes);
            }
            else
            {
                _removedResources.Add(entityType, nodes);
            }
            var newValue = GetTrackedState(entityType.ToString()) - nodes.Count;
            SetTrackedState(entityType.ToString(), newValue);
        }
        else
        {
            if (_addedResources.TryGetValue(entityType, out var resource))
            {
                resource.AddRange(nodes);
            }
            else
            {
                _addedResources.Add(entityType, nodes);
            }

            var newValue = GetTrackedState(entityType.ToString()) + nodes.Count;
            SetTrackedState(entityType.ToString(), newValue);
        }
    }
    
    public GoapWorldStateModel<TNode> GetWorldStateModel()
    {
        return _worldStateModel.GetCopy();
    }

    public GoapWorldStateMemento<TNode> GetCopy()
    {
        var worldStateMemento = new GoapWorldStateMemento<TNode>(_worldStateModel.GetCopy());
        foreach (var (key, value) in _addedResources)
        {
            var nodesCopy = new List<TNode>(value);
            worldStateMemento.AddModifiedResource(key, nodesCopy, false);
        }

        foreach (var (key, value) in _removedResources)
        {
            var nodesCopy = new List<TNode>(value);
            worldStateMemento.AddModifiedResource(key, nodesCopy, true);
        }
        return worldStateMemento;
    }

    public  IReadOnlyList<TNode> GetResource(EntityType entityType)
    {
        var resourceWithModifiedNodes = new List<TNode>();
        if (_worldStateModel.ResourcesAmountByType.TryGetValue(entityType, out var resourceNodes))
            resourceWithModifiedNodes.AddRange(resourceNodes);
        
        if(_removedResources.TryGetValue(entityType, out var removedResourceNodes))
            resourceWithModifiedNodes.RemoveAll(removedResourceNodes.Contains);
        
        if(_addedResources.TryGetValue(entityType, out var addedResourceNodes))
            resourceWithModifiedNodes.AddRange(addedResourceNodes);
        
        return resourceWithModifiedNodes;
    }
    
    // Would be better to replace TNode generic with a custom type like GodotNode to pick between Node2D or Node3D based
    // on the type of the node. This would allow flexibility in adding new node types, although besides 3D and 2D I
    // doubt there will be another base positional node since we can't perceive higher than 3D.
    public TNode GetClosestElementByType(EntityType entityType, TNode agent)
    {
        if (entityType == EntityType.Unknown)
            return null;
        _worldStateModel.ResourcesAmountByType.TryGetValue(entityType, out var resourceNodes);
		var resourceWithModifiedNodes = new List<TNode>();
        
        if (resourceNodes != null)
            resourceWithModifiedNodes.AddRange(resourceNodes);
        
        TNode closestNode = null;
		
        foreach (var node in resourceWithModifiedNodes)
        {
            closestNode ??= node;
			
            var distanceToNode = GetDistanceToElement(agent, node);
            var distanceToClosestNode = GetDistanceToElement(agent, closestNode);
            if(distanceToNode < distanceToClosestNode)
                closestNode = node;
        }
		
        return closestNode;
    }

    private float GetDistanceToElement(TNode agent, TNode element)
    {
        if(agent is Node3D agent3D && element is Node3D element3D)
            return GetDistanceToElement3D(agent3D, element3D);
        
        if(agent is Node2D agent2D && element is Node2D element2D)
            return GetDistanceToElement2D(agent2D, element2D);
        
        throw new System.NotImplementedException(
            $"GetDistanceToElement method is not implemented for agent of type = {agent.GetType()} and element of type = {element.GetType()}"
        );
    }

    private float GetDistanceToElement3D(Node3D agent, Node3D element)
    {
        return agent.GlobalPosition.DistanceSquaredTo(element.GlobalPosition);
    }

    private float GetDistanceToElement2D(Node2D agent, Node2D element)
    {
        return agent.GlobalPosition.DistanceSquaredTo(element.GlobalPosition);   
    }
    
    public void RevertModificationsToWorldState()
    {
        if(!IsWorldStateModified)
            return;

        foreach (var (key, addedResourceNodes) in _addedResources)
        {
            _worldStateModel.RemoveItems(key, addedResourceNodes);
        }
        
        foreach (var (key, removedResourceNodes) in _removedResources)
        {
            _worldStateModel.AddItems(key, removedResourceNodes);
        }
        
        GenerateTrackedWorldStates();
        IsWorldStateModified = false;
    }
}