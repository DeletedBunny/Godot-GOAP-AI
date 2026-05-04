using System.Collections.Generic;

namespace GodotGOAPAI.Source.WorldEntityItems.Constants;

public static class IgnoredWorldResourceTypes
{
    public static readonly List<EntityType> IgnoreList =
    [
        EntityType.Tree,
        EntityType.Mountain,
        EntityType.Bush
    ];
}

public static class ResourceToPathLookup
{
    public static readonly Dictionary<EntityType, string> EntityToPath = new()
    {
        { EntityType.Axe, "res://Resources/Units/axe.tscn" },
        { EntityType.Hammer, "res://Resources/Units/hammer.tscn" },
        { EntityType.Log, "res://Resources/Buildings/resource_lumber_single.tscn" },
        { EntityType.Stone, "res://Resources/Buildings/resource_stone_single.tscn" },
        { EntityType.HomeA, "res://Resources/Buildings/building_home_A_yellow.tscn" },
        { EntityType.BuildingZone, "res://Resources/Buildings/building_home_A_yellow.tscn" },
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
    HomeA,
    BuildingZone,
    Unknown,
    None
}