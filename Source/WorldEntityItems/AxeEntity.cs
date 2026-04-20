using System;
using Godot;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems;

public partial class AxeEntity : Sprite3D, IEntity, IPickupEntity
{
    public EntityType EntityType => EntityType.Axe;
    public int ResourceToSpawnAmount => 0;
    public int Durability => int.MaxValue;

    public void Pickup(Node3D newOwner)
    {
        GlobalPosition = Vector3.Zero;
        GetParent().RemoveChild(this);
        newOwner.AddChild(this);
        GoapWorldStateService.Instance.EntityPickedUp(this);
    }

    public void Drop()
    {
        var parent = GetParent();
        var positionToDropAt = (parent as Node3D)?.GlobalPosition;
        if (!positionToDropAt.HasValue)
        {
            throw new Exception("Parent is does not have a GlobalPosition");
        }
        parent.RemoveChild(this);
        GoapWorldStateService.Instance.WorldItemsCollectionNode.AddChild(this);
        GlobalPosition = positionToDropAt.Value;
        GoapWorldStateService.Instance.EntityDropped(this);
    }
}