using System.Collections.Generic;

namespace GodotGOAPAI.Source.GOAP.Models;

public static class IgnoredWorldResourceTypes
{
    public static List<GoapResourceType> IgnoreList = new List<GoapResourceType>()
    {
        GoapResourceType.Tree,
        GoapResourceType.Mountain,
        GoapResourceType.Bush
    };
}

public enum GoapResourceType
{
    Axe,
    Hammer,
    Log,
    Stone,
    Tree,
    Mountain,
    Bush
}