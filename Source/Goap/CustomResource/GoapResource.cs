using Godot;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.Goap.CustomResource;

[GlobalClass]
public partial class GoapResource : Resource
{
    public static string Name => "GoapResourceType";
    [Export(PropertyHint.Enum)] 
    public EntityType EntityType { get; private set; }
}