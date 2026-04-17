using Godot.Collections;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.Agent;

public static class AgentInventoryHelper
{
    public static Dictionary<EntityType, int> InventoryConstants = new Dictionary<EntityType, int>()
    {
        { EntityType.Axe, 1 },
        { EntityType.Hammer, 1 },
        { EntityType.Log, 2 },
        { EntityType.Stone, 2 }
    };
}