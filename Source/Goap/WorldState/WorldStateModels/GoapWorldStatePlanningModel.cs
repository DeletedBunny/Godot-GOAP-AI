using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStatePlanningModel
{
    public const string TrackedWorldStateModifier = "InWorld";
    private readonly Dictionary<string, int> _trackedWorldStates = new();
    
    public GoapWorldStatePlanningModel(GoapWorldStateMemento worldStateMemento)
    {
        GenerateTrackedWorldStates(worldStateMemento.GetWorldStateResources());
    }

    public GoapWorldStatePlanningModel(Dictionary<string, int> trackedWorldStates)
    {
        _trackedWorldStates = trackedWorldStates;
    }

    public GoapWorldStatePlanningModel GetCopy()
    {
        var dictionaryCopy = new Dictionary<string, int>(_trackedWorldStates);
        return new GoapWorldStatePlanningModel(dictionaryCopy);
    }
    
    private void GenerateTrackedWorldStates(ImmutableDictionary<EntityType, List<Node3D>> worldStateResources)
    {
        _trackedWorldStates.Clear();
        foreach (var resourceInWorld in worldStateResources)
        {
            SetTrackedState(resourceInWorld.Key + "InWorld", resourceInWorld.Value.Count);
        }
    }
    
    public void UpdateState(string key, int value)
    {
        var current = GetTrackedState(key);
        _trackedWorldStates[key] = current + value;
    }

    public void SyncState(GoapWorldStatePlanningModel otherModel)
    {
        _trackedWorldStates.Clear();
        foreach (var kvp in otherModel._trackedWorldStates)
        {
            _trackedWorldStates.Add(kvp.Key, kvp.Value);
        }
    }
    
    public int GetTrackedState(string key) => _trackedWorldStates.GetValueOrDefault(key, 0);
    public void SetTrackedState(string key, int value) => _trackedWorldStates[key] = value;
}