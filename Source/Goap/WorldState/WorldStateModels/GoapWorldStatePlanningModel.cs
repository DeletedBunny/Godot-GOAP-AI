using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateModels;

public class GoapWorldStatePlanningModel
{
    private const string TrackedWorldStateModifier = "InWorld";
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
            SetTrackedState(resourceInWorld.Key.ToString(), resourceInWorld.Value.Count);
        }
    }
    
    public int GetTrackedState(string key) => _trackedWorldStates.GetValueOrDefault(key + TrackedWorldStateModifier, 0);
    public void SetTrackedState(string key, int value) => _trackedWorldStates[key + TrackedWorldStateModifier] = value;
}