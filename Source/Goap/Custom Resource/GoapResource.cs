using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.Goap.Custom_Resource;

[GlobalClass]
public partial class GoapResource : Resource
{
    [Export(PropertyHint.Enum)] 
    public GoapResourceType GoapResourceType { get; private set; }
}