using Godot;

namespace GodotGOAPAI.Source.GodotHelpers;

public static class NodeExtensions
{
    public static object GetMetaData(this Node node, string metaDataName)
    {
        return !node.HasMeta(metaDataName) ? null : node.GetMeta(metaDataName).Obj;
    }
}