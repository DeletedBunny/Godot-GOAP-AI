using System;
using Godot;
using GodotGOAPAI.Source.Goap.WorldState;
using GodotGOAPAI.Source.WorldEntityItems.Constants;
using GodotGOAPAI.Source.WorldEntityItems.Interfaces;

namespace GodotGOAPAI.Source.WorldEntityItems.Abstractions;

public abstract partial class PickupEntityBase : BaseEntity, IPickupEntity
{
    public virtual void Pickup(Node3D newOwner)
    {
        GlobalPosition = Vector3.Zero;
        GetParent().RemoveChild(this);
        newOwner.AddChild(this);
        GoapWorldStateService.Instance.EntityPickedUp(this);
    }

    public virtual void Drop()
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