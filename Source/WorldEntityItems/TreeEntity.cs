using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.GodotHelpers;
using GodotGOAPAI.Source.WorldEntityItems.Abstractions;
using GodotGOAPAI.Source.WorldEntityItems.Constants;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class TreeEntity : ResourceInteractableEntityBase
{
    [Export]
    private AnimatedSprite3D _treeSprite3D;
    
    protected override string AnimationName => "shake";
    
    public override EntityType EntityType => EntityType.Tree;
    public override EntityType RequiredEntityTypeForInteraction => EntityType.Axe;
    public override EntityType ResourceEntityTypeToSpawnOnDestroy => EntityType.Log;
    public override int ResourceToSpawnAmount => 2;
    public override int Durability { get; protected set; } = 3;
    
    public override void _Ready()
    {
        var rng = new RandomNumberGenerator();
        rng.SetSeed("GoapTreeSeed".Hash());
        var treeFrame = rng.RandiRange(0, 4);
        _treeSprite3D.Frame = treeFrame;
    }
}