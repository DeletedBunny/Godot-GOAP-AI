using System;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using System.Collections.Generic;
using System.Linq;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStateModel
{
    private readonly object _lock = new();
    private readonly Dictionary<string, int> _virtualStates = new();
    private readonly Dictionary<EntityType, List<Node3D>> _physicalResources = new();

    public GoapWorldStateModel()
    {
        foreach (var type in Enum.GetValues<EntityType>())
            _physicalResources[type] = new();
    }

    private GoapWorldStateModel(Dictionary<string, int> virtualStates, Dictionary<EntityType, List<Node3D>> physicalResources)
    {
        _virtualStates = new Dictionary<string, int>(virtualStates);
        foreach (var kvp in physicalResources)
            _physicalResources[kvp.Key] = new List<Node3D>(kvp.Value);
    }

    public void AddItems(EntityType entityType, List<Node3D> items)
    {
        lock (_lock)
        {
            if (!_physicalResources.ContainsKey(entityType))
                _physicalResources[entityType] = new List<Node3D>();
            _physicalResources[entityType].AddRange(items);
        }
    }

    public void RemoveItems(EntityType entityType, List<Node3D> items)
    {
        lock (_lock)
        {
            if(_physicalResources.TryGetValue(entityType, out var existingItems))
                existingItems.RemoveAll(items.Contains);
        }
    }

    public GoapWorldStateModel GetSimulationCopy(IEnumerable<(string, int)> agentInternalState)
    {
        lock (_lock)
        {
            var copy = Clone();
            foreach (var (key, value) in agentInternalState)
                copy.SetState(key, value);
            return copy;
        }
    }

    public GoapWorldStateModel Clone()
    {
        return new GoapWorldStateModel(_virtualStates, _physicalResources);
    }
    
    public void SyncState(GoapWorldStateModel other)
    {
        lock (_lock)
        {
            _virtualStates.Clear();
            foreach (var kvp in other._virtualStates)
                _virtualStates.Add(kvp.Key, kvp.Value);

            _physicalResources.Clear();
            foreach (var kvp in other._physicalResources)
                _physicalResources.Add(kvp.Key, new List<Node3D>(kvp.Value));
        }
    }

    public int GetState(string key)
    {
        if (key.EndsWith("InWorld"))
        {
            var typeString = key.Replace("InWorld", "");
            if(Enum.TryParse<EntityType>(typeString, out var type))
                return _physicalResources.TryGetValue(type, out var nodes) ? nodes.Count : 0;
        }
        
        return _virtualStates.GetValueOrDefault(key, 0);
    }
    
    public int GetState(EntityType entityType) => _physicalResources.TryGetValue(entityType, out var nodes) ? nodes.Count : 0;

    public void UpdateState(string key, int valueDelta)
    {
        if (key.EndsWith("InWorld"))
            return;
        
        _virtualStates[key] = GetState(key) + valueDelta;
    }
    
    private void SetState(string key, int value) => _virtualStates[key] = value;

    public Node3D GetClosestAndRemove(EntityType entityType, Vector3 fromPosition)
    {
        var closest = GetClosest(entityType, fromPosition);
        if(closest != null)
            _physicalResources[entityType].Remove(closest);
        
        return closest;
    }

    public Node3D GetClosest(EntityType entityType, Vector3 fromPosition)
    {
        if (!_physicalResources.TryGetValue(entityType, out var nodes) || nodes.Count == 0)
            return null;
        
        return nodes.OrderBy(node => node.GlobalPosition.DistanceSquaredTo(fromPosition)).First();
    }
}