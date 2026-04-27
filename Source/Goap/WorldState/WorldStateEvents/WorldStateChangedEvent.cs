using System.Collections.Generic;
using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;

public class WorldStateChangedEvent : IEvent
{
    public EntityType Type { get; init; }
    public Dictionary<EntityType, List<Node3D>> ChangedNodes { get; init; }
    public bool IsRemoved { get; init; }
}