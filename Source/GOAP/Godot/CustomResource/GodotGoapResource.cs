using Godot;
using GodotGOAPAI.Source.GOAP.Models;

namespace GodotGOAPAI.Source.GOAP.Godot.CustomResource;

[GlobalClass]
public partial class GodotGoapResource : Resource
{
    [Export(PropertyHint.Enum)] 
    public GoapResourceType GoapResourceType { get; private set; }
}