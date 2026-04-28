using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStateMemento
{
    private readonly GoapWorldStateModel _worldStateModel;
    private readonly Dictionary<EntityType, List<Node3D>> _removedResources = new();
    private readonly Dictionary<EntityType, List<Node3D>> _addedResources = new();
    
    public bool IsWorldStateModified { get; private set; }
    
    public GoapWorldStateMemento(GoapWorldStateModel worldStateModel)
    {
        _worldStateModel = worldStateModel;
    }

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
    
    public void AddModifiedResource(EntityType entityType, List<Node3D> nodes, bool isRemoved)
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
        }
    }
    
    public GoapWorldStateModel GetWorldStateModel()
    {
        return _worldStateModel.GetCopy();
    }

    public ImmutableDictionary<EntityType, List<Node3D>> GetWorldStateResources()
    {
        return _worldStateModel.ResourcesAmountByType.ToImmutableDictionary();
    }

    public GoapWorldStateMemento GetCopy()
    {
        var worldStateMemento = new GoapWorldStateMemento(_worldStateModel.GetCopy());
        foreach (var (key, value) in _addedResources)
        {
            var nodesCopy = new List<Node3D>(value);
            worldStateMemento.AddModifiedResource(key, nodesCopy, false);
        }

        foreach (var (key, value) in _removedResources)
        {
            var nodesCopy = new List<Node3D>(value);
            worldStateMemento.AddModifiedResource(key, nodesCopy, true);
        }
        return worldStateMemento;
    }

    public  IReadOnlyList<Node3D> GetResource(EntityType entityType)
    {
        var resourceWithModifiedNodes = new List<Node3D>();
        if (_worldStateModel.ResourcesAmountByType.TryGetValue(entityType, out var resourceNodes))
            resourceWithModifiedNodes.AddRange(resourceNodes);
        
        if(_removedResources.TryGetValue(entityType, out var removedResourceNodes))
            resourceWithModifiedNodes.RemoveAll(removedResourceNodes.Contains);
        
        if(_addedResources.TryGetValue(entityType, out var addedResourceNodes))
            resourceWithModifiedNodes.AddRange(addedResourceNodes);
        
        return resourceWithModifiedNodes;
    }
    
    public Node3D GetClosestElementByType(EntityType entityType, Node3D agent)
    {
        if (entityType == EntityType.Unknown || entityType == EntityType.None)
            return null;
        
        var resourceNodes = GetResource(entityType);
		var resourceWithModifiedNodes = new List<Node3D>();
        
        if (resourceNodes != null)
            resourceWithModifiedNodes.AddRange(resourceNodes);
        
        Node3D closestNode = null;
		
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

    private float GetDistanceToElement(Node3D agent, Node3D element)
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
        
        IsWorldStateModified = false;
    }
}