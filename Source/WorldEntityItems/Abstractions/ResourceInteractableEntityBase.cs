using Godot;
using GodotGOAPAI.Source.EventSystem;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.Goap.WorldState.WorldStateEvents;
using GodotGOAPAI.Source.GodotHelpers;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems.Abstractions;

public abstract partial class ResourceInteractableEntityBase : BaseEntity, IInteractableEntity, IResourceSpawningEntity
{
    private double _deltaTimeCummulative = 1; // Start at 1 to cause first hit to be instant and only then require cooldown between hits
    [Export]
    private AnimationPlayer _animationPlayer;
    
    protected abstract string AnimationName { get; }
    
    public bool IsEntityDestroyed { get; protected set; } = false;
    public abstract EntityType RequiredEntityTypeForInteraction { get; }
    public abstract EntityType ResourceEntityTypeToSpawnOnDestroy { get; }
    public abstract int ResourceToSpawnAmount { get; }
    public abstract int Durability { get; protected set; }

    public virtual void Interact(double deltaTime)
    {
        if (IsEntityDestroyed)
            return;
        
        _deltaTimeCummulative += deltaTime;

        if (_deltaTimeCummulative < 1)
            return;
        
        _deltaTimeCummulative %= 1;
        
        if (Durability <= 0)
        {
            var packedScene = SceneLoader.LoadScene(ResourceEntityTypeToSpawnOnDestroy);
            var collectionNodeToInstanceIn = GoapWorldStateService.Instance.WorldItemsCollectionNode;
            var instanceList = packedScene.InstanceSceneOnNode3D(collectionNodeToInstanceIn, GlobalPosition, ResourceToSpawnAmount);
            EventBus.Instance.SendEvent(new WorldStateChangedEvent()
            {
                ChangedNodes = new()
                {
                    { ResourceEntityTypeToSpawnOnDestroy, instanceList }
                }, 
                IsRemoved = false
            });
            IsEntityDestroyed = true;
            QueueFree();
            return;
        }
        
        Durability--;
        _animationPlayer.Play(AnimationName);
    }
}