using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.World_State.World_State_Models;

public class GoapWorldStateMemento<TNode> where TNode : Node
{
    private readonly GoapWorldStateModel<TNode> _worldStateModel;
    private readonly Dictionary<GoapResourceType, List<TNode>> _removedResources = new Dictionary<GoapResourceType, List<TNode>>();
    private readonly Dictionary<GoapResourceType, List<TNode>> _addedResources = new Dictionary<GoapResourceType, List<TNode>>();
    
    public bool IsWorldStateModified { get; private set; }
    
    public GoapWorldStateMemento(GoapWorldStateModel<TNode> worldStateModel)
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
    
    public void AddModifiedResource(GoapResourceType resourceType, List<TNode> nodes, bool isRemoved)
    {
        if (isRemoved)
        {
            if (_removedResources.TryGetValue(resourceType, out var resource))
            {
                resource.AddRange(nodes);
            }
            else
            {
                _removedResources.Add(resourceType, nodes);
            }
        }
        else
        {
            if (_addedResources.TryGetValue(resourceType, out var resource))
            {
                resource.AddRange(nodes);
            }
            else
            {
                _addedResources.Add(resourceType, nodes);
            }
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

    public  IReadOnlyList<TNode> GetResource(GoapResourceType resourceType)
    {
        var resourceWithModifiedNodes = new List<TNode>();
        if (_worldStateModel.ResourcesAmountByType.TryGetValue(resourceType, out var resourceNodes))
            resourceWithModifiedNodes.AddRange(resourceNodes);
        
        if(_removedResources.TryGetValue(resourceType, out var removedResourceNodes))
            resourceWithModifiedNodes.RemoveAll(removedResourceNodes.Contains);
        
        if(_addedResources.TryGetValue(resourceType, out var addedResourceNodes))
            resourceWithModifiedNodes.AddRange(addedResourceNodes);
        
        return resourceWithModifiedNodes;
    }
    
    // Would be better to replace TNode generic with a custom type like GodotNode to pick between Node2D or Node3D based
    // on the type of the node. This would allow flexibility in adding new node types, although besides 3D and 2D I
    // doubt there will be another base positional node since we can't perceive higher than 3D.
    public TNode GetClosestElementByType(GoapResourceType resourceType, TNode agent)
    {
        _worldStateModel.ResourcesAmountByType.TryGetValue(resourceType, out var resourceNodes);
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
    }
}