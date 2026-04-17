using System.Collections.Generic;

namespace GodotGOAPAI.Source.WorldEntityItems.Constants;

public static class IgnoredWorldResourceTypes
{
    public static List<EntityType> IgnoreList = new List<EntityType>()
    {
        EntityType.Tree,
        EntityType.Mountain,
        EntityType.Bush
    };
}

public static class ResourceToPathLookup
{
    public static Dictionary<EntityType, string> EntityToPath = new Dictionary<EntityType, string>()
    {
        { EntityType.Axe, "res://Resources/Units/axe.tscn" },
        { EntityType.Hammer, "res://Resources/Units/hammer.tscn" },
        { EntityType.Log, "res://Resources/Buildings/resource_lumber_single.tscn" },
        { EntityType.Stone, "res://Resources/Buildings/resource_stone_single.tscn" },
        { EntityType.Unknown, "res://Resources/Buildings/sack.tscn" }
    };
}

public enum EntityType
{
    Axe,
    Hammer,
    Log,
    Stone,
    Tree,
    Mountain,
    Bush,
    Unknown
}