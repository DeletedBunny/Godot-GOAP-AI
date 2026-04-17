using Godot;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.GodotHelpers;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class TreeEntity : Node3D, IEntity, IInteractableEntity, IResourceSpawningEntity
{
    private bool _isDisposed = false;
    private int _amountOfHitsUntilCut = 4;
    private double _deltaTimeCummulative = 0;
    [Export]
    private AnimationPlayer _animationPlayer;
    [Export]
    private AnimatedSprite3D _treeSprite3D;
    
    public EntityType EntityType => EntityType.Tree;

    public EntityType RequiredEntityTypeForInteraction => EntityType.Axe;
    public bool IsEntityDestroyed => _isDisposed;

    public EntityType ResourceEntityTypeToSpawnOnDestroy => EntityType.Log;
    public int ResourceToSpawnAmount => 2;
    public int Durability => _amountOfHitsUntilCut;
    
    public override void _Ready()
    {
        var rng = new RandomNumberGenerator();
        rng.SetSeed("GoapTreeSeed".Hash());
        var treeFrame = rng.RandiRange(0, 4);
        _treeSprite3D.Frame = treeFrame;
    }

    public void Interact(double deltaTime)
    {
        if (_isDisposed)
            return;
        
        _deltaTimeCummulative += deltaTime;

        if (_deltaTimeCummulative < 1)
            return;
        
        _deltaTimeCummulative %= 1;
        
        if (_amountOfHitsUntilCut <= 0)
        {
            var logScene = SceneLoader.LoadScene(ResourceEntityTypeToSpawnOnDestroy);
            var collectionNodeToInstanceIn = GoapWorldStateService.Instance.WorldItemsCollectionNode;
            logScene.InstanceSceneOnNode3D(collectionNodeToInstanceIn, GlobalPosition, ResourceToSpawnAmount);
            _isDisposed = true;
            QueueFree();
            return;
        }
        
        _amountOfHitsUntilCut--;
        _animationPlayer.Play("shake");
    }
}