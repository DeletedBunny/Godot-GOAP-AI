using Godot;

namespace GodotGOAPAI.Source.GOAP.Models;

public partial class GoapTree : AnimatedSprite3D
{
    public override void _Ready()
    {
        var rng = new RandomNumberGenerator();
        rng.SetSeed("GoapTreeSeed".Hash());
        var treeFrame = rng.RandiRange(0, 4);
        Frame = treeFrame;
    }
}